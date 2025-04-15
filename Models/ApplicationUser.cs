using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            CreatedChallenges = new List<Challenge>();
            ChallengeProgress = new List<Progress>();
            Categories = new List<Category>();
            FirstName = string.Empty;
            LastName = string.Empty;
            SelectedTheme = "light";
            AvatarUrl = "/images/default-avatar.svg";
            CreatedAt = DateTime.UtcNow;
        }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(255)]
        public string AvatarUrl { get; set; }

        [StringLength(20)]
        public string SelectedTheme { get; set; }

        public bool EnableEmailNotifications { get; set; } = true;

        public int Points { get; set; }
        public int CompletedChallenges { get; set; }
        public int Level { get; set; }
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Challenge> CreatedChallenges { get; set; }
        public virtual ICollection<Progress> ChallengeProgress { get; set; }
        public virtual ICollection<Category> Categories { get; set; }

        // Statistics
        public virtual ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
        public virtual ICollection<UserStatistic> Statistics { get; set; } = new List<UserStatistic>();
    }
}