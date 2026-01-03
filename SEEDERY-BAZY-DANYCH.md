# Seedery Bazy Danych - Dane Początkowe

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

---

## 🎯 Cel

Seedery to mechanizm wypełniania bazy danych początkowymi danymi. Po rozpakowaniu projektu i uruchomieniu migracji, baza jest **pusta**. Seedery automatycznie dodają niezbędne dane do działania aplikacji.

---

## CZĘŚĆ 1: Teoria Seederów

### 1.1. Po co seedery?

**Problem bez seederów:**
```
1. Student rozpakowuje projekt
2. Uruchamia migracje → pusta baza
3. Otwiera aplikację mobilną → puste listy
4. Próbuje dodać produkt → błąd "Brak kategorii"
5. Frustracja 😤
```

**Z seederami:**
```
1. Student rozpakowuje projekt
2. Uruchamia migracje + seedery → baza z danymi
3. Otwiera aplikację → widzi przykładowe dane
4. Może od razu testować CRUD
5. Sukces 🎉
```

### 1.2. Co seedować?

| Encja | Czy seedować? | Dlaczego? |
|-------|---------------|-----------|
| **Category** | ✅ TAK | Wymagane do tworzenia produktów |
| **UnitOfMeasurement** | ✅ TAK | Wymagane do produktów |
| **Worker** | ✅ TAK | Potrzebny do zamówień |
| **Client** | ⚠️ Opcjonalnie | Przydatne do testów |
| **Item** | ⚠️ Opcjonalnie | Przykładowe produkty |
| **Order** | ❌ NIE | Użytkownik tworzy sam |

---

## CZĘŚĆ 2: Implementacja w Entity Framework Core

### 2.1. Metoda 1: HasData w OnModelCreating (Zalecana)

**Data/ApplicationDbContext.cs:**

