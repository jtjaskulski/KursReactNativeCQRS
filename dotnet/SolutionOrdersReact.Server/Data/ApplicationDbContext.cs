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

        // =========================================
        // DbSets - TABELE W BAZIE DANYCH
        // =========================================

        // Słowniki (lookup tables)
        public DbSet<Category> Categories { get; set; }
        public DbSet<UnitOfMeasurement> UnitOfMeasurements { get; set; }

        // Encje główne
        public DbSet<Item> Items { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Worker> Workers { get; set; }

        // Zamówienia (Master-Detail)
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================
            // KONFIGURACJA RELACJI - ITEM
            // =========================================

            // Item → Category (M:1, wymagana)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.IdCategory)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);  // Nie można usunąć kategorii z produktami

            // Item → UnitOfMeasurement (M:1, opcjonalna)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.UnitOfMeasurement)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.IdUnitOfMeasurement)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);  // Usunięcie jednostki = NULL w produktach

            // =========================================
            // KONFIGURACJA RELACJI - ORDER
            // =========================================

            // Order → Client (M:1, opcjonalna)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.IdClient)
                .IsRequired(false)  // Zamówienie może być anonimowe
                .OnDelete(DeleteBehavior.SetNull);  // Usunięcie klienta = NULL w zamówieniach

            // Order → Worker (M:1, opcjonalna)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Worker)
                .WithMany(w => w.Orders)
                .HasForeignKey(o => o.IdWorker)
                .IsRequired(false)  // Zamówienie może nie mieć przypisanego pracownika
                .OnDelete(DeleteBehavior.SetNull);  // Usunięcie pracownika = NULL w zamówieniach

            // =========================================
            // KONFIGURACJA RELACJI - ORDER_ITEM (MASTER-DETAIL)
            // =========================================

            // OrderItem → Order (M:1, wymagana, CASCADE DELETE)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.IdOrder)
                .IsRequired(true)  // Pozycja MUSI należeć do zamówienia
                .OnDelete(DeleteBehavior.Cascade);  // Usunięcie zamówienia = usunięcie pozycji

            // OrderItem → Item (M:1, wymagana, RESTRICT)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(oi => oi.IdItem)
                .IsRequired(true)  // Pozycja MUSI mieć produkt
                .OnDelete(DeleteBehavior.Restrict);  // Nie można usunąć produktu który jest w zamówieniu

            // =========================================
            // SEED DATA - SŁOWNIKI
            // =========================================

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    IdCategory = 1,
                    Name = "Elektronika",
                    Description = "Urządzenia elektroniczne",
                    IsActive = true
                },
                new Category
                {
                    IdCategory = 2,
                    Name = "Żywność",
                    Description = "Produkty spożywcze",
                    IsActive = true
                },
                new Category
                {
                    IdCategory = 3,
                    Name = "Odzież",
                    Description = "Ubrania i akcesoria",
                    IsActive = true
                }
            );

            modelBuilder.Entity<UnitOfMeasurement>().HasData(
                new UnitOfMeasurement
                {
                    IdUnitOfMeasurement = 1,
                    Name = "szt",
                    Description = "Sztuki",
                    IsActive = true
                },
                new UnitOfMeasurement
                {
                    IdUnitOfMeasurement = 2,
                    Name = "kg",
                    Description = "Kilogramy",
                    IsActive = true
                },
                new UnitOfMeasurement
                {
                    IdUnitOfMeasurement = 3,
                    Name = "l",
                    Description = "Litry",
                    IsActive = true
                }
            );

            // =========================================
            // SEED DATA - PRODUKTY
            // =========================================

            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    IdItem = 1,
                    Name = "Laptop Dell",
                    Description = "Laptop Dell Inspiron 15",
                    IdCategory = 1,
                    Price = 3500m,
                    Quantity = 10m,
                    IdUnitOfMeasurement = 1,
                    Code = "LAP001",
                    IsActive = true
                },
                new Item
                {
                    IdItem = 2,
                    Name = "Monitor Samsung",
                    Description = "Monitor 24 cale",
                    IdCategory = 1,
                    Price = 800m,
                    Quantity = 15m,
                    IdUnitOfMeasurement = 1,
                    Code = "MON001",
                    IsActive = true
                },
                new Item
                {
                    IdItem = 3,
                    Name = "Mysz Logitech",
                    Description = "Mysz bezprzewodowa",
                    IdCategory = 1,
                    Price = 150m,
                    Quantity = 50m,
                    IdUnitOfMeasurement = 1,
                    Code = "MYS001",
                    IsActive = true
                }
            );

            // =========================================
            // SEED DATA - KLIENCI
            // =========================================

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    IdClient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@email.com",
                    Phone = "123456789",
                    Address = "ul. Główna 1",
                    City = "Warszawa",
                    PostalCode = "00-001",
                    IsActive = true
                },
                new Client
                {
                    IdClient = 2,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@email.com",
                    Phone = "987654321",
                    Address = "ul. Rynek 5",
                    City = "Kraków",
                    PostalCode = "30-001",
                    IsActive = true
                }
            );

            // =========================================
            // SEED DATA - PRACOWNICY
            // =========================================

            modelBuilder.Entity<Worker>().HasData(
                new Worker
                {
                    IdWorker = 1,
                    FirstName = "Piotr",
                    LastName = "Wiśniewski",
                    Email = "piotr.wisniewski@firma.pl",
                    Phone = "111222333",
                    Address = "ul. Handlowa 10",
                    City = "Warszawa",
                    PostalCode = "00-100",
                    IsActive = true
                },
                new Worker
                {
                    IdWorker = 2,
                    FirstName = "Maria",
                    LastName = "Zielińska",
                    Email = "maria.zielinska@firma.pl",
                    Phone = "444555666",
                    Address = "ul. Biurowa 15",
                    City = "Kraków",
                    PostalCode = "30-100",
                    IsActive = true
                }
            );

            // =========================================
            // SEED DATA - ZAMÓWIENIA (MASTER)
            // =========================================

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    IdOrder = 1,
                    DataOrder = new DateTime(2024, 1, 15),
                    IdClient = 1,
                    IdWorker = 1,
                    Notes = "Pierwsze zamówienie testowe",
                    DeliveryDate = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Order
                {
                    IdOrder = 2,
                    DataOrder = new DateTime(2024, 2, 10),
                    IdClient = 2,
                    IdWorker = 2,
                    Notes = "Drugie zamówienie testowe",
                    DeliveryDate = new DateTime(2024, 2, 15),
                    IsActive = true
                }
            );

            // =========================================
            // SEED DATA - POZYCJE ZAMÓWIEŃ (DETAIL)
            // =========================================

            modelBuilder.Entity<OrderItem>().HasData(
                // Pozycje zamówienia #1
                new OrderItem
                {
                    IdOrderItem = 1,
                    IdOrder = 1,
                    IdItem = 1,  // Laptop Dell
                    Quantity = 2,
                    UnitPrice = 3500m,
                    IsActive = true
                },
                new OrderItem
                {
                    IdOrderItem = 2,
                    IdOrder = 1,
                    IdItem = 2,  // Monitor Samsung
                    Quantity = 3,
                    UnitPrice = 800m,
                    IsActive = true
                },
                // Pozycje zamówienia #2
                new OrderItem
                {
                    IdOrderItem = 3,
                    IdOrder = 2,
                    IdItem = 3,  // Mysz Logitech
                    Quantity = 5,
                    UnitPrice = 150m,
                    IsActive = true
                },
                new OrderItem
                {
                    IdOrderItem = 4,
                    IdOrder = 2,
                    IdItem = 1,  // Laptop Dell
                    Quantity = 1,
                    UnitPrice = 3500m,
                    IsActive = true
                }
            );
        }
    }
}