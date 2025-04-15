using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PersonalChallengePlatform.Models
{
    public class UserSettingsViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Profile Picture")]
        public string AvatarUrl { get; set; } = "/images/default-avatar.svg";
        
        [Display(Name = "Profile Picture URL")]
        public string ProfilePictureUrl { get; set; } = "/images/default-avatar.svg";
        
        [Display(Name = "Theme")]
        [Required]
        [StringLength(20)]
        public string SelectedTheme { get; set; } = "light";
        
        [Display(Name = "Dark Mode")]
        public bool IsDarkModeEnabled { get; set; }
        
        [Display(Name = "Email Notifications")]
        public bool EnableEmailNotifications { get; set; } = true;
        
        [Display(Name = "Bio")]
        [StringLength(500)]
        public string? Bio { get; set; }
        
        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Location")]
        [StringLength(100)]
        public string? Location { get; set; }

        // Security tab properties
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        // Theme options
        public static Dictionary<string, string> ThemeOptions { get; } = new()
        {
            { "light", "Light" },
            { "dark", "Dark" },
            { "auto", "System Default" }
        };
    }
} 