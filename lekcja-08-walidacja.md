# Lekcja 8: Walidacja – FluentValidation + Pipeline Behaviors

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

**Moduł:** Walidacja danych, obsługa błędów  
**Czas trwania:** 2,5 godziny  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Konfigurować FluentValidation w projekcie .NET 8
- ✅ Tworzyć walidatory dla Commands
- ✅ Implementować Pipeline Behaviors w MediatR
- ✅ Budować globalny middleware do obsługi błędów
- ✅ Tworzyć niestandardowe wyjątki
- ✅ Wyświetlać błędy walidacji w React Native

---

## CZĘŚĆ 1: Teoria Walidacji (20 minut)

### 1.1. Gdzie walidować dane?

**SCRIPT dla prowadzącego:**

> „Walidacja może odbywać się na różnych poziomach aplikacji. Kluczowe pytanie: gdzie? Odpowiedź: WSZĘDZIE! Ale z różnym zakresem."

**Poziomy walidacji:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        WARSTWY WALIDACJI                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │ 1. FRONTEND (React Native)                                          │   │
│   │    ✓ Podstawowa walidacja formularzy                                │   │
│   │    ✓ Natychmiastowy feedback dla użytkownika                        │   │
│   │    ✗ NIE WYSTARCZY! Można ominąć (np. Postman)                     │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                         │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │ 2. CONTROLLER (ASP.NET)                                             │   │
│   │    ✓ [Required], [StringLength] - atrybuty                         │   │
│   │    ✓ ModelState.IsValid                                            │   │
│   │    ✗ Podstawowe, nie obsługuje złożonych reguł                     │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                         │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │ 3. PIPELINE BEHAVIOR (MediatR) ★ NASZA GŁÓWNA WARSTWA ★            │   │
│   │    ✓ FluentValidation                                               │   │
│   │    ✓ Złożone reguły biznesowe                                       │   │
│   │    ✓ Walidacja przed Handlerem                                      │   │
│   │    ✓ Automatyczne dla wszystkich Commands                          │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                         │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │ 4. HANDLER (Business Logic)                                         │   │
│   │    ✓ Walidacja wymagająca dostępu do bazy danych                   │   │
│   │    ✓ Np. "Czy klient o takim ID istnieje?"                         │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                         │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │ 5. DATABASE (SQL Server)                                            │   │
│   │    ✓ Constraints (FK, UNIQUE, CHECK)                               │   │
│   │    ✓ Ostatnia linia obrony                                         │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Dlaczego FluentValidation?

**Porównanie z DataAnnotations:**

| Cecha | DataAnnotations | FluentValidation |
|-------|-----------------|------------------|
| Czytelność | Atrybuty na properties | Fluent API |
| Testowalność | Trudna | Łatwa (unit testy) |
| Złożone reguły | Ograniczone | Pełna elastyczność |
| Warunki | `IValidatableObject` | `When()`, `Unless()` |
| Powiązane pola | Trudne | Proste |
| Wiadomości | Statyczne | Dynamiczne, lokalizowalne |

**Przykład porównawczy:**

```csharp
// DataAnnotations - ograniczone
public class CreateItemCommand
{
    [Required(ErrorMessage = "Nazwa jest wymagana")]
    [StringLength(200, MinimumLength = 2)]
    public string? Name { get; set; }

    [Range(0.01, 999999.99)]
    public decimal? Price { get; set; }
}

// FluentValidation - elastyczne
public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana")
            .Length(2, 200).WithMessage("Nazwa musi mieć od 2 do 200 znaków");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Cena jest wymagana")
            .GreaterThan(0).WithMessage("Cena musi być większa od 0")
            .LessThan(1000000).WithMessage("Cena nie może przekraczać 1 000 000");

        // Zaawansowana reguła: jeśli jest promocja, cena musi być niższa
        When(x => x.IsPromotion, () =>
        {
            RuleFor(x => x.Price)
                .LessThan(x => x.RegularPrice)
                .WithMessage("Cena promocyjna musi być niższa od regularnej");
        });
    }
}
```

### 1.3. Pipeline Behaviors w MediatR

**SCRIPT dla prowadzącego:**

> „Pipeline Behaviors to middleware dla MediatR. Każdy Request przechodzi przez pipeline ZANIM dotrze do Handlera. To idealny moment na walidację, logowanie, cache'owanie."

**Przepływ żądania:**

```
Request (CreateItemCommand)
    │
    ▼
┌──────────────────────────────┐
│ LoggingBehavior              │  ← Loguje wejście
├──────────────────────────────┤
│ ValidationBehavior           │  ← Waliduje Command ★
├──────────────────────────────┤
│ TransactionBehavior          │  ← Zarządza transakcją
├──────────────────────────────┤
│ PerformanceBehavior          │  ← Mierzy czas
└──────────────────────────────┘
    │
    ▼
┌──────────────────────────────┐
│ Handler                      │  ← Właściwa logika
└──────────────────────────────┘
    │
    ▼
Response
```

