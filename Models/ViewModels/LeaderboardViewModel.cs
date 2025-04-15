using System.Collections.Generic;

namespace PersonalChallengePlatform.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        public List<LeaderboardUser> TopUsers { get; set; } = new();
        public int CurrentUserRank { get; set; }
        public int TotalUsers { get; set; }
        public LeaderboardUser? CurrentUser { get; set; }
    }

    public class LeaderboardUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/images/default-avatar.svg";
        public int Points { get; set; }
        public int CompletedChallenges { get; set; }
        public string AchievementLevel { get; set; } = "Beginner";
    }
} 