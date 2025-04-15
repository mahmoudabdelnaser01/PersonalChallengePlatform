using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class Progress
    {
        public Progress()
        {
            Notes = string.Empty;
            Description = string.Empty;
        }

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public int ChallengeId { get; set; }
        public virtual Challenge Challenge { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Points are required")]
        [Range(0, int.MaxValue, ErrorMessage = "Points cannot be negative")]
        [Display(Name = "Points Earned")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string GetChallengeName() => Challenge?.Title ?? "Unknown Challenge";
        public string GetFormattedDate() => Date.ToString("yyyy-MM-dd");
    }
}