# Lekcja 7: Zamówienia – Relacje Master-Detail (Order → OrderItem)

**Moduł:** Relacje Master-Detail, logika biznesowa zamówień  
**Czas trwania:** 3 godziny  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Modelować relacje Master-Detail (Order → OrderItem)
- ✅ Konfigurować kaskadowe usuwanie i aktualizacje
- ✅ Tworzyć zamówienia z wieloma pozycjami w jednej transakcji
- ✅ Mapować zagnieżdżone relacje do DTO
- ✅ Budować formularz zamówienia z dynamiczną listą pozycji
- ✅ Implementować DateTimePicker w React Native
- ✅ Obliczać sumy, walidować stany magazynowe

---

## CZĘŚĆ 1: Teoria Relacji Master-Detail (25 minut)

### 1.1. Czym jest Relacja Master-Detail?

**SCRIPT dla prowadzącego:**

> „Master-Detail to wzorzec gdzie jeden rekord główny (Master) zawiera wiele rekordów szczegółowych (Detail). Typowy przykład to faktura z pozycjami. Faktura to Master, pozycje to Detail. Bez faktury pozycje nie mają sensu."

**Przykłady w rzeczywistych aplikacjach:**

| Master | Detail | Relacja |
|--------|--------|---------|
| Zamówienie (Order) | Pozycje zamówienia (OrderItem) | 1:M |
| Faktura | Pozycje faktury | 1:M |
| Koszyk | Produkty w koszyku | 1:M |
| Post na blogu | Komentarze | 1:M |
| Przepis kulinarny | Składniki | 1:M |

**Diagram relacji w naszym projekcie:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        RELACJE ZAMÓWIEŃ                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   ┌─────────────┐              ┌─────────────┐           ┌─────────────┐    │
│   │   Client    │              │   Worker    │           │    Item     │    │
│   ├─────────────┤              ├─────────────┤           ├─────────────┤    │
│   │ IdClient PK │              │ IdWorker PK │           │ IdItem PK   │    │
│   │ Name        │              │ FirstName   │           │ Name        │    │
│   │ Address     │              │ LastName    │           │ Price       │    │
│   │ PhoneNumber │              │ Login       │           │ Quantity    │    │
│   └──────┬──────┘              └──────┬──────┘           └──────┬──────┘    │
│          │                            │                         │           │
│          │ 1:M (opcjonalna)           │ 1:M (opcjonalna)        │           │
│          │                            │                         │           │
│          ↓                            ↓                         │           │
│   ┌─────────────────────────────────────────────────┐           │           │
│   │                    ORDER (MASTER)                │           │           │
│   ├─────────────────────────────────────────────────┤           │           │
│   │  IdOrder (PK)                                   │           │           │
│   │  DataOrder       (data utworzenia)              │           │           │
│   │  IdClient (FK?)  (opcjonalny klient)            │           │           │
│   │  IdWorker (FK?)  (opcjonalny pracownik)         │           │           │
│   │  Notes           (uwagi)                        │           │           │
│   │  DeliveryDate    (data dostawy)                 │           │           │
│   │  IsActive                                       │           │           │
│   └───────────────────────┬─────────────────────────┘           │           │
│                           │                                     │           │
│                           │ 1:M (wymagana)                      │           │
│                           │ CASCADE DELETE                      │           │
│                           ↓                                     │           │
│   ┌─────────────────────────────────────────────────┐           │           │
│   │               ORDER_ITEM (DETAIL)                │           │           │
│   ├─────────────────────────────────────────────────┤           │           │
│   │  IdOrderItem (PK)                               │           │           │
│   │  IdOrder (FK)     ← klucz do MASTER             │           │           │
│   │  IdItem (FK)      ← klucz do produktu ──────────┼───────────┘           │
│   │  Quantity         (ilość zamówiona)             │                       │
│   │  UnitPrice        (cena jednostkowa w momencie) │                       │
│   │  IsActive                                       │                       │
│   └─────────────────────────────────────────────────┘                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Kaskadowe Zachowania (Cascade Behaviors)

**SCRIPT dla prowadzącego:**

> „Co się dzieje gdy usuwamy zamówienie? Czy pozycje zamówienia powinny też zostać usunięte? W relacji Master-Detail zazwyczaj TAK - to nazywamy CASCADE DELETE."

**Opcje DeleteBehavior w EF Core:**

| Zachowanie | Opis | Kiedy używać |
|------------|------|--------------|
| `Cascade` | Usuń dzieci wraz z rodzicem | Order → OrderItems |
| `Restrict` | Zablokuj usunięcie jeśli ma dzieci | Category → Items |
| `SetNull` | Ustaw FK na NULL | Opcjonalne relacje |
| `NoAction` | Baza decyduje | Rzadko używane |

**W naszym projekcie:**
- Order → OrderItem: **Cascade** (usunięcie zamówienia usuwa pozycje)
- Order → Client: **SetNull** (usunięcie klienta nie usuwa zamówień)
- OrderItem → Item: **Restrict** (nie można usunąć produktu jeśli jest w zamówieniu)

### 1.3. Dlaczego przechowujemy cenę w OrderItem?

**SCRIPT dla prowadzącego:**

> „Zauważcie że w OrderItem mamy pole UnitPrice mimo że Item też ma Price. Dlaczego? Bo ceny się zmieniają! Jeśli klient zamówił laptop za 3000zł, a za miesiąc cena wzrosła do 3500zł, chcemy widzieć ile FAKTYCZNIE zapłacił."

```
Scenariusz:
1. Klient składa zamówienie na Laptop (cena: 3000 zł)
2. OrderItem zapisuje: IdItem=1, UnitPrice=3000
3. Tydzień później: cena Laptopa zmienia się na 3500 zł
4. Patrzymy na zamówienie: UnitPrice nadal 3000 zł ✓

Gdybyśmy pobierali cenę z Item:
1. Klient składa zamówienie
2. OrderItem zapisuje tylko IdItem (bez ceny)
3. Tydzień później: cena zmienia się
4. Patrzymy na zamówienie: pokazuje 3500 zł ✗ BŁĄD!
```

---

## CZĘŚĆ 2: Modele Entity Framework (30 minut)

### 2.1. Model Order.cs