---

## CZĘŚĆ 2: Instalacja i Konfiguracja (15 minut)

### 2.1. Instalacja pakietów NuGet

**W Package Manager Console:**

```powershell
# FluentValidation dla ASP.NET Core
Install-Package FluentValidation.AspNetCore -Version 11.7.0

# Automatyczne rejestrowanie walidatorów
Install-Package FluentValidation.DependencyInjectionExtensions -Version 11.7.0
```

**Lub przez .NET CLI:**

```bash
dotnet add package FluentValidation.AspNetCore --version 11.7.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.7.0
```

> **⚠️ Uwaga o wersjonowaniu:**
> 
> Pakiet `FluentValidation.AspNetCore` został oznaczony jako **deprecated** (przestarzały) od wersji 11.x.
> Oficjalne zalecenie to używanie **FluentValidation** z **ręczną integracją** przez MediatR Pipeline Behaviors,
> co właśnie robimy w tej lekcji. Pakiet nadal działa i jest wspierany, ale nowe projekty powinny
> preferować integrację przez Behaviors (patrz sekcja 3.1).

### 2.2. Rejestracja w Program.cs

**Program.cs:**

```csharp
using FluentValidation;
using System.Reflection;
using SolutionOrdersReact.Server.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// ... istniejący kod ...

// =========================================
// FLUENT VALIDATION
// =========================================

// Automatyczne rejestrowanie wszystkich walidatorów z assembly
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Wyłączenie automatycznej walidacji ASP.NET (używamy FluentValidation w Pipeline)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// =========================================
// MEDIATR + PIPELINE BEHAVIORS
// =========================================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

    // WAŻNA KOLEJNOŚĆ: Behaviors wykonywane od góry do dołu
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

var app = builder.Build();

// =========================================
// MIDDLEWARE OBSŁUGI BŁĘDÓW (musi być przed UseRouting!)
// =========================================
app.UseMiddleware<ExceptionMiddleware>();

// ... reszta konfiguracji ...
```

---

## CZĘŚĆ 3: Walidatory FluentValidation (40 minut)

### 3.1. Walidator dla CreateItemCommand

**Features/Items/Commands/CreateItem/CreateItemValidator.cs:**

```csharp
using FluentValidation;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    public class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator()
        {
            // =========================================
            // NAZWA - wymagana, długość 2-200
            // =========================================
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Nazwa produktu jest wymagana")
                    .WithErrorCode("ITEM_NAME_REQUIRED")
                .MinimumLength(2)
                    .WithMessage("Nazwa musi mieć minimum 2 znaki")
                    .WithErrorCode("ITEM_NAME_TOO_SHORT")
                .MaximumLength(200)
                    .WithMessage("Nazwa może mieć maksymalnie 200 znaków")
                    .WithErrorCode("ITEM_NAME_TOO_LONG");

            // =========================================
            // KOD PRODUKTU - opcjonalny, ale jeśli podany to max 50 znaków
            // =========================================
            RuleFor(x => x.Code)
                .MaximumLength(50)
                    .WithMessage("Kod produktu może mieć maksymalnie 50 znaków")
                .When(x => !string.IsNullOrEmpty(x.Code));

            // =========================================
            // CENA - wymagana, > 0, < 10 milionów
            // =========================================
            RuleFor(x => x.Price)
                .NotNull()
                    .WithMessage("Cena jest wymagana")
                    .WithErrorCode("ITEM_PRICE_REQUIRED")
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Cena nie może być ujemna")
                    .WithErrorCode("ITEM_PRICE_NEGATIVE")
                .LessThan(10_000_000)
                    .WithMessage("Cena nie może przekraczać 10 000 000")
                    .WithErrorCode("ITEM_PRICE_TOO_HIGH");

            // =========================================
            // ILOŚĆ - wymagana, >= 0
            // =========================================
            RuleFor(x => x.Quantity)
                .NotNull()
                    .WithMessage("Ilość jest wymagana")
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Ilość nie może być ujemna");

            // =========================================
            // KATEGORIA - wymagana, > 0
            // =========================================
            RuleFor(x => x.IdCategory)
                .GreaterThan(0)
                    .WithMessage("Wybierz kategorię produktu")
                    .WithErrorCode("ITEM_CATEGORY_REQUIRED");

            // =========================================
            // JEDNOSTKA - opcjonalna, ale jeśli podana to > 0
            // =========================================
            RuleFor(x => x.IdUnitOfMeasurement)
                .GreaterThan(0)
                    .WithMessage("Nieprawidłowy ID jednostki miary")
                .When(x => x.IdUnitOfMeasurement.HasValue);

            // =========================================
            // OPIS - opcjonalny, max 2000 znaków
            // =========================================
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                    .WithMessage("Opis może mieć maksymalnie 2000 znaków")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
```

### 3.2. Walidator dla UpdateItemCommand

