using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PersonalChallengePlatform.Models.ViewModels
{
    public class ChallengeCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Challenge Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(ChallengeCreateViewModel), nameof(ValidateEndDate))]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public static ValidationResult? ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = (ChallengeCreateViewModel)context.ObjectInstance;
            if (endDate <= instance.StartDate)
            {
                return new ValidationResult("End date must be after start date");
            }
            return ValidationResult.Success;
        }
    }
} 