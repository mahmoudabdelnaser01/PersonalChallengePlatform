using System.Collections.Generic;

namespace PersonalChallengePlatform.Models.ViewModels
{
    public class ChallengeViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Challenge> Challenges { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
} 