```csharp
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasurement> UnitOfMeasurements => Set<UnitOfMeasurement>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== SEEDERY ====================
        
        SeedCategories(modelBuilder);
        SeedUnitsOfMeasurement(modelBuilder);
        SeedWorkers(modelBuilder);
        SeedClients(modelBuilder);
        SeedItems(modelBuilder);
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category 
            { 
                IdCategory = 1, 
                Name = "Elektronika", 
                Description = "Urządzenia elektroniczne i akcesoria",
                IsActive = true 
            },
            new Category 
            { 
                IdCategory = 2, 
                Name = "AGD", 
                Description = "Sprzęt gospodarstwa domowego",
                IsActive = true 
            },
            new Category 
            { 
                IdCategory = 3, 
                Name = "Meble", 
                Description = "Meble domowe i biurowe",
                IsActive = true 
            },
            new Category 
            { 
                IdCategory = 4, 
                Name = "Odzież", 
                Description = "Ubrania i akcesoria",
                IsActive = true 
            },
            new Category 
            { 
                IdCategory = 5, 
                Name = "Żywność", 
                Description = "Produkty spożywcze",
                IsActive = true 
            }
        );
    }

    private static void SeedUnitsOfMeasurement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnitOfMeasurement>().HasData(
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 1, 
                Name = "szt.", 
                Description = "Sztuka",
                IsActive = true 
            },
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 2, 
                Name = "kg", 
                Description = "Kilogram",
                IsActive = true 
            },
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 3, 
                Name = "l", 
                Description = "Litr",
                IsActive = true 
            },
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 4, 
                Name = "m", 
                Description = "Metr",
                IsActive = true 
            },
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 5, 
                Name = "opak.", 
                Description = "Opakowanie",
                IsActive = true 
            },
            new UnitOfMeasurement 
            { 
                IdUnitOfMeasurement = 6, 
                Name = "kpl.", 
                Description = "Komplet",
                IsActive = true 
            }
        );
    }

    private static void SeedWorkers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Worker>().HasData(
            new Worker 
            { 
                IdWorker = 1, 
                FirstName = "Jan", 
                LastName = "Kowalski",
                Login = "jkowalski",
                Password = "haslo123", // W produkcji: zahashowane!
                IsActive = true 
            },
            new Worker 
            { 
                IdWorker = 2, 
                FirstName = "Anna", 
                LastName = "Nowak",
                Login = "anowak",
                Password = "haslo123",
                IsActive = true 
            },
            new Worker 
            { 
                IdWorker = 3, 
                FirstName = "Admin", 
                LastName = "System",
                Login = "admin",
                Password = "admin123",
                IsActive = true 
            }
        );
    }

    private static void SeedClients(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client 
            { 
                IdClient = 1, 
                Name = "Firma ABC Sp. z o.o.", 
                Adress = "ul. Główna 1, 00-001 Warszawa",
                PhoneNumber = "+48 123 456 789",
                IsActive = true 
            },
            new Client 
            { 
                IdClient = 2, 
                Name = "XYZ Corporation", 
                Adress = "ul. Biznesowa 15, 30-001 Kraków",
                PhoneNumber = "+48 987 654 321",
                IsActive = true 
            },
            new Client 
            { 
                IdClient = 3, 
                Name = "Klient Detaliczny", 
                Adress = "ul. Prywatna 5, 80-001 Gdańsk",
                PhoneNumber = "+48 555 123 456",
                IsActive = true 
            }
        );
    }

    private static void SeedItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>().HasData(
            // Elektronika
            new Item 
            { 
                IdItem = 1,
                Name = "Laptop Dell XPS 15", 
                Description = "Laptop biznesowy z procesorem Intel i7",
                IdCategory = 1, // Elektronika
                IdUnitOfMeasurement = 1, // szt.
                Price = 5999.99m,
                Quantity = 10,
                Code = "DELL-XPS-15",
                IsActive = true 
            },
            new Item 
            { 
                IdItem = 2,
                Name = "Monitor Samsung 27\"", 
                Description = "Monitor 4K UHD",
                IdCategory = 1,
                IdUnitOfMeasurement = 1,
                Price = 1299.00m,
                Quantity = 25,
                Code = "SAM-MON-27",
                IsActive = true 
            },
            // AGD
            new Item 
            { 
                IdItem = 3,
                Name = "Lodówka Samsung", 
                Description = "Lodówka side-by-side z kostkarką",
                IdCategory = 2, // AGD
                IdUnitOfMeasurement = 1,
                Price = 4500.00m,
                Quantity = 5,
                Code = "SAM-FRIDGE-01",
                IsActive = true 
            },
            new Item 
            { 
                IdItem = 4,
                Name = "Pralka Bosch", 
                Description = "Pralka automatyczna 8kg",
                IdCategory = 2,
                IdUnitOfMeasurement = 1,
                Price = 2200.00m,
                Quantity = 8,
                Code = "BOSCH-WASH-8",
                IsActive = true 
            },
            // Meble
            new Item 
            { 
                IdItem = 5,
                Name = "Biurko gamingowe", 
                Description = "Biurko z podświetleniem RGB",
                IdCategory = 3, // Meble
                IdUnitOfMeasurement = 1,
                Price = 899.00m,
                Quantity = 15,
                Code = "DESK-GAME-01",
                IsActive = true 
            },
            // Żywność
            new Item 
            { 
                IdItem = 6,
                Name = "Kawa ziarnista", 
                Description = "Arabica 100%, 1kg",
                IdCategory = 5, // Żywność
                IdUnitOfMeasurement = 2, // kg
                Price = 89.99m,
                Quantity = 100,
                Code = "COFFEE-ARAB-1",
                IsActive = true 
            }
        );
    }
}
```

### 2.2. Utworzenie migracji z seederami

Po dodaniu HasData, utwórz nową migrację:

```bash
# W folderze projektu .NET
dotnet ef migrations add SeedInitialData

# Zastosuj migrację
dotnet ef database update
```

