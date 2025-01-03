using Microsoft.EntityFrameworkCore;
using PocketMapApp.Models;

namespace PocketMapApp.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Debt> Debts { get; set; }

        public DatabaseContext()
        {
            SQLitePCL.Batteries_V2.Init();
            //Database.EnsureDeleted(); // This will delete the existing database
            Database.EnsureCreated(); // This will create a new database with the current schema
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                Directory.CreateDirectory(FileSystem.AppDataDirectory);
                optionsBuilder.UseSqlite($"Filename={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.PreferredCurrency)
                      .HasMaxLength(10)
                      .HasDefaultValue("USD");
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.User)
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(t => t.Amount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(t => t.Date)
                      .IsRequired();

                entity.Property(t => t.Notes)
                      .HasMaxLength(500);

                entity.Property(t => t.Tags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => string.IsNullOrEmpty(v)
                              ? new List<string>()
                              : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );
            });

            // Configure Debt entity
            modelBuilder.Entity<Debt>(entity =>
            {
                entity.HasOne(d => d.User)
                      .WithMany()
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(d => d.Source)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(d => d.Amount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(d => d.DueDate)
                      .IsRequired();

                entity.Property(d => d.CreatedDate)
                      .IsRequired();

                entity.Property(d => d.Notes)
                      .HasMaxLength(500);
            });
        }
    }
}
