using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }  // Primary key

        [Required]
        public string Username { get; set; } = "";  // User's name

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";     // User's email

        [Required]
        public string Password { get; set; } = "";
    }
}
