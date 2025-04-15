namespace PersonalChallengePlatform.Services
{
    public class AchievementService : IAchievementService
    {
        public string DetermineAchievementLevel(int points)
        {
            return points switch
            {
                >= 1000 => "Champion",
                >= 750 => "Elite",
                >= 500 => "Advanced",
                >= 250 => "Intermediate",
                _ => "Beginner"
            };
        }
    }
} 