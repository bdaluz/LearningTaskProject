using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.User
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(256, ErrorMessage = "Password is too long.")]
        public required string NewPassword { get; set; }


        [Required(ErrorMessage = "Token is required.")]
        [Length(43, 43, ErrorMessage = "Token must have 43 characters.")]
        public required string Token { get; set; }
    }
}
