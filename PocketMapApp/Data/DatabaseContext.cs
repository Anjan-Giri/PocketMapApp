using Microsoft.EntityFrameworkCore; //provide classes and methods to work with database
using PocketMapApp.Models; //models

namespace PocketMapApp.Data
{
    //inherits dbcontext for database connection and management
    public class DatabaseContext : DbContext
    {
        //mapping entities to table
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Debt> Debts { get; set; }

        //initialize database context
        public DatabaseContext()
        {
            SQLitePCL.Batteries_V2.Init();
            //Database.EnsureDeleted(); //deleting existing database (development)
            Database.EnsureCreated(); //creating database
        }

        //configuring the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //avoiding reconfiguring
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db"); //path to store database file
                Directory.CreateDirectory(FileSystem.AppDataDirectory); //making sure the directory exists to store the file
                optionsBuilder.UseSqlite($"Filename={dbPath}"); //giving file path
            }
        }

        //datbase schema and entity
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //calling base method to apply default configurations

            //User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username)
                      .IsUnique(); //username should be unique

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.PreferredCurrency)
                      .HasMaxLength(10)
                      .HasDefaultValue("USD");
            });

            //Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                //one-to-many relation between user and transaction
                entity.HasOne(t => t.User)
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade); //for deleting transaction when user is deleted

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(100); //2 decimal places and 18 digits

                entity.Property(t => t.Amount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(t => t.Date)
                      .IsRequired();

                entity.Property(t => t.Notes)
                      .HasMaxLength(500);

                entity.Property(t => t.Tags)
                      .HasConversion(
                          v => string.Join(',', v), //converting list to string
                          v => string.IsNullOrEmpty(v) //converting to list when getting value
                              ? new List<string>()
                              : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );
            });

            //Debt entity
            modelBuilder.Entity<Debt>(entity =>
            {
                //one-to-many relation between user and debt
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
