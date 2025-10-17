
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Tasks
{
    public class CreateDTO
    {
        [Required]
        [MinLength(1, ErrorMessage = "The task title must be at least 1 characters long.")]
        [MaxLength(100, ErrorMessage = "The task title cannot exceed 100 characters.")]
        public required string Title { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The task description must be at least 1 characters long.")]
        [MaxLength(1000, ErrorMessage = "The task description cannot exceed 1000 characters.")]
        public required string Description { get; set; }
    }
}