**Models/Order.cs:**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Zamówienie - rekord Master
    /// </summary>
    public class Order
    {
        // ========== PRIMARY KEY ==========
        [Key]
        public int IdOrder { get; set; }

        // ========== DANE PODSTAWOWE ==========
        
        /// <summary>
        /// Data utworzenia zamówienia
        /// </summary>
        public DateTime? DataOrder { get; set; }

        /// <summary>
        /// Notatki/uwagi do zamówienia
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Planowana data dostawy
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Czy zamówienie jest aktywne (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== RELACJA Z CLIENT (1:M, opcjonalna) ==========
        
        /// <summary>
        /// FK do klienta (nullable = zamówienie anonimowe)
        /// </summary>
        public int? IdClient { get; set; }

        /// <summary>
        /// Navigation property do klienta
        /// </summary>
        public virtual Client? Client { get; set; }

        // ========== RELACJA Z WORKER (1:M, opcjonalna) ==========
        
        /// <summary>
        /// FK do pracownika obsługującego
        /// </summary>
        public int? IdWorker { get; set; }

        /// <summary>
        /// Navigation property do pracownika
        /// </summary>
        public virtual Worker? Worker { get; set; }

        // ========== RELACJA Z ORDER_ITEMS (1:M, Master-Detail) ==========
        
        /// <summary>
        /// Kolekcja pozycji zamówienia (Detail)
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // ========== COMPUTED PROPERTIES (nie mapowane do bazy) ==========
        
        /// <summary>
        /// Suma wartości zamówienia
        /// </summary>
        [NotMapped]
        public decimal TotalAmount => OrderItems
            .Where(oi => oi.IsActive)
            .Sum(oi => (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0));

        /// <summary>
        /// Liczba pozycji w zamówieniu
        /// </summary>
        [NotMapped]
        public int ItemCount => OrderItems.Count(oi => oi.IsActive);
    }
}
```

### 2.2. Model OrderItem.cs

**Models/OrderItem.cs:**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Pozycja zamówienia - rekord Detail
    /// </summary>
    public class OrderItem
    {
        // ========== PRIMARY KEY ==========
        [Key]
        public int IdOrderItem { get; set; }

        // ========== RELACJA Z ORDER (wymagana, Master) ==========
        
        /// <summary>
        /// FK do zamówienia (NOT NULL - pozycja musi należeć do zamówienia)
        /// </summary>
        public int IdOrder { get; set; }

        /// <summary>
        /// Navigation property do zamówienia (Master)
        /// </summary>
        public virtual Order Order { get; set; } = null!;

        // ========== RELACJA Z ITEM (wymagana) ==========
        
        /// <summary>
        /// FK do produktu
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Navigation property do produktu
        /// </summary>
        public virtual Item Item { get; set; } = null!;

        // ========== DANE POZYCJI ==========
        
        /// <summary>
        /// Ilość zamówiona
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Cena jednostkowa W MOMENCIE ZAMÓWIENIA
        /// (kopia z Item.Price - nie zmienia się gdy cena produktu się zmieni)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// Czy pozycja jest aktywna (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== COMPUTED PROPERTIES ==========
        
        /// <summary>
        /// Wartość pozycji (ilość × cena)
        /// </summary>
        [NotMapped]
        public decimal LineTotal => (Quantity ?? 0) * (UnitPrice ?? 0);
    }
}
```

### 2.3. Konfiguracja w DbContext

**Data/ApplicationDbContext.cs (fragment):**

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // =========================================
    // KONFIGURACJA ORDER
    // =========================================

    // Order → Client (1:M, opcjonalna)
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Client)
        .WithMany(c => c.Orders)  // Zakładając że Client ma ICollection<Order>
        .HasForeignKey(o => o.IdClient)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.SetNull);  // Usunięcie klienta = NULL w zamówieniach

    // Order → Worker (1:M, opcjonalna)
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Worker)
        .WithMany(w => w.Orders)  // Zakładając że Worker ma ICollection<Order>
        .HasForeignKey(o => o.IdWorker)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.SetNull);

    // =========================================
    // KONFIGURACJA ORDER_ITEM (MASTER-DETAIL)
    // =========================================

    // OrderItem → Order (1:M, wymagana, CASCADE DELETE)
    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Order)
        .WithMany(o => o.OrderItems)
        .HasForeignKey(oi => oi.IdOrder)
        .IsRequired(true)  // Pozycja MUSI mieć zamówienie
        .OnDelete(DeleteBehavior.Cascade);  // Usunięcie zamówienia = usunięcie pozycji

    // OrderItem → Item (1:M, wymagana, RESTRICT)
    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Item)
        .WithMany(i => i.OrderItems)  // Zakładając że Item ma ICollection<OrderItem>
        .HasForeignKey(oi => oi.IdItem)
        .IsRequired(true)
        .OnDelete(DeleteBehavior.Restrict);  // Nie można usunąć produktu który jest w zamówieniu

    // =========================================
    // SEED DATA
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
        }
    );

    modelBuilder.Entity<OrderItem>().HasData(
        new OrderItem
        {
            IdOrderItem = 1,
            IdOrder = 1,
            IdItem = 1,  // Laptop
            Quantity = 2,
            UnitPrice = 3500m,
            IsActive = true
        },
        new OrderItem
        {
            IdOrderItem = 2,
            IdOrder = 1,
            IdItem = 2,  // Monitor
            Quantity = 3,
            UnitPrice = 800m,
            IsActive = true
        }
    );
}
```

---

## CZĘŚĆ 3: CQRS - Queries dla Zamówień (35 minut)

### 3.1. DTOs dla Zamówień

**Dto/OrderDto.cs:**

```csharp
namespace SolutionOrdersReact.Server.Dto
{
    /// <summary>
    /// DTO dla zamówienia (lista i szczegóły)
    /// </summary>
    public class OrderDto
    {
        public int IdOrder { get; set; }
        public DateTime? DataOrder { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        // Dane klienta (z relacji)
        public int? IdClient { get; set; }
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }

        // Dane pracownika (z relacji)
        public int? IdWorker { get; set; }
        public string? WorkerName { get; set; }

        // Pozycje zamówienia (zagnieżdżona lista)
        public List<OrderItemDto> Items { get; set; } = new();

