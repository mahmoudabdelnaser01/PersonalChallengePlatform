using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalChallengePlatform.Models
{
    public class NotFoundError
    {
        public NotFoundError()
        {
            Path = string.Empty;
            Referrer = string.Empty;
            UserId = string.Empty;
            UserAgent = string.Empty;
            IPAddress = string.Empty;
            QueryString = string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Referrer { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string UserAgent { get; set; }

        [Required]
        public string IPAddress { get; set; }

        [Required]
        public string QueryString { get; set; }
    }
} 