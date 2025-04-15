using System;
using System.Collections.Generic;

namespace PersonalChallengePlatform.Models
{
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public List<ErrorSuggestion> Suggestions { get; set; } = new List<ErrorSuggestion>();
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string StatusDescription { get; set; } = string.Empty;
    }

    public class ErrorSuggestion
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