        // Obliczone wartości
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// DTO dla pozycji zamówienia
    /// </summary>
    public class OrderItemDto
    {
        public int IdOrderItem { get; set; }
        public int IdItem { get; set; }
        public string? ItemName { get; set; }
        public string? ItemCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? UnitName { get; set; }
        public bool IsActive { get; set; }
    }
}
```

### 3.2. GetAllOrdersQuery

**Features/Orders/Queries/GetAllOrders/GetAllOrdersQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrderDto>>
    {
        // Opcjonalne filtry
        public int? ClientId { get; set; }
        public int? WorkerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}
```

### 3.3. GetAllOrdersHandler z zagnieżdżonymi Include

**Features/Orders/Queries/GetAllOrders/GetAllOrdersHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllOrdersHandler> _logger;

        public GetAllOrdersHandler(
            ApplicationDbContext context,
            ILogger<GetAllOrdersHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderDto>> Handle(
            GetAllOrdersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie zamówień");

            // Budowanie query z Include'ami
            var query = _context.Orders
                .Include(o => o.Client)                    // Relacja z Client
                .Include(o => o.Worker)                    // Relacja z Worker
                .Include(o => o.OrderItems)                // Relacja z OrderItems
                    .ThenInclude(oi => oi.Item)            // Zagnieżdżona relacja OrderItem → Item
                        .ThenInclude(i => i.UnitOfMeasurement)  // Jeszcze głębiej: Item → Unit
                .AsQueryable();

            // Filtrowanie po aktywności
            if (!request.IncludeInactive)
            {
                query = query.Where(o => o.IsActive);
            }

            // Filtrowanie po kliencie
            if (request.ClientId.HasValue)
            {
                query = query.Where(o => o.IdClient == request.ClientId.Value);
            }

            // Filtrowanie po pracowniku
            if (request.WorkerId.HasValue)
            {
                query = query.Where(o => o.IdWorker == request.WorkerId.Value);
            }

            // Filtrowanie po dacie
            if (request.DateFrom.HasValue)
            {
                query = query.Where(o => o.DataOrder >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(o => o.DataOrder <= request.DateTo.Value);
            }

            // Projekcja do DTO
            var orders = await query
                .OrderByDescending(o => o.DataOrder)  // Najnowsze pierwsze
                .Select(o => new OrderDto
                {
                    IdOrder = o.IdOrder,
                    DataOrder = o.DataOrder,
                    DeliveryDate = o.DeliveryDate,
                    Notes = o.Notes,
                    IsActive = o.IsActive,

                    // Dane klienta
                    IdClient = o.IdClient,
                    ClientName = o.Client != null ? o.Client.Name : null,
                    ClientPhone = o.Client != null ? o.Client.PhoneNumber : null,

                    // Dane pracownika
                    IdWorker = o.IdWorker,
                    WorkerName = o.Worker != null
                        ? o.Worker.FirstName + " " + o.Worker.LastName
                        : null,

                    // Pozycje zamówienia (zagnieżdżony Select)
                    Items = o.OrderItems
                        .Where(oi => oi.IsActive)
                        .Select(oi => new OrderItemDto
                        {
                            IdOrderItem = oi.IdOrderItem,
                            IdItem = oi.IdItem,
                            ItemName = oi.Item.Name,
                            ItemCode = oi.Item.Code,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            LineTotal = (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0),
                            UnitName = oi.Item.UnitOfMeasurement != null
                                ? oi.Item.UnitOfMeasurement.Name
                                : "szt",
                            IsActive = oi.IsActive
                        })
                        .ToList(),

                    // Obliczone wartości
                    TotalAmount = o.OrderItems
                        .Where(oi => oi.IsActive)
                        .Sum(oi => (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0)),
                    ItemCount = o.OrderItems.Count(oi => oi.IsActive)
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Pobrano {Count} zamówień", orders.Count);

            return orders;
        }
    }
}
```

### 3.4. GetOrderByIdQuery

**Features/Orders/Queries/GetOrderById/GetOrderByIdQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto?>
    {
        public int Id { get; set; }

        public GetOrderByIdQuery(int id)
        {
            Id = id;
        }
    }
}
```

**GetOrderByIdHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetOrderByIdHandler> _logger;

        public GetOrderByIdHandler(
            ApplicationDbContext context,
            ILogger<GetOrderByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OrderDto?> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie zamówienia ID: {Id}", request.Id);

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Worker)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                        .ThenInclude(i => i.UnitOfMeasurement)
                .Where(o => o.IdOrder == request.Id)
                .Select(o => new OrderDto
                {
                    IdOrder = o.IdOrder,
                    DataOrder = o.DataOrder,
                    DeliveryDate = o.DeliveryDate,
                    Notes = o.Notes,
                    IsActive = o.IsActive,
                    IdClient = o.IdClient,
                    ClientName = o.Client != null ? o.Client.Name : null,
                    ClientPhone = o.Client != null ? o.Client.PhoneNumber : null,
                    IdWorker = o.IdWorker,
                    WorkerName = o.Worker != null
                        ? o.Worker.FirstName + " " + o.Worker.LastName
                        : null,
                    Items = o.OrderItems
                        .Select(oi => new OrderItemDto
                        {
                            IdOrderItem = oi.IdOrderItem,
                            IdItem = oi.IdItem,
                            ItemName = oi.Item.Name,
                            ItemCode = oi.Item.Code,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            LineTotal = (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0),
                            UnitName = oi.Item.UnitOfMeasurement != null
                                ? oi.Item.UnitOfMeasurement.Name
                                : "szt",
                            IsActive = oi.IsActive
                        })
                        .ToList(),
                    TotalAmount = o.OrderItems
                        .Where(oi => oi.IsActive)
                        .Sum(oi => (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0)),
                    ItemCount = o.OrderItems.Count(oi => oi.IsActive)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Zamówienie ID {Id} nie znalezione", request.Id);
            }

            return order;
        }
    }
}
```

---

## CZĘŚĆ 4: CQRS - Commands dla Zamówień (40 minut)

### 4.1. CreateOrderCommand z pozycjami

**Features/Orders/Commands/CreateOrder/CreateOrderCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Command do tworzenia nowego zamówienia z pozycjami
    /// </summary>
    public class CreateOrderCommand : IRequest<int>
    {
        // Dane zamówienia
        public int? IdClient { get; set; }
        public int? IdWorker { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Lista pozycji (Detail)
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO dla pojedynczej pozycji przy tworzeniu
    /// </summary>
    public class CreateOrderItemDto
    {
        public int IdItem { get; set; }
        public decimal Quantity { get; set; }
        // UnitPrice pobierzemy z Item przy zapisie
    }
}
```

### 4.2. CreateOrderHandler z transakcją

**Features/Orders/Commands/CreateOrder/CreateOrderHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(
            ApplicationDbContext context,
            ILogger<CreateOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie zamówienia dla klienta {ClientId}", request.IdClient);

            // =========================================
            // WALIDACJA
            // =========================================

            // Sprawdź czy są pozycje
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("Zamówienie musi zawierać przynajmniej jedną pozycję");
            }

