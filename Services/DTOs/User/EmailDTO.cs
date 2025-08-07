using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.User
{
    public class EmailDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MinLength(6, ErrorMessage = "Please verify the format of the email provided.")]
        [MaxLength(254, ErrorMessage = "The email provided is invalid.")]
        public required string Email { get; set; }
    }
}