**Features/Items/Commands/UpdateItem/UpdateItemValidator.cs:**

```csharp
using FluentValidation;

namespace SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            // ID musi być > 0
            RuleFor(x => x.IdItem)
                .GreaterThan(0)
                    .WithMessage("Nieprawidłowy ID produktu");

            // Reguły jak w CreateItemValidator
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Nazwa produktu jest wymagana")
                .MinimumLength(2)
                    .WithMessage("Nazwa musi mieć minimum 2 znaki")
                .MaximumLength(200)
                    .WithMessage("Nazwa może mieć maksymalnie 200 znaków");

            RuleFor(x => x.Price)
                .NotNull()
                    .WithMessage("Cena jest wymagana")
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Cena nie może być ujemna");

            RuleFor(x => x.Quantity)
                .NotNull()
                    .WithMessage("Ilość jest wymagana")
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Ilość nie może być ujemna");

            RuleFor(x => x.IdCategory)
                .GreaterThan(0)
                    .WithMessage("Wybierz kategorię produktu");
        }
    }
}
```

### 3.3. Walidator dla CreateOrderCommand

**Features/Orders/Commands/CreateOrder/CreateOrderValidator.cs:**

```csharp
using FluentValidation;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            // =========================================
            // KLIENT - opcjonalny, ale jeśli podany to > 0
            // =========================================
            RuleFor(x => x.IdClient)
                .GreaterThan(0)
                    .WithMessage("Nieprawidłowy ID klienta")
                .When(x => x.IdClient.HasValue);

            // =========================================
            // PRACOWNIK - opcjonalny, ale jeśli podany to > 0
            // =========================================
            RuleFor(x => x.IdWorker)
                .GreaterThan(0)
                    .WithMessage("Nieprawidłowy ID pracownika")
                .When(x => x.IdWorker.HasValue);

            // =========================================
            // DATA DOSTAWY - jeśli podana to w przyszłości
            // =========================================
            RuleFor(x => x.DeliveryDate)
                .GreaterThan(DateTime.Now.Date)
                    .WithMessage("Data dostawy musi być w przyszłości")
                .When(x => x.DeliveryDate.HasValue);

            // =========================================
            // NOTATKI - max 1000 znaków
            // =========================================
            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                    .WithMessage("Notatki mogą mieć maksymalnie 1000 znaków")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            // =========================================
            // POZYCJE - musi być przynajmniej jedna
            // =========================================
            RuleFor(x => x.Items)
                .NotNull()
                    .WithMessage("Lista pozycji nie może być null")
                .NotEmpty()
                    .WithMessage("Zamówienie musi zawierać przynajmniej jedną pozycję");

            // =========================================
            // WALIDACJA KAŻDEJ POZYCJI
            // =========================================
            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }

    /// <summary>
    /// Walidator dla pojedynczej pozycji zamówienia
    /// </summary>
    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.IdItem)
                .GreaterThan(0)
                    .WithMessage("Wybierz produkt");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                    .WithMessage("Ilość musi być większa od 0")
                .LessThanOrEqualTo(10000)
                    .WithMessage("Ilość nie może przekraczać 10 000");
        }
    }
}
```

### 3.4. Walidator z dostępem do bazy danych

**Zaawansowany przykład - walidacja unikalności:**

**Features/Items/Commands/CreateItem/CreateItemValidatorWithDb.cs:**

```csharp
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    /// <summary>
    /// Walidator z dostępem do bazy danych
    /// Sprawdza unikalność nazwy i kodu produktu
    /// </summary>
    public class CreateItemValidatorWithDb : AbstractValidator<CreateItemCommand>
    {
        private readonly ApplicationDbContext _context;

        public CreateItemValidatorWithDb(ApplicationDbContext context)
        {
            _context = context;

            // Podstawowe reguły
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Nazwa jest wymagana")
                .MustAsync(BeUniqueName)
                    .WithMessage("Produkt o takiej nazwie już istnieje");

            RuleFor(x => x.Code)
                .MustAsync(BeUniqueCode)
                    .WithMessage("Produkt o takim kodzie już istnieje")
                .When(x => !string.IsNullOrEmpty(x.Code));

            RuleFor(x => x.IdCategory)
                .MustAsync(CategoryExists)
                    .WithMessage("Wybrana kategoria nie istnieje");

            RuleFor(x => x.IdUnitOfMeasurement)
                .MustAsync(UnitExists)
                    .WithMessage("Wybrana jednostka miary nie istnieje")
                .When(x => x.IdUnitOfMeasurement.HasValue);
        }

        private async Task<bool> BeUniqueName(
            string? name,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name)) return true;

            return !await _context.Items
                .AnyAsync(i => i.Name == name && i.IsActive, cancellationToken);
        }

        private async Task<bool> BeUniqueCode(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(code)) return true;

            return !await _context.Items
                .AnyAsync(i => i.Code == code && i.IsActive, cancellationToken);
        }

        private async Task<bool> CategoryExists(
            int categoryId,
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AnyAsync(c => c.IdCategory == categoryId && c.IsActive, cancellationToken);
        }

        private async Task<bool> UnitExists(
            int? unitId,
            CancellationToken cancellationToken)
        {
            if (!unitId.HasValue) return true;

            return await _context.UnitsOfMeasurement
                .AnyAsync(u => u.IdUnitOfMeasurement == unitId.Value && u.IsActive, cancellationToken);
        }
    }
}
```

