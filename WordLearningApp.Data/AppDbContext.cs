using Microsoft.EntityFrameworkCore;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        
        public DbSet<TestResult> TestResults { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // ЗАМЕНИТЬ на SQL Server
            // options.UseSqlite("Data Source=WordLearningApp.db");

            // Вариант 1: Windows Authentication (рекомендуется)
            options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=WordLearningAppDB;Trusted_Connection=True;TrustServerCertificate=True;");

            // Вариант 2: SQL Authentication (с логином и паролем)
            // options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=WordLearningApp;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=True;");

            // Вариант 3: LocalDB
            // options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WordLearningApp;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Word>()
                .HasOne(w => w.Category)
                .WithMany()
                .HasForeignKey(w => w.CategoryId);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.Word)
                .WithMany()
                .HasForeignKey(up => up.WordId);

            modelBuilder.Entity<Word>()
                .Property(w => w.Difficulty)
                .HasDefaultValue(1);

            modelBuilder.Entity<Word>()
                .Property(w => w.AddedDate)
                .HasDefaultValueSql("GETDATE()"); // Для SQL Server
        }
    }
}