            // Sprawdź czy klient istnieje (jeśli podany)
            if (request.IdClient.HasValue)
            {
                var clientExists = await _context.Clients
                    .AnyAsync(c => c.IdClient == request.IdClient && c.IsActive, cancellationToken);

                if (!clientExists)
                {
                    throw new ArgumentException($"Klient o ID {request.IdClient} nie istnieje");
                }
            }

            // Sprawdź czy pracownik istnieje (jeśli podany)
            if (request.IdWorker.HasValue)
            {
                var workerExists = await _context.Workers
                    .AnyAsync(w => w.IdWorker == request.IdWorker && w.IsActive, cancellationToken);

                if (!workerExists)
                {
                    throw new ArgumentException($"Pracownik o ID {request.IdWorker} nie istnieje");
                }
            }

            // Pobierz produkty dla walidacji i cen
            var itemIds = request.Items.Select(i => i.IdItem).ToList();
            var items = await _context.Items
                .Where(i => itemIds.Contains(i.IdItem))
                .ToDictionaryAsync(i => i.IdItem, cancellationToken);

            // Sprawdź czy wszystkie produkty istnieją
            foreach (var itemDto in request.Items)
            {
                if (!items.ContainsKey(itemDto.IdItem))
                {
                    throw new ArgumentException($"Produkt o ID {itemDto.IdItem} nie istnieje");
                }

                if (!items[itemDto.IdItem].IsActive)
                {
                    throw new ArgumentException($"Produkt '{items[itemDto.IdItem].Name}' jest nieaktywny");
                }

                if (itemDto.Quantity <= 0)
                {
                    throw new ArgumentException($"Ilość musi być większa od 0 dla produktu '{items[itemDto.IdItem].Name}'");
                }
            }

            // =========================================
            // TWORZENIE W TRANSAKCJI
            // =========================================

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Utwórz zamówienie (Master)
                var order = new Order
                {
                    DataOrder = DateTime.Now,
                    IdClient = request.IdClient,
                    IdWorker = request.IdWorker,
                    Notes = request.Notes,
                    DeliveryDate = request.DeliveryDate,
                    IsActive = true
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Utworzono zamówienie ID: {OrderId}", order.IdOrder);

                // Utwórz pozycje (Detail)
                foreach (var itemDto in request.Items)
                {
                    var item = items[itemDto.IdItem];

                    var orderItem = new OrderItem
                    {
                        IdOrder = order.IdOrder,
                        IdItem = itemDto.IdItem,
                        Quantity = itemDto.Quantity,
                        UnitPrice = item.Price ?? 0,  // Kopiujemy aktualną cenę!
                        IsActive = true
                    };

                    _context.OrderItems.Add(orderItem);

                    _logger.LogInformation(
                        "Dodano pozycję: {ItemName} x {Quantity} @ {Price}",
                        item.Name, itemDto.Quantity, item.Price);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Commit transakcji
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Zamówienie {OrderId} utworzone z {Count} pozycjami",
                    order.IdOrder, request.Items.Count);

                return order.IdOrder;
            }
            catch (Exception ex)
            {
                // Rollback w przypadku błędu
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Błąd podczas tworzenia zamówienia");
                throw;
            }
        }
    }
}
```

### 4.3. UpdateOrderCommand

**Features/Orders/Commands/UpdateOrder/UpdateOrderCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<Unit>
    {
        public int IdOrder { get; set; }
        public int? IdClient { get; set; }
        public int? IdWorker { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool IsActive { get; set; }

        // Aktualizacja pozycji - pełna wymiana
        public List<UpdateOrderItemDto> Items { get; set; } = new();
    }

    public class UpdateOrderItemDto
    {
        public int? IdOrderItem { get; set; }  // null = nowa pozycja
        public int IdItem { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
```

**UpdateOrderHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateOrderHandler> _logger;

        public UpdateOrderHandler(
            ApplicationDbContext context,
            ILogger<UpdateOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Aktualizacja zamówienia ID: {OrderId}", request.IdOrder);

            // Znajdź zamówienie z pozycjami
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdOrder == request.IdOrder, cancellationToken);

            if (order == null)
            {
                throw new KeyNotFoundException($"Zamówienie o ID {request.IdOrder} nie istnieje");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Aktualizuj dane zamówienia
                order.IdClient = request.IdClient;
                order.IdWorker = request.IdWorker;
                order.Notes = request.Notes;
                order.DeliveryDate = request.DeliveryDate;
                order.IsActive = request.IsActive;

                // Pobierz produkty do walidacji cen
                var itemIds = request.Items.Select(i => i.IdItem).ToList();
                var items = await _context.Items
                    .Where(i => itemIds.Contains(i.IdItem))
                    .ToDictionaryAsync(i => i.IdItem, cancellationToken);

                // Usuń pozycje które nie są w nowej liście (hard delete lub soft delete)
                var incomingIds = request.Items
                    .Where(i => i.IdOrderItem.HasValue)
                    .Select(i => i.IdOrderItem!.Value)
                    .ToList();

                var toRemove = order.OrderItems
                    .Where(oi => !incomingIds.Contains(oi.IdOrderItem))
                    .ToList();

                foreach (var item in toRemove)
                {
                    item.IsActive = false;  // Soft delete
                }

                // Aktualizuj istniejące i dodaj nowe
                foreach (var itemDto in request.Items)
                {
                    if (itemDto.IdOrderItem.HasValue)
                    {
                        // Aktualizacja istniejącej pozycji
                        var existing = order.OrderItems
                            .FirstOrDefault(oi => oi.IdOrderItem == itemDto.IdOrderItem);

                        if (existing != null)
                        {
                            existing.IdItem = itemDto.IdItem;
                            existing.Quantity = itemDto.Quantity;
                            existing.IsActive = itemDto.IsActive;
                            // Cena pozostaje oryginalna (z momentu zamówienia)
                        }
                    }
                    else
                    {
                        // Nowa pozycja
                        var newItem = new OrderItem
                        {
                            IdOrder = order.IdOrder,
                            IdItem = itemDto.IdItem,
                            Quantity = itemDto.Quantity,
                            UnitPrice = items.ContainsKey(itemDto.IdItem)
                                ? items[itemDto.IdItem].Price ?? 0
                                : 0,
                            IsActive = true
                        };

                        _context.OrderItems.Add(newItem);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Zamówienie {OrderId} zaktualizowane", request.IdOrder);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Błąd podczas aktualizacji zamówienia {OrderId}", request.IdOrder);
                throw;
            }
        }
    }
}
```

### 4.4. DeleteOrderCommand

**Features/Orders/Commands/DeleteOrder/DeleteOrderCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public int IdOrder { get; set; }
        public bool HardDelete { get; set; } = false;  // Domyślnie soft delete

        public DeleteOrderCommand(int idOrder, bool hardDelete = false)
        {
            IdOrder = idOrder;
            HardDelete = hardDelete;
        }
    }
}
```

**DeleteOrderHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteOrderHandler> _logger;

        public DeleteOrderHandler(
            ApplicationDbContext context,
            ILogger<DeleteOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Usuwanie zamówienia ID: {OrderId}, HardDelete: {HardDelete}",
                request.IdOrder, request.HardDelete);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdOrder == request.IdOrder, cancellationToken);

            if (order == null)
            {
                throw new KeyNotFoundException($"Zamówienie o ID {request.IdOrder} nie istnieje");
            }

            if (request.HardDelete)
            {
                // Hard delete - fizyczne usunięcie (CASCADE usunie też OrderItems)
                _context.Orders.Remove(order);
                _logger.LogWarning("Hard delete zamówienia {OrderId}", request.IdOrder);
            }
            else
            {
                // Soft delete - oznaczenie jako nieaktywne
                order.IsActive = false;
                foreach (var item in order.OrderItems)
                {
                    item.IsActive = false;
                }
                _logger.LogInformation("Soft delete zamówienia {OrderId}", request.IdOrder);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
```

---

## CZĘŚĆ 5: Controller API dla Zamówień (20 minut)

### 5.1. OrdersController

**Controllers/OrdersController.cs:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder;
using SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder;
using SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder;
using SolutionOrdersReact.Server.Features.Orders.Queries.GetAllOrders;
using SolutionOrdersReact.Server.Features.Orders.Queries.GetOrderById;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie zamówienia
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? clientId = null,
            [FromQuery] int? workerId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] bool includeInactive = false)
        {
            var query = new GetAllOrdersQuery
            {
                ClientId = clientId,
                WorkerId = workerId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                IncludeInactive = includeInactive
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Pobiera zamówienie po ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Zamówienie o ID {id} nie zostało znalezione" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nowe zamówienie z pozycjami
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var orderId = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = orderId },
                    new { id = orderId, message = "Zamówienie zostało utworzone" }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Aktualizuje zamówienie
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderCommand command)
        {
            if (id != command.IdOrder)
            {
                return BadRequest(new { message = "ID w URL nie zgadza się z ID w body" });
            }

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Usuwa zamówienie (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            try
            {
                var command = new DeleteOrderCommand(id, hardDelete);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
```

---

## CZĘŚĆ 6: React Native - Formularz Zamówienia (40 minut)

### 6.1. Instalacja DateTimePicker

```bash
# Instalacja
pnpm add @react-native-community/datetimepicker

# iOS - pod install
cd ios && pod install && cd ..

# WAŻNE: Przebuduj aplikację!
pnpm react-native run-android
```

### 6.2. TypeScript Types dla zamówień

**src/types/models.ts (dodaj):**

```typescript
// ========== ZAMÓWIENIA ==========

export interface Order {
  idOrder: number;
  dataOrder: string | null;
  deliveryDate: string | null;
  notes: string | null;
  isActive: boolean;
  idClient: number | null;
  clientName: string | null;
  clientPhone: string | null;
  idWorker: number | null;
  workerName: string | null;
  items: OrderItem[];
  totalAmount: number;
  itemCount: number;
}

export interface OrderItem {
  idOrderItem: number;
  idItem: number;
  itemName: string | null;
  itemCode: string | null;
  quantity: number | null;
  unitPrice: number | null;
  lineTotal: number;
  unitName: string | null;
  isActive: boolean;
}

export interface CreateOrderRequest {
  idClient?: number;
  idWorker?: number;
  notes?: string;
  deliveryDate?: string;
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  idItem: number;
  quantity: number;
}
```

### 6.3. API Service dla zamówień

**src/api/apiService.ts (dodaj):**

```typescript
// ========== ZAMÓWIENIA ==========

async getOrders(): Promise<Order[]> {
  return this.request<Order[]>('/Orders');
}

async getOrder(id: number): Promise<Order> {
  return this.request<Order>(`/Orders/${id}`);
}

async createOrder(data: CreateOrderRequest): Promise<{ id: number }> {
  return this.request<{ id: number }>('/Orders', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

async updateOrder(id: number, data: UpdateOrderRequest): Promise<void> {
  return this.request<void>(`/Orders/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ ...data, idOrder: id }),
  });
}

async deleteOrder(id: number): Promise<void> {
  return this.request<void>(`/Orders/${id}`, {
    method: 'DELETE',
  });
}
```

### 6.4. Komponent OrderFormScreen

**src/screens/OrderFormScreen.tsx:**

```tsx
import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ScrollView,
  Alert,
  ActivityIndicator,
  Platform,
} from 'react-native';
import DateTimePicker from '@react-native-community/datetimepicker';
import { PickerField } from '../components/PickerField';
import apiService from '../api/apiService';
import type { Client, Worker, Item, CreateOrderItemRequest } from '../types/models';

interface OrderFormProps {
  navigation: any;
  route?: any;
}

interface OrderLineItem {
  id: string;  // Tymczasowe ID dla key w liście
  idItem: number | null;
  itemName: string;
  quantity: string;
  unitPrice: number;
}