---

## CZĘŚĆ 4: Pipeline Behaviors (30 minut)

### 4.1. Niestandardowy wyjątek ValidationException

**Exceptions/ValidationException.cs:**

```csharp
namespace SolutionOrdersReact.Server.Exceptions
{
    /// <summary>
    /// Wyjątek rzucany przy błędach walidacji
    /// </summary>
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("Wystąpiły błędy walidacji")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToArray()
                );
        }

        public ValidationException(string propertyName, string errorMessage)
            : this()
        {
            Errors = new Dictionary<string, string[]>
            {
                { propertyName, new[] { errorMessage } }
            };
        }
    }
}
```

### 4.2. NotFoundException

**Exceptions/NotFoundException.cs:**

```csharp
namespace SolutionOrdersReact.Server.Exceptions
{
    /// <summary>
    /// Wyjątek gdy zasób nie został znaleziony
    /// </summary>
    public class NotFoundException : Exception
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public NotFoundException(string entityName, object entityId)
            : base($"{entityName} o ID {entityId} nie został znaleziony")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
```

### 4.3. ValidationBehavior

**Behaviors/ValidationBehavior.cs:**

```csharp
using FluentValidation;
using MediatR;
using ValidationException = SolutionOrdersReact.Server.Exceptions.ValidationException;

namespace SolutionOrdersReact.Server.Behaviors
{
    /// <summary>
    /// Pipeline Behavior wykonujący walidację przed Handlerem
    /// </summary>
    /// <typeparam name="TRequest">Typ żądania (Command/Query)</typeparam>
    /// <typeparam name="TResponse">Typ odpowiedzi</typeparam>
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Jeśli nie ma walidatorów - przejdź dalej
            if (!_validators.Any())
            {
                return await next();
            }

            var requestName = typeof(TRequest).Name;
            _logger.LogDebug("Walidacja {RequestName}", requestName);

            // Utwórz kontekst walidacji
            var context = new ValidationContext<TRequest>(request);

            // Wykonaj wszystkie walidatory
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            // Zbierz błędy
            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            // Jeśli są błędy - rzuć wyjątek
            if (failures.Any())
            {
                _logger.LogWarning(
                    "Walidacja {RequestName} nie powiodła się. Błędy: {@Errors}",
                    requestName,
                    failures.Select(f => new { f.PropertyName, f.ErrorMessage })
                );

                throw new ValidationException(failures);
            }

            _logger.LogDebug("Walidacja {RequestName} zakończona sukcesem", requestName);

            // Walidacja OK - przejdź do Handlera
            return await next();
        }
    }
}
```

### 4.4. LoggingBehavior

**Behaviors/LoggingBehavior.cs:**

```csharp
using MediatR;
using System.Diagnostics;
using System.Text.Json;

namespace SolutionOrdersReact.Server.Behaviors
{
    /// <summary>
    /// Pipeline Behavior logujący żądania i odpowiedzi
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid().ToString("N")[..8];

            // Logowanie wejścia
            _logger.LogInformation(
                "[{RequestId}] ➡️ START {RequestName}",
                requestId, requestName);

            // W trybie Debug - loguj pełny obiekt
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                try
                {
                    var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        MaxDepth = 3
                    });
                    _logger.LogDebug("[{RequestId}] Request: {Request}", requestId, requestJson);
                }
                catch
                {
                    _logger.LogDebug("[{RequestId}] Request: (nie można zserializować)", requestId);
                }
            }

            // Zmierz czas wykonania
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                // Logowanie sukcesu
                _logger.LogInformation(
                    "[{RequestId}] ✅ END {RequestName} ({ElapsedMs}ms)",
                    requestId, requestName, stopwatch.ElapsedMilliseconds);

                // Ostrzeżenie jeśli za długo
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    _logger.LogWarning(
                        "[{RequestId}] ⚠️ SLOW {RequestName} ({ElapsedMs}ms)",
                        requestId, requestName, stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Logowanie błędu
                _logger.LogError(
                    ex,
                    "[{RequestId}] ❌ FAIL {RequestName} ({ElapsedMs}ms) - {ErrorMessage}",
                    requestId, requestName, stopwatch.ElapsedMilliseconds, ex.Message);

                throw;
            }
        }
    }
}
```

### 4.5. PerformanceBehavior (opcjonalny)

**Behaviors/PerformanceBehavior.cs:**

