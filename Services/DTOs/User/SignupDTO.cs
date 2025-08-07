using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.User
{
    public class SignupDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "The username must be at least 3 characters long.")]
        [MaxLength(36, ErrorMessage = "The username cannot exceed 36 characters.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,10}$", ErrorMessage = "Invalid email format.")]
        [MinLength(6, ErrorMessage = "Please verify the format of the email provided.")]
        [MaxLength(254, ErrorMessage = "The email provided is invalid.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(256, ErrorMessage = "Password is too long.")]
        public required string Password { get; set; }
    }
}