const OrderFormScreen: React.FC<OrderFormProps> = ({ navigation }) => {
  // ========== STATE ==========
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // Dane słownikowe
  const [clients, setClients] = useState<Client[]>([]);
  const [workers, setWorkers] = useState<Worker[]>([]);
  const [items, setItems] = useState<Item[]>([]);

  // Dane formularza
  const [idClient, setIdClient] = useState<number | null>(null);
  const [idWorker, setIdWorker] = useState<number | null>(null);
  const [notes, setNotes] = useState('');
  const [deliveryDate, setDeliveryDate] = useState<Date>(new Date());
  const [showDatePicker, setShowDatePicker] = useState(false);

  // Pozycje zamówienia
  const [orderItems, setOrderItems] = useState<OrderLineItem[]>([
    { id: '1', idItem: null, itemName: '', quantity: '1', unitPrice: 0 }
  ]);

  // ========== LOAD DATA ==========
  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [clientsData, workersData, itemsData] = await Promise.all([
        apiService.getClients(),
        apiService.getWorkers(),
        apiService.getItems(),
      ]);
      setClients(clientsData);
      setWorkers(workersData);
      setItems(itemsData);
    } catch (error) {
      Alert.alert('Błąd', 'Nie udało się załadować danych');
    } finally {
      setLoading(false);
    }
  };

  // ========== ORDER ITEMS MANAGEMENT ==========
  const addOrderItem = () => {
    setOrderItems([
      ...orderItems,
      {
        id: Date.now().toString(),
        idItem: null,
        itemName: '',
        quantity: '1',
        unitPrice: 0,
      }
    ]);
  };

  const removeOrderItem = (id: string) => {
    if (orderItems.length <= 1) {
      Alert.alert('Błąd', 'Zamówienie musi mieć przynajmniej jedną pozycję');
      return;
    }
    setOrderItems(orderItems.filter(item => item.id !== id));
  };

  const updateOrderItem = (id: string, field: keyof OrderLineItem, value: any) => {
    setOrderItems(orderItems.map(item => {
      if (item.id !== id) return item;

      const updated = { ...item, [field]: value };

      // Jeśli zmieniono produkt - pobierz cenę
      if (field === 'idItem' && value) {
        const product = items.find(i => i.idItem === value);
        if (product) {
          updated.itemName = product.name || '';
          updated.unitPrice = product.price || 0;
        }
      }

      return updated;
    }));
  };

  // ========== CALCULATE TOTAL ==========
  const calculateTotal = (): number => {
    return orderItems.reduce((sum, item) => {
      const qty = parseFloat(item.quantity) || 0;
      return sum + (qty * item.unitPrice);
    }, 0);
  };

  // ========== DATE PICKER ==========
  const onDateChange = (event: any, selectedDate?: Date) => {
    setShowDatePicker(Platform.OS === 'ios');
    if (selectedDate) {
      setDeliveryDate(selectedDate);
    }
  };

  // ========== SUBMIT ==========
  const handleSubmit = async () => {
    // Walidacja
    const validItems = orderItems.filter(item => item.idItem !== null);
    if (validItems.length === 0) {
      Alert.alert('Błąd', 'Dodaj przynajmniej jedną pozycję');
      return;
    }

    for (const item of validItems) {
      const qty = parseFloat(item.quantity);
      if (isNaN(qty) || qty <= 0) {
        Alert.alert('Błąd', `Nieprawidłowa ilość dla produktu: ${item.itemName}`);
        return;
      }
    }

    try {
      setSubmitting(true);

      const orderData: CreateOrderRequest = {
        idClient: idClient || undefined,
        idWorker: idWorker || undefined,
        notes: notes.trim() || undefined,
        deliveryDate: deliveryDate.toISOString(),
        items: validItems.map(item => ({
          idItem: item.idItem!,
          quantity: parseFloat(item.quantity),
        })),
      };

      await apiService.createOrder(orderData);

      Alert.alert('Sukces', 'Zamówienie zostało utworzone', [
        { text: 'OK', onPress: () => navigation.goBack() }
      ]);
    } catch (error) {
      Alert.alert('Błąd', (error as Error).message);
    } finally {
      setSubmitting(false);
    }
  };

  // ========== RENDER ==========
  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Ładowanie danych...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <View style={styles.form}>
        <Text style={styles.title}>Nowe zamówienie</Text>

        {/* Klient */}
        <PickerField
          label="Klient"
          placeholder="Wybierz klienta (opcjonalne)"
          value={idClient}
          items={clients}
          getValue={(c) => c.idClient}
          getLabel={(c) => c.name || 'Brak nazwy'}
          onChange={(value) => setIdClient(value as number | null)}
          disabled={submitting}
        />

        {/* Pracownik */}
        <PickerField
          label="Pracownik"
          placeholder="Wybierz pracownika (opcjonalne)"
          value={idWorker}
          items={workers}
          getValue={(w) => w.idWorker}
          getLabel={(w) => `${w.firstName} ${w.lastName}`}
          onChange={(value) => setIdWorker(value as number | null)}
          disabled={submitting}
        />

        {/* Data dostawy */}
        <View style={styles.field}>
          <Text style={styles.label}>Data dostawy</Text>
          <TouchableOpacity
            style={styles.dateButton}
            onPress={() => setShowDatePicker(true)}
            disabled={submitting}
          >
            <Text style={styles.dateButtonText}>
              {deliveryDate.toLocaleDateString('pl-PL')}
            </Text>
          </TouchableOpacity>

          {showDatePicker && (
            <DateTimePicker
              value={deliveryDate}
              mode="date"
              display="default"
              onChange={onDateChange}
              minimumDate={new Date()}
            />
          )}
        </View>

        {/* Notatki */}
        <View style={styles.field}>
          <Text style={styles.label}>Notatki</Text>
          <TextInput
            style={[styles.input, styles.multiline]}
            placeholder="Dodatkowe uwagi do zamówienia..."
            value={notes}
            onChangeText={setNotes}
            multiline
            numberOfLines={3}
            editable={!submitting}
          />
        </View>

        {/* ========== POZYCJE ZAMÓWIENIA ========== */}
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Pozycje zamówienia</Text>
          <TouchableOpacity
            style={styles.addItemButton}
            onPress={addOrderItem}
            disabled={submitting}
          >
            <Text style={styles.addItemButtonText}>+ Dodaj pozycję</Text>
          </TouchableOpacity>
        </View>

        {orderItems.map((item, index) => (
          <View key={item.id} style={styles.orderItemCard}>
            <View style={styles.orderItemHeader}>
              <Text style={styles.orderItemIndex}>#{index + 1}</Text>
              {orderItems.length > 1 && (
                <TouchableOpacity
                  onPress={() => removeOrderItem(item.id)}
                  disabled={submitting}
                >
                  <Text style={styles.removeItemText}>🗑️ Usuń</Text>
                </TouchableOpacity>
              )}
            </View>

            {/* Wybór produktu */}
            <PickerField
              label="Produkt"
              placeholder="Wybierz produkt..."
              value={item.idItem}
              items={items}
              getValue={(i) => i.idItem}
              getLabel={(i) => `${i.name} (${i.price?.toFixed(2)} zł)`}
              onChange={(value) => updateOrderItem(item.id, 'idItem', value)}
              required
              disabled={submitting}
            />

            {/* Ilość */}
            <View style={styles.quantityRow}>
              <View style={styles.quantityField}>
                <Text style={styles.label}>Ilość</Text>
                <TextInput
                  style={styles.input}
                  value={item.quantity}
                  onChangeText={(text) => updateOrderItem(item.id, 'quantity', text)}
                  keyboardType="decimal-pad"
                  editable={!submitting}
                />
              </View>

              <View style={styles.priceInfo}>
                <Text style={styles.priceLabel}>Cena jedn.:</Text>
                <Text style={styles.priceValue}>{item.unitPrice.toFixed(2)} zł</Text>
                <Text style={styles.lineTotalLabel}>Suma:</Text>
                <Text style={styles.lineTotalValue}>
                  {((parseFloat(item.quantity) || 0) * item.unitPrice).toFixed(2)} zł
                </Text>
              </View>
            </View>
          </View>
        ))}

        {/* ========== PODSUMOWANIE ========== */}
        <View style={styles.totalSection}>
          <Text style={styles.totalLabel}>RAZEM:</Text>
          <Text style={styles.totalValue}>{calculateTotal().toFixed(2)} zł</Text>
        </View>

        {/* ========== PRZYCISKI ========== */}
        <View style={styles.buttons}>
          <TouchableOpacity
            style={[styles.button, styles.cancelButton]}
            onPress={() => navigation.goBack()}
            disabled={submitting}
          >
            <Text style={styles.cancelButtonText}>Anuluj</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.button, styles.submitButton, submitting && styles.disabledButton]}
            onPress={handleSubmit}
            disabled={submitting}
          >
            <Text style={styles.submitButtonText}>
              {submitting ? 'Wysyłanie...' : 'Utwórz zamówienie'}
            </Text>
          </TouchableOpacity>
        </View>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  centerContainer: { flex: 1, justifyContent: 'center', alignItems: 'center' },
  loadingText: { marginTop: 10, color: '#666' },
  form: { padding: 16 },
  title: { fontSize: 24, fontWeight: 'bold', marginBottom: 20, color: '#333' },
  field: { marginBottom: 16 },
  label: { fontSize: 14, fontWeight: '600', color: '#333', marginBottom: 8 },
  input: {
    borderWidth: 1, borderColor: '#ddd', borderRadius: 8,
    padding: 12, fontSize: 16, backgroundColor: '#fff',
  },
  multiline: { height: 80, textAlignVertical: 'top' },
  dateButton: {
    borderWidth: 1, borderColor: '#ddd', borderRadius: 8,
    padding: 12, backgroundColor: '#fff',
  },
  dateButtonText: { fontSize: 16, color: '#333' },
  sectionHeader: {
    flexDirection: 'row', justifyContent: 'space-between',
    alignItems: 'center', marginTop: 20, marginBottom: 12,
  },
  sectionTitle: { fontSize: 18, fontWeight: 'bold', color: '#333' },
  addItemButton: { backgroundColor: '#4CAF50', paddingHorizontal: 12, paddingVertical: 8, borderRadius: 6 },
  addItemButtonText: { color: '#fff', fontWeight: '600' },
  orderItemCard: {
    backgroundColor: '#fff', padding: 16, borderRadius: 8,
    marginBottom: 12, borderWidth: 1, borderColor: '#e0e0e0',
  },
  orderItemHeader: {
    flexDirection: 'row', justifyContent: 'space-between',
    alignItems: 'center', marginBottom: 12,
  },
  orderItemIndex: { fontSize: 16, fontWeight: 'bold', color: '#007AFF' },
  removeItemText: { color: '#F44336', fontSize: 14 },
  quantityRow: { flexDirection: 'row', alignItems: 'flex-start' },
  quantityField: { flex: 1, marginRight: 16 },
  priceInfo: { backgroundColor: '#f0f0f0', padding: 12, borderRadius: 8 },
  priceLabel: { fontSize: 12, color: '#666' },
  priceValue: { fontSize: 16, fontWeight: '600', color: '#333' },
  lineTotalLabel: { fontSize: 12, color: '#666', marginTop: 8 },
  lineTotalValue: { fontSize: 18, fontWeight: 'bold', color: '#4CAF50' },
  totalSection: {
    flexDirection: 'row', justifyContent: 'space-between',
    alignItems: 'center', backgroundColor: '#007AFF',
    padding: 16, borderRadius: 8, marginTop: 16,
  },
  totalLabel: { fontSize: 18, fontWeight: 'bold', color: '#fff' },
  totalValue: { fontSize: 24, fontWeight: 'bold', color: '#fff' },
  buttons: { flexDirection: 'row', marginTop: 24, marginBottom: 40 },
  button: { flex: 1, padding: 16, borderRadius: 8, alignItems: 'center' },
  cancelButton: { backgroundColor: '#f0f0f0', marginRight: 8 },
  cancelButtonText: { color: '#666', fontWeight: '600', fontSize: 16 },
  submitButton: { backgroundColor: '#007AFF', marginLeft: 8 },
  submitButtonText: { color: '#fff', fontWeight: '600', fontSize: 16 },
  disabledButton: { opacity: 0.5 },
});

