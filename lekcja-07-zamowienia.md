# Lekcja 7: Zamówienia – Relacje M:M z OrderItem (3 godziny)

**Moduł:** Relacje Many-to-Many, zamówienia w logice biznesowej  
**Czas trwania:** 3 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Modelować relacje M:M w EF Core
- ✅ Tworzyć zamówienia z wieloma pozycjami
- ✅ Mapować Order, OrderItem i Item do DTO
- ✅ Budować ekrany obsługujące zamówienia w React Native
- ✅ Implementować DateTimePicker

---

## CZĘŚĆ 1: Model bazy Order i OrderItem (20 minut)

### 1.1. Order.cs
```csharp
public class Order {
    public int IdOrder { get; set; }
    public DateTime? DataOrder { get; set; }
    public int? IdClient { get; set; }
    public int? IdWorker { get; set; }
    public string? Notes { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public virtual Client? Client { get; set; }
    public virtual Worker? Worker { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
```

### 1.2. OrderItem.cs
```csharp
public class OrderItem {
    public int IdOrderItem { get; set; }
    public int IdOrder { get; set; }
    public int IdItem { get; set; }
    public decimal? Quantity { get; set; }
    public bool IsActive { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;
}
```

---

## CZĘŚĆ 2: CQRS – Queries z relacjami (35 minut)

### 2.1. GetAllOrdersQuery
```csharp
public class GetAllOrdersQuery : IRequest<List<OrderDto>> { }
public class OrderDto {
    public int IdOrder { get; set; }
    public DateTime? DataOrder { get; set; }
    public string? ClientName { get; set; }
    public string? WorkerName { get; set; }
    public string? Notes { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
public class OrderItemDto {
    public int IdOrderItem { get; set; }
    public string? ItemName { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
}
```

### 2.2. Handler
```csharp
var orders = await _context.Orders
    .Include(o => o.Client)
    .Include(o => o.Worker)
    .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
    .Select(o => new OrderDto {
        IdOrder = o.IdOrder,
        DataOrder = o.DataOrder,
        ClientName = o.Client != null ? o.Client.Name : null,
        WorkerName = o.Worker != null ? o.Worker.FirstName + " " + o.Worker.LastName : null,
        Notes = o.Notes,
        DeliveryDate = o.DeliveryDate,
        Items = o.OrderItems.Select(oi => new OrderItemDto {
            IdOrderItem = oi.IdOrderItem,
            ItemName = oi.Item.Name,
            Quantity = oi.Quantity,
            Price = oi.Item.Price
        }).ToList()
    })
    .ToListAsync(cancellationToken);
```

---

## CZĘŚĆ 3: Tworzenie zamówienia – Command (25 minut)

### 3.1. CreateOrderCommand
```csharp
public class CreateOrderCommand : IRequest<int> {
    public int? IdClient { get; set; }
    public int? IdWorker { get; set; }
    public string? Notes { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
public class CreateOrderItemDto {
    public int IdItem { get; set; }
    public decimal? Quantity { get; set; }
}
```

### 3.2. Handler
```csharp
var order = new Order {
    DataOrder = DateTime.Now,
    IdClient = request.IdClient,
    IdWorker = request.IdWorker,
    Notes = request.Notes,
    DeliveryDate = request.DeliveryDate,
};
_context.Orders.Add(order);
await _context.SaveChangesAsync(cancellationToken);

// Dodajemy pozycje
foreach (var orderItem in request.Items) {
    var oi = new OrderItem {
        IdOrder = order.IdOrder,
        IdItem = orderItem.IdItem,
        Quantity = orderItem.Quantity,
        IsActive = true
    };
    _context.OrderItems.Add(oi);
}
await _context.SaveChangesAsync(cancellationToken);
return order.IdOrder;
```

---

## CZĘŚĆ 4: Formularz zamówienia w React Native (35 minut)

- PickerField dla klienta, pracownika
- Lista produktów z możliwością dodania każdej pozycji (Picker + ilość)
- Data dostawy: 
```tsx
<DateTimePicker ... />
```
- Walidacja: minimalna ilość > 0, wymagane klient/pozycje
- Wyświetlanie zamówienia z listą pozycji

---

## 📝 Zadania praktyczne

### Zadanie 1: Edycja zamówienia – dołóż obsługę edycji listy pozycji
### Zadanie 2: Usuwanie pozycji (OrderItem) z zamówienia
### Zadanie 3: CRUD dla Order i OrderItem

---

## ➡️ Następna Lekcja

**[Lekcja 8: Walidacja – FluentValidation + Pipeline Behaviors](./lekcja-08-walidacja.md)**

---

**Gratulacje! Umiesz już obsługiwać relacje złożone!**
