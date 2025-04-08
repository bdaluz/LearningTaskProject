using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.User
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public required string Token { get; set; }
    }
}
