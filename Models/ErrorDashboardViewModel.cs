using System.Collections.Generic;

namespace PersonalChallengePlatform.Models
{
    public class ErrorDashboardViewModel
    {
        public List<NotFoundError> RecentErrors { get; set; } = new();
        public Dictionary<string, int> ErrorCounts { get; set; } = new();
    }
} 