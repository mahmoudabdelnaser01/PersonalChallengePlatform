using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class Category
    {
        public Category()
        {
            Name = string.Empty;
            Description = string.Empty;
            Challenges = new List<Challenge>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public ICollection<Challenge> Challenges { get; set; }
    }
}