```csharp
using MediatR;
using System.Diagnostics;

namespace SolutionOrdersReact.Server.Behaviors
{
    /// <summary>
    /// Pipeline Behavior monitorujący wydajność
    /// Loguje ostrzeżenie gdy żądanie trwa > threshold
    /// </summary>
    public class PerformanceBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly int _thresholdMs;

        public PerformanceBehavior(
            ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _thresholdMs = configuration.GetValue("Performance:ThresholdMs", 500);
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > _thresholdMs)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "🐢 Long Running Request: {Name} ({ElapsedMilliseconds}ms)",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}
```

---

## CZĘŚĆ 5: Globalna Obsługa Błędów (25 minut)

### 5.1. ExceptionMiddleware

**Middleware/ExceptionMiddleware.cs:**

```csharp
using System.Net;
using System.Text.Json;
using SolutionOrdersReact.Server.Exceptions;

namespace SolutionOrdersReact.Server.Middleware
{
    /// <summary>
    /// Middleware do globalnej obsługi wyjątków
    /// Zamienia wyjątki na odpowiednie kody HTTP i JSON
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Nieobsłużony wyjątek: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                // Błędy walidacji - 400 Bad Request
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Type = "ValidationError";
                    response.Title = "Błędy walidacji";
                    response.Status = 400;
                    response.Errors = validationEx.Errors;
                    break;

                // Nie znaleziono - 404 Not Found
                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Type = "NotFound";
                    response.Title = notFoundEx.Message;
                    response.Status = 404;
                    break;

                // Nieprawidłowy argument - 400 Bad Request
                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Type = "BadRequest";
                    response.Title = argEx.Message;
                    response.Status = 400;
                    break;

                // Brak autoryzacji - 401 Unauthorized
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Type = "Unauthorized";
                    response.Title = "Brak autoryzacji";
                    response.Status = 401;
                    break;

                // Operacja niedozwolona - 403 Forbidden
                case InvalidOperationException invalidOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Type = "Forbidden";
                    response.Title = invalidOpEx.Message;
                    response.Status = 403;
                    break;

                // Wszystko inne - 500 Internal Server Error
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Type = "InternalError";
                    response.Title = _environment.IsDevelopment()
                        ? exception.Message
                        : "Wystąpił błąd serwera";
                    response.Status = 500;

                    // W development dodaj stack trace
                    if (_environment.IsDevelopment())
                    {
                        response.Detail = exception.StackTrace;
                    }
                    break;
            }

            // Dodaj TraceId dla śledzenia
            response.TraceId = context.TraceIdentifier;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            });

            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Standardowa struktura odpowiedzi błędu (RFC 7807)
    /// </summary>
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Detail { get; set; }
        public string? TraceId { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
```

### 5.2. Extension Method dla rejestracji

**Extensions/MiddlewareExtensions.cs:**

```csharp
using SolutionOrdersReact.Server.Middleware;

namespace SolutionOrdersReact.Server.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
```

**Użycie w Program.cs:**

```csharp
var app = builder.Build();

// Globalna obsługa błędów - MUSI być przed innymi middleware
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## CZĘŚĆ 6: React Native - Obsługa Błędów (30 minut)

### 6.1. Typy błędów

**src/types/errors.ts:**

```typescript
/**
 * Standardowa struktura błędu z API (RFC 7807)
 */
export interface ApiError {
  type: string;
  title: string;
  status: number;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

/**
 * Błąd walidacji z listą błędów per pole
 */
export interface ValidationError extends ApiError {
  type: 'ValidationError';
  errors: Record<string, string[]>;
}

/**
 * Helper do sprawdzania typu błędu
 */
export function isValidationError(error: ApiError): error is ValidationError {
  return error.type === 'ValidationError' && error.errors !== undefined;
}

/**
 * Helper do sprawdzania czy to ApiError
 */
export function isApiError(error: unknown): error is ApiError {
  return (
    typeof error === 'object' &&
    error !== null &&
    'type' in error &&
    'title' in error &&
    'status' in error
  );
}
```

### 6.2. Zaktualizowany ApiService

**src/api/apiService.ts:**

```typescript
import { API_BASE_URL } from './config';
import type { ApiError } from '../types/errors';

class ApiService {
  private baseUrl: string;

  constructor() {
    this.baseUrl = API_BASE_URL;
  }

  /**
   * Generyczna metoda do wykonywania żądań HTTP
   */
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(url, config);

      // Parsuj odpowiedź jako JSON
      const data = await response.json().catch(() => null);

      // Sprawdź czy odpowiedź jest OK
      if (!response.ok) {
        // Jeśli serwer zwrócił błąd w formacie RFC 7807
        if (data && data.type && data.title) {
          throw data as ApiError;
        }

        // Fallback dla innych błędów
        throw {
          type: 'Error',
          title: data?.message || `HTTP Error ${response.status}`,
          status: response.status,
        } as ApiError;
      }

