using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersonalChallengePlatform.Services
{
    public class ErrorNotificationLogger : ILogger
    {
        private readonly string _name;
        private readonly ErrorNotificationLoggerConfiguration _config;

        public ErrorNotificationLogger(string name, ErrorNotificationLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        IDisposable ILogger.BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                if (!IsEnabled(logLevel))
                    return;

                if (exception == null)
                    return;

                var message = formatter?.Invoke(state, exception) ?? "Unknown error occurred";
                var errorDetails = new
                {
                    Timestamp = DateTime.UtcNow,
                    Level = logLevel.ToString(),
                    Message = message,
                    Exception = exception.ToString(),
                    RequestPath = state?.ToString() ?? "Unknown",
                    Environment = _config.Environment ?? "Unknown"
                };

                _ = SendErrorNotificationAsync(errorDetails);
            }
            catch (Exception ex)
            {
                // Log to console as a last resort
                Console.WriteLine($"Failed to log error: {ex.Message}");
            }
        }

        private async Task SendErrorNotificationAsync(object errorDetails)
        {
            try
            {
                if (errorDetails == null)
                {
                    throw new ArgumentNullException(nameof(errorDetails));
                }

                if (_config.NotificationType == "Slack" && !string.IsNullOrEmpty(_config.WebhookUrl))
                {
                    await SendSlackNotificationAsync(errorDetails);
                }
                else if (_config.NotificationType == "Email" && !string.IsNullOrEmpty(_config.EmailEndpoint))
                {
                    await SendEmailNotificationAsync(errorDetails);
                }
                else
                {
                    Console.WriteLine("Error notification type not configured or missing required settings");
                }
            }
            catch (Exception ex)
            {
                // Log the notification failure but don't throw
                Console.WriteLine($"Failed to send error notification: {ex.Message}");
            }
        }

        private async Task SendSlackNotificationAsync(object errorDetails)
        {
            try
            {
                using var client = new HttpClient();
                var payload = new
                {
                    text = $"🚨 *Error in {_config.Environment ?? "Unknown"}*\n" +
                          $"*Message:* {errorDetails.GetType().GetProperty("Message")?.GetValue(errorDetails) ?? "Unknown"}\n" +
                          $"*Path:* {errorDetails.GetType().GetProperty("RequestPath")?.GetValue(errorDetails) ?? "Unknown"}\n" +
                          $"*Exception:* ```{errorDetails.GetType().GetProperty("Exception")?.GetValue(errorDetails) ?? "Unknown"}```"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(_config.WebhookUrl, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send Slack notification: {ex.Message}");
            }
        }

        private async Task SendEmailNotificationAsync(object errorDetails)
        {
            using var client = new HttpClient();
            var emailContent = new
            {
                to = _config.EmailRecipients,
                subject = $"Error in {_config.Environment} - {errorDetails.GetType().GetProperty("Message")?.GetValue(errorDetails)}",
                text = JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions { WriteIndented = true })
            };

            var content = new StringContent(
                JsonSerializer.Serialize(emailContent),
                Encoding.UTF8,
                "application/json");

            await client.PostAsync(_config.EmailEndpoint, content);
        }
    }

    public class ErrorNotificationLoggerProvider : ILoggerProvider
    {
        private readonly ErrorNotificationLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, ErrorNotificationLogger> _loggers = new();

        public ErrorNotificationLoggerProvider(ErrorNotificationLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ErrorNotificationLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    public class ErrorNotificationLoggerConfiguration
    {
        public string NotificationType { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
        public string EmailEndpoint { get; set; } = string.Empty;
        public string[] EmailRecipients { get; set; } = Array.Empty<string>();
        public string Environment { get; set; } = "Development";
    }
} 