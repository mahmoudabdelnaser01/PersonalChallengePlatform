using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class Challenge
    {
        public Challenge()
        {
            Title = string.Empty;
            Description = string.Empty;
            Progress = new List<Progress>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Challenge Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Compare("StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Progress> Progress { get; set; }

        public string GetCategoryName() => Category?.Name ?? "Unknown Category";
        public string GetUserName() => User?.UserName ?? "Unknown User";
    }
}