      return data as T;
    } catch (error) {
      // Błąd sieci (brak połączenia)
      if (error instanceof TypeError && error.message === 'Network request failed') {
        throw {
          type: 'NetworkError',
          title: 'Brak połączenia z serwerem',
          status: 0,
          detail: 'Sprawdź połączenie internetowe i spróbuj ponownie',
        } as ApiError;
      }

      // Przekaż dalej jeśli to już ApiError
      if (typeof error === 'object' && error !== null && 'type' in error) {
        throw error;
      }

      // Inny nieznany błąd
      throw {
        type: 'UnknownError',
        title: error instanceof Error ? error.message : 'Nieznany błąd',
        status: 500,
      } as ApiError;
    }
  }

  // ... reszta metod ...
}

export default new ApiService();
```

### 6.3. Hook useFormErrors

**src/hooks/useFormErrors.ts:**

```typescript
import { useState, useCallback } from 'react';
import type { ApiError, ValidationError } from '../types/errors';
import { isValidationError } from '../types/errors';

interface FormErrors {
  [key: string]: string | undefined;
}

interface UseFormErrorsResult {
  errors: FormErrors;
  generalError: string | null;
  setFieldError: (field: string, message: string) => void;
  clearFieldError: (field: string) => void;
  clearAllErrors: () => void;
  handleApiError: (error: ApiError) => void;
  hasErrors: boolean;
}

/**
 * Hook do zarządzania błędami formularza
 */
export function useFormErrors(): UseFormErrorsResult {
  const [errors, setErrors] = useState<FormErrors>({});
  const [generalError, setGeneralError] = useState<string | null>(null);

  const setFieldError = useCallback((field: string, message: string) => {
    setErrors(prev => ({ ...prev, [field]: message }));
  }, []);

  const clearFieldError = useCallback((field: string) => {
    setErrors(prev => {
      const { [field]: _, ...rest } = prev;
      return rest;
    });
  }, []);

  const clearAllErrors = useCallback(() => {
    setErrors({});
    setGeneralError(null);
  }, []);

  const handleApiError = useCallback((error: ApiError) => {
    // Błąd walidacji - mapuj błędy na pola
    if (isValidationError(error)) {
      const fieldErrors: FormErrors = {};

      Object.entries(error.errors).forEach(([field, messages]) => {
        // Konwertuj nazwę pola na camelCase (C# używa PascalCase)
        const fieldName = field.charAt(0).toLowerCase() + field.slice(1);
        fieldErrors[fieldName] = messages[0]; // Weź pierwszy błąd
      });

      setErrors(fieldErrors);
      setGeneralError(null);
    } else {
      // Inny błąd - ustaw jako generalError
      setErrors({});
      setGeneralError(error.title);
    }
  }, []);

  const hasErrors = Object.keys(errors).length > 0 || generalError !== null;

  return {
    errors,
    generalError,
    setFieldError,
    clearFieldError,
    clearAllErrors,
    handleApiError,
    hasErrors,
  };
}
```

### 6.4. Komponent FormField z obsługą błędów

**src/components/FormField.tsx:**

```tsx
import React from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TextInputProps,
} from 'react-native';

interface FormFieldProps extends TextInputProps {
  label: string;
  error?: string;
  required?: boolean;
}

export const FormField: React.FC<FormFieldProps> = ({
  label,
  error,
  required,
  style,
  ...inputProps
}) => {
  return (
    <View style={styles.container}>
      <View style={styles.labelContainer}>
        <Text style={styles.label}>{label}</Text>
        {required && <Text style={styles.required}>*</Text>}
      </View>

      <TextInput
        style={[
          styles.input,
          error && styles.inputError,
          style,
        ]}
        placeholderTextColor="#999"
        {...inputProps}
      />

      {error && (
        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>⚠️ {error}</Text>
        </View>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: 16,
  },
  labelContainer: {
    flexDirection: 'row',
    marginBottom: 6,
  },
  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  required: {
    color: '#E53935',
    marginLeft: 4,
    fontWeight: 'bold',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fff',
    color: '#333',
  },
  inputError: {
    borderColor: '#E53935',
    borderWidth: 2,
    backgroundColor: '#FFEBEE',
  },
  errorContainer: {
    marginTop: 4,
    paddingHorizontal: 4,
  },
  errorText: {
    color: '#E53935',
    fontSize: 12,
    fontWeight: '500',
  },
});
```

### 6.5. Komponent ErrorBanner

**src/components/ErrorBanner.tsx:**

```tsx
import React from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  Animated,
} from 'react-native';

interface ErrorBannerProps {
  message: string;
  onDismiss?: () => void;
  type?: 'error' | 'warning' | 'info';
}

