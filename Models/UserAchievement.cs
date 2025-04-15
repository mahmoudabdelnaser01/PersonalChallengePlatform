using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Level { get; set; } = "Beginner";

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
} 