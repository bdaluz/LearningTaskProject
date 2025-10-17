using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(256)]
        public string Title { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public User User { get; set; }
        public int UserId { get; set; }

        public ToDoTask(string title, string description)
        {
            Title = title;
            Description = description;
        }


    }
}
