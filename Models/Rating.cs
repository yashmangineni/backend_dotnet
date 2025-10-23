using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Score { get; set; } // Rating score (1-5)

        public string? Comment { get; set; } // Optional comment

        public DateTime Date { get; set; } = DateTime.UtcNow; // When the rating was submitted

        // Foreign key to User
        public int? UserId { get; set; }
        public User? User { get; set; } // Navigation property
    }
}