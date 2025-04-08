using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<ToDoTask> Tasks { get; set; } = new List<ToDoTask>();

        public string? Token { get; set; }

        public User(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
