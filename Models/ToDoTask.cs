
using System.ComponentModel.DataAnnotations;

namespace ProjetoTasks.Models
{
    internal class ToDoTask
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
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
