# Lekcja 9: Zaawansowane CQRS – Behaviors, Logging, Paginacja (2 godziny)

**Moduł:** CQRS Advanced Patterns  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Zaimplementować paginację i filtrowanie Queries
- ✅ Użyć custom pipeline behaviors do logowania
- ✅ Rozszerzyć CQRS o audyt i performance logging
- ✅ Testować wydajność handli 

---

## CZĘŚĆ 1: CQRS Queries – Paginacja i Filtrowanie (40 minut)

### 1.1. Rozszerzenie GetAllItemsQuery

**GetAllItemsQuery.cs:**
```csharp
public class GetAllItemsQuery : IRequest<PaginatedList<ItemDto>> {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}

public class PaginatedList<T> {
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
```

### 1.2. Handler z paginacją i filtrowaniem
```csharp
var queryable = _context.Items
    .Include(i => i.Category)
    .Include(i => i.UnitOfMeasurement)
    .Where(i => i.IsActive);
if (!string.IsNullOrEmpty(request.Search)) {
    queryable = queryable.Where(i => i.Name.Contains(request.Search));
}
var totalCount = await queryable.CountAsync(cancellationToken);
var items = await queryable
    .OrderBy(i => i.Name)
    .Skip((request.PageNumber - 1) * request.PageSize)
    .Take(request.PageSize)
    .Select(i => new ItemDto { /* ... */ })
    .ToListAsync(cancellationToken);
return new PaginatedList<ItemDto> {
    Items = items,
    TotalCount = totalCount,
    PageNumber = request.PageNumber,
    PageSize = request.PageSize
};
```

### 1.3. Controller z parametrami query
```csharp
[HttpGet]
public async Task<IActionResult> GetAll([FromQuery] GetAllItemsQuery query)
{
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

---

## CZĘŚĆ 2: Pipeline Logging Behavior (35 minut)

### 2.1. Wzorzec LoggingBehavior
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) { _logger = logger; }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {Request} with payload: {@Payload}", requestName, request);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        _logger.LogInformation("Handled {Request} in {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);
        return response;
    }
}
```

### 2.2. Rejestracja: 
```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

---

## CZĘŚĆ 3: Audyt – Logging do pliku (20 minut)

- Konfiguracja loggera do pliku w appsettings.json:
```json
"Logging": {
  "LogLevel": { "Default": "Information" },
  "File": {
    "Path": "Logs/log-.txt",
    "RollingInterval": "Day"
  }
}
```
- Możesz dodać (np. z Serilogiem, NLog) logowanie do pliku

---

## CZĘŚĆ 4: Performance Monitoring (10 minut)

- Stopwatch w LoggingBehavior
- Mierz ile ms trwał każdy request
- Zbieraj statystyki do monitorowania API

---

## 📝 Zadania praktyczne

### Zadanie 1: Rozszerz CQRS wszystkich encji o paginację/query params
### Zadanie 2: Add Search do kategorii i klientów
### Zadanie 3: Przetestuj wydajność 1000 requestów (czy loguje times)

---

## ➡️ Następna Lekcja
**[Lekcja 10: Natywne Moduły, Storage, Permissions](./lekcja-10-natywne-moduly.md)**

**Brawo! Poznałeś zaawansowane CQRS Patterny!**