**Wynik:** Migracja zawiera INSERT dla wszystkich seedów.

---

## CZĘŚĆ 3: Metoda 2 - Seeder jako serwis (Alternatywa)

Dla bardziej dynamicznych danych lub gdy chcesz seedować warunkowo:

### 3.1. Interfejs i implementacja

**Services/IDataSeeder.cs:**
```csharp
namespace SolutionOrdersReact.Server.Services;

public interface IDataSeeder
{
    Task SeedAsync();
}
```

**Services/DataSeeder.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Services;

public class DataSeeder : IDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Sprawdzanie czy potrzebne seedowanie...");

        // Seeduj tylko jeśli baza jest pusta
        if (await _context.Categories.AnyAsync())
        {
            _logger.LogInformation("Baza już zawiera dane - pomijam seedowanie");
            return;
        }

        _logger.LogInformation("Seedowanie danych początkowych...");

        await SeedCategoriesAsync();
        await SeedUnitsAsync();
        await SeedWorkersAsync();
        await SeedClientsAsync();
        await SeedItemsAsync();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Seedowanie zakończone pomyślnie!");
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new List<Category>
        {
            new() { Name = "Elektronika", Description = "Urządzenia elektroniczne", IsActive = true },
            new() { Name = "AGD", Description = "Sprzęt domowy", IsActive = true },
            new() { Name = "Meble", Description = "Meble domowe i biurowe", IsActive = true },
            new() { Name = "Odzież", Description = "Ubrania i akcesoria", IsActive = true },
            new() { Name = "Żywność", Description = "Produkty spożywcze", IsActive = true },
        };

        await _context.Categories.AddRangeAsync(categories);
        _logger.LogInformation("Dodano {Count} kategorii", categories.Count);
    }

    private async Task SeedUnitsAsync()
    {
        var units = new List<UnitOfMeasurement>
        {
            new() { Name = "szt.", Description = "Sztuka", IsActive = true },
            new() { Name = "kg", Description = "Kilogram", IsActive = true },
            new() { Name = "l", Description = "Litr", IsActive = true },
            new() { Name = "m", Description = "Metr", IsActive = true },
            new() { Name = "opak.", Description = "Opakowanie", IsActive = true },
        };

        await _context.UnitOfMeasurements.AddRangeAsync(units);
        _logger.LogInformation("Dodano {Count} jednostek miary", units.Count);
    }

    private async Task SeedWorkersAsync()
    {
        var workers = new List<Worker>
        {
            new() { FirstName = "Jan", LastName = "Kowalski", Login = "jkowalski", Password = "test123", IsActive = true },
            new() { FirstName = "Anna", LastName = "Nowak", Login = "anowak", Password = "test123", IsActive = true },
            new() { FirstName = "Admin", LastName = "System", Login = "admin", Password = "admin", IsActive = true },
        };

        await _context.Workers.AddRangeAsync(workers);
        _logger.LogInformation("Dodano {Count} pracowników", workers.Count);
    }

    private async Task SeedClientsAsync()
    {
        var clients = new List<Client>
        {
            new() { Name = "Firma ABC", Adress = "Warszawa, ul. Główna 1", PhoneNumber = "123456789", IsActive = true },
            new() { Name = "XYZ Corp", Adress = "Kraków, ul. Biznesowa 15", PhoneNumber = "987654321", IsActive = true },
        };

        await _context.Clients.AddRangeAsync(clients);
        _logger.LogInformation("Dodano {Count} klientów", clients.Count);
    }

    private async Task SeedItemsAsync()
    {
        // Pobierz ID kategorii i jednostek (zostały już dodane)
        await _context.SaveChangesAsync(); // Zapisz żeby mieć ID

        var elektronika = await _context.Categories.FirstAsync(c => c.Name == "Elektronika");
        var sztuka = await _context.UnitOfMeasurements.FirstAsync(u => u.Name == "szt.");

        var items = new List<Item>
        {
            new() 
            { 
                Name = "Laptop Dell", 
                Description = "Laptop biznesowy i7",
                IdCategory = elektronika.IdCategory,
                IdUnitOfMeasurement = sztuka.IdUnitOfMeasurement,
                Price = 4999.99m,
                Quantity = 10,
                Code = "DELL-001",
                IsActive = true 
            },
            new() 
            { 
                Name = "Monitor 27\"", 
                Description = "Monitor 4K",
                IdCategory = elektronika.IdCategory,
                IdUnitOfMeasurement = sztuka.IdUnitOfMeasurement,
                Price = 1299.00m,
                Quantity = 25,
                Code = "MON-27-4K",
                IsActive = true 
            },
        };

        await _context.Items.AddRangeAsync(items);
        _logger.LogInformation("Dodano {Count} produktów", items.Count);
    }
}
```

### 3.2. Rejestracja i wywołanie w Program.cs

**Program.cs:**
```csharp
using SolutionOrdersReact.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// ... inne serwisy ...

