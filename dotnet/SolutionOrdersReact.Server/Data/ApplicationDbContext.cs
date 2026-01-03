using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - tabele w bazie
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UnitOfMeasurement> UnitOfMeasurements { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji Item -> Category (wiele-do-jednego)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.IdCategory)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfiguracja relacji Item -> UnitOfMeasurement (wiele-do-jednego, opcjonalna)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.UnitOfMeasurement)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.IdUnitOfMeasurement)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed data - początkowe dane
            modelBuilder.Entity<Category>().HasData(
                new Category { IdCategory = 1, Name = "Elektronika", Description = "Urządzenia elektroniczne", IsActive = true },
                new Category { IdCategory = 2, Name = "Żywność", Description = "Produkty spożywcze", IsActive = true },
                new Category { IdCategory = 3, Name = "Odzież", Description = "Ubrania i akcesoria", IsActive = true }
            );

            modelBuilder.Entity<UnitOfMeasurement>().HasData(
                new UnitOfMeasurement { IdUnitOfMeasurement = 1, Name = "szt", Description = "Sztuki", IsActive = true },
                new UnitOfMeasurement { IdUnitOfMeasurement = 2, Name = "kg", Description = "Kilogramy", IsActive = true },
                new UnitOfMeasurement { IdUnitOfMeasurement = 3, Name = "l", Description = "Litry", IsActive = true }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item { IdItem = 1, Name = "Laptop Dell", Description = "Laptop Dell Inspiron 15", IdCategory = 1, Price = 3500m, Quantity = 10m, IdUnitOfMeasurement = 1, Code = "LAP001", IsActive = true },
                new Item { IdItem = 2, Name = "Monitor Samsung", Description = "Monitor 24 cale", IdCategory = 1, Price = 800m, Quantity = 15m, IdUnitOfMeasurement = 1, Code = "MON001", IsActive = true },
                new Item { IdItem = 3, Name = "Mysz Logitech", Description = "Mysz bezprzewodowa", IdCategory = 1, Price = 150m, Quantity = 50m, IdUnitOfMeasurement = 1, Code = "MYS001", IsActive = true }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client { IdClient = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@email.com", Phone = "123456789", City = "Warszawa", IsActive = true },
                new Client { IdClient = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@email.com", Phone = "987654321", City = "Kraków", IsActive = true }
            );
        }
    }
}