export const ErrorBanner: React.FC<ErrorBannerProps> = ({
  message,
  onDismiss,
  type = 'error',
}) => {
  const colors = {
    error: { bg: '#FFEBEE', border: '#E53935', text: '#C62828', icon: '❌' },
    warning: { bg: '#FFF3E0', border: '#FF9800', text: '#E65100', icon: '⚠️' },
    info: { bg: '#E3F2FD', border: '#2196F3', text: '#1565C0', icon: 'ℹ️' },
  };

  const color = colors[type];

  return (
    <View style={[styles.container, { backgroundColor: color.bg, borderColor: color.border }]}>
      <Text style={styles.icon}>{color.icon}</Text>
      <Text style={[styles.message, { color: color.text }]}>{message}</Text>
      {onDismiss && (
        <TouchableOpacity onPress={onDismiss} style={styles.dismissButton}>
          <Text style={styles.dismissText}>✕</Text>
        </TouchableOpacity>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 12,
    borderWidth: 1,
    borderRadius: 8,
    marginBottom: 16,
  },
  icon: {
    fontSize: 18,
    marginRight: 10,
  },
  message: {
    flex: 1,
    fontSize: 14,
    fontWeight: '500',
  },
  dismissButton: {
    padding: 4,
    marginLeft: 8,
  },
  dismissText: {
    fontSize: 18,
    color: '#666',
  },
});
```

### 6.6. Przykład formularza z obsługą błędów

**src/screens/CreateItemScreen.tsx:**

```tsx
import React, { useState, useEffect } from 'react';
import {
  View,
  ScrollView,
  TouchableOpacity,
  Text,
  StyleSheet,
  Alert,
  ActivityIndicator,
} from 'react-native';
import { FormField } from '../components/FormField';
import { ErrorBanner } from '../components/ErrorBanner';
import { PickerField } from '../components/PickerField';
import { useFormErrors } from '../hooks/useFormErrors';
import apiService from '../api/apiService';
import type { Category, UnitOfMeasurement, CreateItemRequest } from '../types/models';
import type { ApiError } from '../types/errors';

interface Props {
  navigation: any;
}

const CreateItemScreen: React.FC<Props> = ({ navigation }) => {
  // Form state
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [quantity, setQuantity] = useState('');
  const [idCategory, setIdCategory] = useState<number | null>(null);
  const [idUnit, setIdUnit] = useState<number | null>(null);

  // Lookups
  const [categories, setCategories] = useState<Category[]>([]);
  const [units, setUnits] = useState<UnitOfMeasurement[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // Errors
  const {
    errors,
    generalError,
    clearFieldError,
    clearAllErrors,
    handleApiError,
  } = useFormErrors();

  useEffect(() => {
    loadLookups();
  }, []);

  const loadLookups = async () => {
    try {
      const [cats, units] = await Promise.all([
        apiService.getCategories(),
        apiService.getUnitOfMeasurements(),
      ]);
      setCategories(cats);
      setUnits(units);
    } catch (error) {
      Alert.alert('Błąd', 'Nie udało się załadować danych');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    clearAllErrors();

    // Walidacja frontend (szybki feedback)
    if (!name.trim()) {
      handleApiError({
        type: 'ValidationError',
        title: 'Błędy walidacji',
        status: 400,
        errors: { name: ['Nazwa jest wymagana'] },
      });
      return;
    }

    if (!idCategory) {
      handleApiError({
        type: 'ValidationError',
        title: 'Błędy walidacji',
        status: 400,
        errors: { idCategory: ['Wybierz kategorię'] },
      });
      return;
    }

    try {
      setSubmitting(true);

      const data: CreateItemRequest = {
        name: name.trim(),
        code: code.trim() || undefined,
        description: description.trim() || undefined,
        price: parseFloat(price) || 0,
        quantity: parseFloat(quantity) || 0,
        idCategory: idCategory,
        idUnitOfMeasurement: idUnit || undefined,
      };

      await apiService.createItem(data);

      Alert.alert('Sukces', 'Produkt został utworzony', [
        { text: 'OK', onPress: () => navigation.goBack() }
      ]);
    } catch (error) {
      handleApiError(error as ApiError);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <View style={styles.form}>
        <Text style={styles.title}>Nowy produkt</Text>

        {/* General Error Banner */}
        {generalError && (
          <ErrorBanner
            message={generalError}
            onDismiss={clearAllErrors}
          />
        )}

        {/* Nazwa */}
        <FormField
          label="Nazwa produktu"
          required
          value={name}
          onChangeText={(text) => {
            setName(text);
            clearFieldError('name');
          }}
          placeholder="Wpisz nazwę..."
          error={errors.name}
          editable={!submitting}
        />

        {/* Kod */}
        <FormField
          label="Kod produktu"
          value={code}
          onChangeText={(text) => {
            setCode(text);
            clearFieldError('code');
          }}
          placeholder="np. SKU-001"
          error={errors.code}
          editable={!submitting}
        />

        {/* Kategoria */}
        <PickerField
          label="Kategoria"
          required
          value={idCategory}
          items={categories}
          getValue={(c) => c.idCategory}
          getLabel={(c) => c.name || 'Brak nazwy'}
          onChange={(value) => {
            setIdCategory(value as number | null);
            clearFieldError('idCategory');
          }}
          error={errors.idCategory}
          disabled={submitting}
        />

        {/* Jednostka */}
        <PickerField
          label="Jednostka miary"
          value={idUnit}
          items={units}
          getValue={(u) => u.idUnitOfMeasurement}
          getLabel={(u) => u.name || 'Brak nazwy'}
          onChange={(value) => setIdUnit(value as number | null)}
          placeholder="Wybierz jednostkę (opcjonalne)"
          disabled={submitting}
        />

        {/* Cena */}
        <FormField
          label="Cena"
          required
          value={price}
          onChangeText={(text) => {
            setPrice(text);
            clearFieldError('price');
          }}
          placeholder="0.00"
          keyboardType="decimal-pad"
          error={errors.price}
          editable={!submitting}
        />

        {/* Ilość */}
        <FormField
          label="Ilość na stanie"
          required
          value={quantity}
          onChangeText={(text) => {
            setQuantity(text);
            clearFieldError('quantity');
          }}
          placeholder="0"
          keyboardType="decimal-pad"
          error={errors.quantity}
          editable={!submitting}
        />

        {/* Opis */}
        <FormField
          label="Opis"
          value={description}
          onChangeText={setDescription}
          placeholder="Opcjonalny opis produktu..."
          multiline
          numberOfLines={3}
          style={styles.multilineInput}
          editable={!submitting}
        />

        {/* Przyciski */}
        <View style={styles.buttons}>
          <TouchableOpacity
            style={[styles.button, styles.cancelButton]}
            onPress={() => navigation.goBack()}
            disabled={submitting}
          >
            <Text style={styles.cancelButtonText}>Anuluj</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[
              styles.button,
              styles.submitButton,
              submitting && styles.disabledButton
            ]}
            onPress={handleSubmit}
            disabled={submitting}
          >
            {submitting ? (
              <ActivityIndicator color="#fff" size="small" />
            ) : (
              <Text style={styles.submitButtonText}>Zapisz</Text>
            )}
          </TouchableOpacity>
        </View>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  centerContainer: { flex: 1, justifyContent: 'center', alignItems: 'center' },
  form: { padding: 16 },
  title: { fontSize: 24, fontWeight: 'bold', marginBottom: 20, color: '#333' },
  multilineInput: { height: 80, textAlignVertical: 'top' },
  buttons: { flexDirection: 'row', marginTop: 24, marginBottom: 40 },
  button: { flex: 1, padding: 16, borderRadius: 8, alignItems: 'center' },
  cancelButton: { backgroundColor: '#f0f0f0', marginRight: 8 },
  cancelButtonText: { color: '#666', fontWeight: '600', fontSize: 16 },
  submitButton: { backgroundColor: '#007AFF', marginLeft: 8 },
  submitButtonText: { color: '#fff', fontWeight: '600', fontSize: 16 },
  disabledButton: { opacity: 0.5 },
});

export default CreateItemScreen;
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Walidator dla Client
Stwórz `CreateClientValidator` z regułami:
- `Name` - wymagane, 2-100 znaków
- `PhoneNumber` - opcjonalne, format +48 XXX XXX XXX
- `Email` - opcjonalne, poprawny format email

### Zadanie 2: Walidator z warunkami
Stwórz walidator dla `CreateWorkerCommand` gdzie:
- Jeśli `IsManager = true`, `Salary` musi być > 5000
- Hasło musi mieć min. 8 znaków, zawierać cyfrę i wielką literę

### Zadanie 3: Toast notifications
Zaimplementuj wyświetlanie błędów jako toast zamiast ErrorBanner, używając biblioteki `react-native-toast-message`.

### Zadanie 4: Retry mechanism
Dodaj automatyczne ponawianie żądań przy błędzie sieci (max 3 próby).

### Zadanie 5: Inline validation
Zaimplementuj walidację "na żywo" - sprawdzanie pola podczas pisania (debounced).

---

## 🔍 Pytania Kontrolne

1. Dlaczego nie wystarczy walidacja tylko na frontendzie?
2. Co to jest Pipeline Behavior i jak działa?
3. Jaka jest różnica między `ValidationException` a `ArgumentException`?
4. Dlaczego używamy RFC 7807 dla odpowiedzi błędów?
5. Kiedy używamy walidatora z dostępem do bazy danych?
6. Jak FluentValidation radzi sobie z walidacją zagnieżdżonych obiektów?

---

## ➡️ Następna Lekcja

**[Lekcja 9: Zaawansowane CQRS – Paginacja, Filtrowanie, Audyt](./lekcja-09-zaawansowane-cqrs.md)**

W następnej lekcji:
- Paginacja wyników
- Zaawansowane filtrowanie i sortowanie
- Audyt zmian (CreatedAt, UpdatedAt, CreatedBy)
- Logowanie do pliku

---

**Gratulacje! 🎉 Twoja aplikacja teraz profesjonalnie obsługuje błędy!**