// Rejestracja seedera
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

var app = builder.Build();

// Automatyczne seedowanie przy starcie (tylko Development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

// ... reszta konfiguracji ...

app.Run();
```

---

## CZĘŚĆ 4: Endpoint do ręcznego seedowania (Opcjonalny)

**Controllers/SeedController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Services;

namespace SolutionOrdersReact.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IDataSeeder _seeder;
    private readonly IWebHostEnvironment _env;

    public SeedController(IDataSeeder seeder, IWebHostEnvironment env)
    {
        _seeder = seeder;
        _env = env;
    }

    /// <summary>
    /// Wypełnia bazę danymi początkowymi (tylko Development!)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        // Zabezpieczenie - tylko w Development
        if (!_env.IsDevelopment())
        {
            return Forbid("Seedowanie dozwolone tylko w trybie Development");
        }

        await _seeder.SeedAsync();
        return Ok("Seedowanie zakończone");
    }
}
```

**Użycie:**
```bash
# Wywołanie przez curl/Postman
POST http://localhost:5000/api/seed
```

---

## CZĘŚĆ 5: Weryfikacja seederów

### 5.1. Po uruchomieniu sprawdź w Swagger

1. Otwórz `http://localhost:5000/swagger`
2. Wywołaj `GET /api/Category`
3. Powinny być widoczne kategorie: Elektronika, AGD, Meble, itp.

### 5.2. Sprawdź w bazie danych

```sql
-- SQL Server Management Studio lub Azure Data Studio
SELECT * FROM Categories;
SELECT * FROM UnitOfMeasurements;
SELECT * FROM Workers;
SELECT * FROM Items;
```

---

## CZĘŚĆ 6: Dobre praktyki

### ✅ DO:
- Seeduj dane **wymagane** do działania aplikacji (kategorie, jednostki)
- Używaj **stałych ID** w HasData dla spójności migracji
- Seeduj **testowego użytkownika** do logowania
- Dodaj **kilka przykładowych produktów** do demonstracji

### ❌ DON'T:
- Nie seeduj **wrażliwych danych** (prawdziwe hasła, dane osobowe)
- Nie seeduj **dużych ilości** danych (to nie jest import)
- Nie seeduj w **produkcji** bez kontroli
- Nie używaj `Random` w HasData (musi być deterministyczne)

---

## CZĘŚĆ 7: Checklist seederów

- [ ] Kategorie produktów (min. 3-5)
- [ ] Jednostki miary (szt., kg, l, m, opak.)
- [ ] Testowy pracownik (login: admin, hasło: admin)
- [ ] Przykładowy klient (opcjonalnie)
- [ ] 3-5 przykładowych produktów
- [ ] Migracja z seedami utworzona
- [ ] Test: po `dotnet ef database update` dane są w bazie

---

**Gotowe! Twój projekt ma teraz seedery i będzie działał od razu po rozpakowaniu. 🌱**
