using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models;

namespace Services.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IEncryptionService _encryptionService;

        public DbSet<ToDoTask> ToDoTasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IEncryptionService encryptionService)
            : base(options)
        {
            _encryptionService = encryptionService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var encryptedConverter = new EncryptedStringConverter(_encryptionService);

            modelBuilder.Entity<ToDoTask>()
                .Property(t => t.Title)
                .HasConversion(encryptedConverter);

            modelBuilder.Entity<ToDoTask>()
                .Property(t => t.Description)
                .HasConversion(encryptedConverter);
        }
    }
}
