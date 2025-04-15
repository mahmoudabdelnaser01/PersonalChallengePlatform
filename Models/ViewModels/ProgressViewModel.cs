using System.Collections.Generic;

namespace PersonalChallengePlatform.Models.ViewModels
{
    public class ProgressViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Progress> Progress { get; set; } = new();
        public int TotalChallenges { get; set; }
        public int CompletedChallenges { get; set; }
        public int CurrentStreak { get; set; }
    }
} 