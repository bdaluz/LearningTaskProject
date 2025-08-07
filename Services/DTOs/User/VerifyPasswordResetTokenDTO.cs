using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.User
{
    public class VerifyPasswordResetTokenDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public required string Token { get; set; }
    }
}
