using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class UserStatistic
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Value { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
} 