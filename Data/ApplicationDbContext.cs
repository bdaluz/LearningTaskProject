using Microsoft.EntityFrameworkCore;
using ProjetoTasks.Models;

namespace ProjetoTasks.Data
{
    internal class ApplicationDbContext : DbContext
    {
        public DbSet<ToDoTask> ToDoTasks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Integrated Security=true;Database=TaskProject;TrustServercertificate=true");
        }
    }
}