export default OrderFormScreen;
```

---

## CZĘŚĆ 7: Lista Zamówień (20 minut)

### 7.1. OrdersListScreen

**src/screens/OrdersListScreen.tsx:**

```tsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  Alert,
} from 'react-native';
import apiService from '../api/apiService';
import type { Order } from '../types/models';

interface Props {
  navigation: any;
}

const OrdersListScreen: React.FC<Props> = ({ navigation }) => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const loadOrders = useCallback(async () => {
    try {
      const data = await apiService.getOrders();
      setOrders(data);
    } catch (error) {
      Alert.alert('Błąd', 'Nie udało się załadować zamówień');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    loadOrders();
  }, [loadOrders]);

  useEffect(() => {
    const unsubscribe = navigation.addListener('focus', () => {
      loadOrders();
    });
    return unsubscribe;
  }, [navigation, loadOrders]);

  const onRefresh = () => {
    setRefreshing(true);
    loadOrders();
  };

  const formatDate = (dateString: string | null): string => {
    if (!dateString) return 'Brak daty';
    return new Date(dateString).toLocaleDateString('pl-PL');
  };

  const renderOrder = ({ item }: { item: Order }) => (
    <TouchableOpacity
      style={styles.orderCard}
      onPress={() => navigation.navigate('OrderDetails', { orderId: item.idOrder })}
    >
      <View style={styles.orderHeader}>
        <Text style={styles.orderId}>Zamówienie #{item.idOrder}</Text>
        <Text style={styles.orderDate}>{formatDate(item.dataOrder)}</Text>
      </View>

      <View style={styles.orderInfo}>
        <Text style={styles.clientName}>
          👤 {item.clientName || 'Klient anonimowy'}
        </Text>
        {item.workerName && (
          <Text style={styles.workerName}>
            🔧 {item.workerName}
          </Text>
        )}
      </View>

      <View style={styles.orderFooter}>
        <Text style={styles.itemCount}>
          📦 {item.itemCount} pozycji
        </Text>
        <Text style={styles.totalAmount}>
          {item.totalAmount.toFixed(2)} zł
        </Text>
      </View>

      {item.deliveryDate && (
        <Text style={styles.deliveryDate}>
          🚚 Dostawa: {formatDate(item.deliveryDate)}
        </Text>
      )}
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Zamówienia ({orders.length})</Text>
        <TouchableOpacity
          style={styles.addButton}
          onPress={() => navigation.navigate('OrderForm')}
        >
          <Text style={styles.addButtonText}>+ Nowe</Text>
        </TouchableOpacity>
      </View>

      <FlatList
        data={orders}
        keyExtractor={(item) => item.idOrder.toString()}
        renderItem={renderOrder}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
        ListEmptyComponent={
          <Text style={styles.emptyText}>Brak zamówień</Text>
        }
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  centerContainer: { flex: 1, justifyContent: 'center', alignItems: 'center' },
  header: {
    flexDirection: 'row', justifyContent: 'space-between',
    alignItems: 'center', padding: 16, backgroundColor: '#fff',
    borderBottomWidth: 1, borderBottomColor: '#e0e0e0',
  },
  title: { fontSize: 24, fontWeight: 'bold', color: '#333' },
  addButton: { backgroundColor: '#007AFF', paddingHorizontal: 16, paddingVertical: 8, borderRadius: 8 },
  addButtonText: { color: '#fff', fontWeight: '600' },
  listContent: { padding: 16 },
  orderCard: {
    backgroundColor: '#fff', padding: 16, borderRadius: 12,
    marginBottom: 12, shadowColor: '#000', shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1, shadowRadius: 4, elevation: 3,
  },
  orderHeader: {
    flexDirection: 'row', justifyContent: 'space-between',
    marginBottom: 8,
  },
  orderId: { fontSize: 16, fontWeight: 'bold', color: '#007AFF' },
  orderDate: { fontSize: 14, color: '#666' },
  orderInfo: { marginBottom: 8 },
  clientName: { fontSize: 14, color: '#333', marginBottom: 4 },
  workerName: { fontSize: 14, color: '#666' },
  orderFooter: {
    flexDirection: 'row', justifyContent: 'space-between',
    alignItems: 'center', paddingTop: 8, borderTopWidth: 1, borderTopColor: '#f0f0f0',
  },
  itemCount: { fontSize: 14, color: '#666' },
  totalAmount: { fontSize: 18, fontWeight: 'bold', color: '#4CAF50' },
  deliveryDate: { fontSize: 12, color: '#FF9800', marginTop: 8 },
  emptyText: { textAlign: 'center', color: '#999', marginTop: 40 },
});

