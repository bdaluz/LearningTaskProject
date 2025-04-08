using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Tasks
{
    public class EditDTO
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
    }
}
