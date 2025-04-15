using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Data;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace PersonalChallengePlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly INotFoundErrorService _notFoundErrorService;

        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDbContext context,
            INotFoundErrorService notFoundErrorService)
        {
            _logger = logger;
            _context = context;
            _notFoundErrorService = notFoundErrorService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                if (string.IsNullOrEmpty(requestId))
                {
                    requestId = "Unknown";
                }

                return View(new ErrorViewModel 
                { 
                    RequestId = requestId,
                    StatusCode = 500,
                    Title = "Error",
                    Message = "An error occurred while processing your request.",
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Error action");
                return StatusCode(500);
            }
        }

        [Route("/404")]
        [Route("/Home/NotFound")]
        public new async Task<IActionResult> NotFound()
        {
            try
            {
                var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : string.Empty;
                var referrer = Request.Headers["Referer"].ToString() ?? string.Empty;
                var userAgent = Request.Headers["User-Agent"].ToString() ?? string.Empty;
                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var path = Request.Path.ToString() ?? "/unknown";

                if (!string.IsNullOrEmpty(userId))
                {
                    await _notFoundErrorService.LogNotFoundErrorAsync(
                        path,
                        referrer,
                        userId,
                        userAgent,
                        ipAddress
                    );
                }

                var model = new ErrorViewModel
                {
                    StatusCode = 404,
                    Title = "Page Not Found",
                    Message = "The page you're looking for doesn't exist or has been moved.",
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                if (model.IsAuthenticated)
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "View Your Challenges", Url = "/Challenge", Icon = "fa-tasks" },
                        new ErrorSuggestion { Title = "Visit Dashboard", Url = "/Dashboard", Icon = "fa-chart-line" },
                        new ErrorSuggestion { Title = "Track Progress", Url = "/Progress", Icon = "fa-chart-bar" },
                        new ErrorSuggestion { Title = "Check Leaderboard", Url = "/Leaderboard", Icon = "fa-trophy" }
                    });
                }
                else
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "Sign In", Url = "/Account/Login", Icon = "fa-sign-in-alt" },
                        new ErrorSuggestion { Title = "Create Account", Url = "/Account/Register", Icon = "fa-user-plus" },
                        new ErrorSuggestion { Title = "Privacy Policy", Url = "/Home/Privacy", Icon = "fa-shield-alt" }
                    });
                }

                Response.StatusCode = 404;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the NotFound action");
                return StatusCode(500);
            }
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            try
            {
                var model = new ErrorViewModel
                {
                    StatusCode = 403,
                    Title = "Access Denied",
                    Message = "You don't have permission to access this resource.",
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                if (model.IsAuthenticated)
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "View Your Challenges", Url = "/Challenge", Icon = "fa-tasks" },
                        new ErrorSuggestion { Title = "Visit Dashboard", Url = "/Dashboard", Icon = "fa-chart-line" },
                        new ErrorSuggestion { Title = "Check Leaderboard", Url = "/Leaderboard", Icon = "fa-trophy" }
                    });
                }
                else
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "Sign In", Url = "/Account/Login", Icon = "fa-sign-in-alt" },
                        new ErrorSuggestion { Title = "Create Account", Url = "/Account/Register", Icon = "fa-user-plus" }
                    });
                }

                Response.StatusCode = 403;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Forbidden action");
                return StatusCode(500);
            }
        }

        [AllowAnonymous]
        public IActionResult InternalServerError()
        {
            try
            {
                var model = new ErrorViewModel
                {
                    StatusCode = 500,
                    Title = "Internal Server Error",
                    Message = "Something went wrong on our end. Please try again later.",
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                if (model.IsAuthenticated)
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "Go to Dashboard", Url = "/Dashboard", Icon = "fa-chart-line" },
                        new ErrorSuggestion { Title = "View Challenges", Url = "/Challenge", Icon = "fa-tasks" }
                    });
                }
                else
                {
                    model.Suggestions.AddRange(new[]
                    {
                        new ErrorSuggestion { Title = "Go to Home", Url = "/", Icon = "fa-home" },
                        new ErrorSuggestion { Title = "Contact Support", Url = "/Home/Contact", Icon = "fa-envelope" }
                    });
                }

                Response.StatusCode = 500;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the InternalServerError action");
                return StatusCode(500);
            }
        }
    }
}