export default OrdersListScreen;
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Edycja zamówienia
Stwórz ekran `OrderEditScreen` do edycji istniejącego zamówienia z możliwością:
- Zmiany klienta/pracownika
- Dodawania/usuwania pozycji
- Zmiany ilości

### Zadanie 2: Szczegóły zamówienia
Stwórz ekran `OrderDetailsScreen` wyświetlający pełne szczegóły zamówienia z listą pozycji.

### Zadanie 3: Walidacja stanu magazynowego
Dodaj walidację w `CreateOrderHandler` sprawdzającą czy ilość produktu w magazynie jest wystarczająca.

### Zadanie 4: Anulowanie zamówienia
Dodaj funkcję anulowania zamówienia (zmiana statusu na "Anulowane").

### Zadanie 5: Filtrowanie zamówień
Dodaj filtry na liście zamówień: po dacie, kliencie, statusie.

---

## 🔍 Pytania Kontrolne

1. Czym różni się relacja Master-Detail od zwykłej 1:M?
2. Dlaczego używamy transakcji przy tworzeniu zamówienia z pozycjami?
3. Jak działa `ThenInclude()` w Entity Framework?
4. Dlaczego przechowujemy UnitPrice w OrderItem zamiast pobierać z Item?
5. Co to jest Cascade Delete i kiedy go używamy?
6. Jak obsługujemy DateTimePicker na różnych platformach (iOS/Android)?

---

## ➡️ Następna Lekcja

**[Lekcja 8: Walidacja – FluentValidation + Pipeline Behaviors](./lekcja-08-walidacja.md)**

W następnej lekcji:
- FluentValidation dla Commands
- Pipeline Behaviors w MediatR
- Globalna obsługa błędów walidacji
- Wyświetlanie błędów w React Native

---

**Gratulacje! 🎉 Umiesz już budować złożone formularze Master-Detail!**
