# Lekcja 3.3: CQRS Implementacje - Queries i Commands

**Moduł:** .NET Backend - CQRS  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Stworzyć strukturę Features/ (Vertical Slice)
- ✅ Zaimplementować Query z DTO
- ✅ Zaimplementować Command
- ✅ Utworzyć Controller z MediatR
- ✅ Testować API przez Swagger

---

## CZĘŚĆ 1: Vertical Slice Architecture

### 1.1. Czym jest Vertical Slice?

**Tradycyjna architektura (Layered):**
```
Controllers/
├── ItemsController.cs
├── OrdersController.cs
└── CategoriesController.cs

Services/
├── ItemService.cs
├── OrderService.cs
└── CategoryService.cs

Repositories/
├── ItemRepository.cs
├── OrderRepository.cs
└── CategoryRepository.cs
```

**❌ Problemy:**
- Zmiana wymaga edycji 3 plików
- Trudne znalezienie wszystkich elementów funkcjonalności
- Dużo abstrakcji (interfejsy repozytoriów)

**Vertical Slice Architecture:**
```
Features/
├── Items/
│   ├── Commands/
│   │   ├── CreateItem/
│   │   │   ├── CreateItemCommand.cs
│   │   │   ├── CreateItemHandler.cs
│   │   │   └── CreateItemValidator.cs
│   │   ├── UpdateItem/
│   │   └── DeleteItem/
│   └── Queries/
│       ├── GetAllItems/
│       │   ├── GetAllItemsQuery.cs
│       │   └── GetAllItemsHandler.cs
│       └── GetItemById/
├── Orders/
└── Categories/
```

**✅ Zalety:**
- Wszystko związane z funkcją w jednym miejscu
- Łatwe dodawanie nowych funkcji
- Szybsze nawigowanie w kodzie
- Mniej abstrakcji

### 1.2. Utworzenie Struktury

W projekcie **SolutionOrdersReact.Server** utwórz:

```
Features/
├── Items/
│   ├── Commands/
│   │   └── CreateItem/
│   └── Queries/
│       └── GetAllItems/
```

---

## CZĘŚĆ 2: Pierwszy Query - GetAllItems

### 2.1. GetAllItemsQuery.cs

**Features/Items/Queries/GetAllItems/GetAllItemsQuery.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems
{
    // Query = Request który zwraca List<ItemDto>
    public class GetAllItemsQuery : IRequest<List<ItemDto>>
    {
        // Query bez parametrów - po prostu "daj wszystkie"
    }

    // DTO (Data Transfer Object) - model dla API
    public class ItemDto
    {
        public int IdItem { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int IdCategory { get; set; }
        public string? CategoryName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public int? IdUnitOfMeasurement { get; set; }
        public string? UnitName { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
```

**Wyjaśnienie:**
- `IRequest<T>` - interfejs z MediatR, `T` = typ zwracany
- **DTO** - model specjalnie dla API (nie encja z bazy!)
- Możemy tu dodać pola z relacji (CategoryName, UnitName)

### 2.2. GetAllItemsHandler.cs

**Features/Items/Queries/GetAllItems/GetAllItemsHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems
{
    // Handler = logika biznesowa dla Query
    public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, List<ItemDto>>
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection - DbContext
        public GetAllItemsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        // Handle = główna metoda
        public async Task<List<ItemDto>> Handle(
            GetAllItemsQuery request,
            CancellationToken cancellationToken)
        {
            // Query do bazy z Include (EAGER LOADING)
            var items = await _context.Items
                .Include(i => i.Category)              // JOIN z Category
                .Include(i => i.UnitOfMeasurement)     // JOIN z UnitOfMeasurement
                .Where(i => i.IsActive)                // Tylko aktywne
                .OrderBy(i => i.Name)                  // Sortowanie
                .Select(i => new ItemDto               // Projekcja do DTO
                {
                    IdItem = i.IdItem,
                    Name = i.Name,
                    Description = i.Description,
                    IdCategory = i.IdCategory,
                    CategoryName = i.Category.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    IdUnitOfMeasurement = i.IdUnitOfMeasurement,
                    UnitName = i.UnitOfMeasurement != null 
                        ? i.UnitOfMeasurement.Name 
                        : null,
                    Code = i.Code,
                    IsActive = i.IsActive
                })
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
```

**Wyjaśnienie:**
- `IRequestHandler<TRequest, TResponse>` - interfejs handlera
- `Include()` - EAGER loading relacji (JOIN w SQL)
- `Select()` - projekcja do DTO (SELECT konkretne kolumny)
- `ToListAsync()` - asynchroniczne wykonanie query
- `cancellationToken` - pozwala anulować operację

**SQL który się wygeneruje:**
```sql
SELECT 
    i.IdItem, i.Name, i.Description, i.IdCategory,
    c.Name AS CategoryName,
    i.Price, i.Quantity, i.IdUnitOfMeasurement,
    u.Name AS UnitName,
    i.Code, i.IsActive
FROM Items i
LEFT JOIN Categories c ON i.IdCategory = c.IdCategory
LEFT JOIN UnitOfMeasurements u ON i.IdUnitOfMeasurement = u.IdUnitOfMeasurement
WHERE i.IsActive = 1
ORDER BY i.Name
```

### 2.3. Controller dla Items

**Controllers/ItemsController.cs:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Dependency Injection - MediatR
        public ItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Pobiera wszystkie aktywne produkty
        /// </summary>
        /// <returns>Lista produktów</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            // Tworzymy Query
            var query = new GetAllItemsQuery();
            
            // Wysyłamy do MediatR
            var result = await _mediator.Send(query);
            
            // Zwracamy HTTP 200 OK z wynikiem
            return Ok(result);
        }
    }
}
```

**Wyjaśnienie:**
- `[ApiController]` - kontroler API (auto ModelState validation)
- `[Route("api/[controller]")]` - route = `/api/items`
- `_mediator.Send()` - wysyła request do odpowiedniego handlera
- `Ok(result)` - zwraca HTTP 200 z JSON

### 2.4. Testowanie w Swagger

1. **Uruchom aplikację:** F5
2. **Otwórz Swagger:** `https://localhost:7xxx/swagger`
3. **Znajdź** `GET /api/items`
4. **Kliknij "Try it out"** → **Execute**

**Wynik:**
```json
[
  {
    "idItem": 1,
    "name": "Laptop Dell",
    "description": "Laptop Dell Inspiron 15",
    "idCategory": 1,
    "categoryName": "Elektronika",
    "price": 3500,
    "quantity": 10,
    "idUnitOfMeasurement": 1,
    "unitName": "szt",
    "code": "LAP001",
    "isActive": true
  },
  {
    "idItem": 2,
    "name": "Monitor Samsung",
    "description": "Monitor 24 cale",
    "idCategory": 1,
    "categoryName": "Elektronika",
    "price": 800,
    "quantity": 15,
    "idUnitOfMeasurement": 1,
    "unitName": "szt",
    "code": "MON001",
    "isActive": true
  }
]
```

---

## CZĘŚĆ 3: Query z Parametrem - GetItemById

### 3.1. GetItemByIdQuery.cs

**Features/Items/Queries/GetItemById/GetItemByIdQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetItemById
{
    // Query z parametrem (Id)
    public class GetItemByIdQuery : IRequest<ItemDto?>
    {
        public int Id { get; set; }

        public GetItemByIdQuery(int id)
        {
            Id = id;
        }
    }
}
```

### 3.2. GetItemByIdHandler.cs

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetItemById
{
    public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetItemByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemDto?> Handle(
            GetItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.UnitOfMeasurement)
                .Where(i => i.IdItem == request.Id)    // Filtr po Id
                .Select(i => new ItemDto
                {
                    IdItem = i.IdItem,
                    Name = i.Name,
                    Description = i.Description,
                    IdCategory = i.IdCategory,
                    CategoryName = i.Category.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    IdUnitOfMeasurement = i.IdUnitOfMeasurement,
                    UnitName = i.UnitOfMeasurement != null ? i.UnitOfMeasurement.Name : null,
                    Code = i.Code,
                    IsActive = i.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);  // Pierwszy lub null

            return item;
        }
    }
}
```

### 3.3. Dodanie do Controllera

**ItemsController.cs:**

```csharp
using SolutionOrdersReact.Server.Features.Items.Queries.GetItemById;

// ... w klasie ItemsController:

/// <summary>
/// Pobiera produkt po ID
/// </summary>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetById(int id)
{
    var query = new GetItemByIdQuery(id);
    var result = await _mediator.Send(query);

    if (result == null)
    {
        return NotFound(new { message = $"Produkt o ID {id} nie został znaleziony" });
    }

    return Ok(result);
}
```

**Test:** `GET /api/items/1`

---

## CZĘŚĆ 4: Pierwszy Command - CreateItem

### 4.1. CreateItemCommand.cs

**Features/Items/Commands/CreateItem/CreateItemCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    // Command = Request który ZMIENIA stan (CREATE)
    // Zwraca ID utworzonego rekordu
    public class CreateItemCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int IdCategory { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string? FotoUrl { get; set; }
        public int? IdUnitOfMeasurement { get; set; }
        public string? Code { get; set; }
    }
}
```

### 4.2. CreateItemHandler.cs

```csharp
using MediatR;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    public class CreateItemHandler : IRequestHandler<CreateItemCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateItemHandler> _logger;

        public CreateItemHandler(
            ApplicationDbContext context,
            ILogger<CreateItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateItemCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie nowego produktu: {Name}", request.Name);

            // Mapowanie Command → Entity
            var item = new Item
            {
                Name = request.Name,
                Description = request.Description,
                IdCategory = request.IdCategory,
                Price = request.Price,
                Quantity = request.Quantity,
                FotoUrl = request.FotoUrl,
                IdUnitOfMeasurement = request.IdUnitOfMeasurement,
                Code = request.Code,
                IsActive = true  // Domyślnie aktywny
            };

            // Dodanie do DbContext
            _context.Items.Add(item);

            // Zapisanie do bazy
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono produkt ID: {IdItem}", item.IdItem);

            // Zwracamy ID
            return item.IdItem;
        }
    }
}
```

**Wyjaśnienie:**
- **Command** zawiera tylko dane wejściowe (nie logikę!)
- **Handler** wykonuje logikę (mapowanie, zapis, logging)
- `SaveChangesAsync()` wykonuje INSERT do bazy
- EF Core automatycznie wypełnia `item.IdItem` po zapisie (IDENTITY)

### 4.3. Dodanie do Controllera

**ItemsController.cs:**

```csharp
using SolutionOrdersReact.Server.Features.Items.Commands.CreateItem;

// ... w klasie ItemsController:

/// <summary>
/// Tworzy nowy produkt
/// </summary>
[HttpPost]
[ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
{
    var itemId = await _mediator.Send(command);

    // HTTP 201 Created z Location header
    return CreatedAtAction(
        nameof(GetById),
        new { id = itemId },
        new { id = itemId, message = "Produkt został utworzony" }
    );
}
```

**Wyjaśnienie:**
- `[HttpPost]` - metoda POST
- `[FromBody]` - dane z body requestu (JSON)
- `CreatedAtAction()` - HTTP 201 z headerem `Location: /api/items/123`

### 4.4. Test w Swagger

**POST /api/items**

Body:
```json
{
  "name": "Klawiatura Logitech",
  "description": "Klawiatura mechaniczna RGB",
  "idCategory": 1,
  "price": 299.99,
  "quantity": 30,
  "idUnitOfMeasurement": 1,
  "code": "KLA001"
}
```

**Odpowiedź:**
```
HTTP 201 Created
Location: /api/items/3

{
  "id": 3,
  "message": "Produkt został utworzony"
}
```

---

## CZĘŚĆ 5: Update i Delete Commands

### 5.1. UpdateItemCommand

**Features/Items/Commands/UpdateItem/UpdateItemCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem
{
    public class UpdateItemCommand : IRequest<Unit>  // Unit = void
    {
        public int IdItem { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int IdCategory { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string? FotoUrl { get; set; }
        public int? IdUnitOfMeasurement { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
```

**UpdateItemHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem
{
    public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateItemHandler> _logger;

        public UpdateItemHandler(
            ApplicationDbContext context,
            ILogger<UpdateItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateItemCommand request,
            CancellationToken cancellationToken)
        {
            // Znajdź rekord
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.IdItem == request.IdItem, cancellationToken);

            if (item == null)
            {
                throw new KeyNotFoundException($"Produkt o ID {request.IdItem} nie istnieje");
            }

            _logger.LogInformation("Aktualizacja produktu ID: {IdItem}", request.IdItem);

            // Aktualizacja pól
            item.Name = request.Name;
            item.Description = request.Description;
            item.IdCategory = request.IdCategory;
            item.Price = request.Price;
            item.Quantity = request.Quantity;
            item.FotoUrl = request.FotoUrl;
            item.IdUnitOfMeasurement = request.IdUnitOfMeasurement;
            item.Code = request.Code;
            item.IsActive = request.IsActive;

            // Zapis
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaktualizowano produkt ID: {IdItem}", request.IdItem);

            return Unit.Value;  // MediatR Unit = void
        }
    }
}
```

**Controller:**

```csharp
/// <summary>
/// Aktualizuje produkt
/// </summary>
[HttpPut("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Update(int id, [FromBody] UpdateItemCommand command)
{
    if (id != command.IdItem)
    {
        return BadRequest(new { message = "ID w URL różni się od ID w body" });
    }

    try
    {
        await _mediator.Send(command);
        return NoContent();  // HTTP 204 - sukces bez body
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
}
```

### 5.2. DeleteItemCommand

**Features/Items/Commands/DeleteItem/DeleteItemCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Commands.DeleteItem
{
    public class DeleteItemCommand : IRequest<Unit>
    {
        public int IdItem { get; set; }

        public DeleteItemCommand(int idItem)
        {
            IdItem = idItem;
        }
    }
}
```

**DeleteItemHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Commands.DeleteItem
{
    public class DeleteItemHandler : IRequestHandler<DeleteItemCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteItemHandler> _logger;

        public DeleteItemHandler(
            ApplicationDbContext context,
            ILogger<DeleteItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteItemCommand request,
            CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.IdItem == request.IdItem, cancellationToken);

            if (item == null)
            {
                throw new KeyNotFoundException($"Produkt o ID {request.IdItem} nie istnieje");
            }

            _logger.LogInformation("Usuwanie produktu ID: {IdItem}", request.IdItem);

            // Soft delete - tylko ustawiamy IsActive = false
            item.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            // Lub hard delete:
            // _context.Items.Remove(item);
            // await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto produkt ID: {IdItem}", request.IdItem);

            return Unit.Value;
        }
    }
}
```

**Controller:**

```csharp
/// <summary>
/// Usuwa produkt (soft delete)
/// </summary>
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(int id)
{
    var command = new DeleteItemCommand(id);

    try
    {
        await _mediator.Send(command);
        return NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
}
```

---

## CZĘŚĆ 6: Pełny CRUD dla Categories

### 6.1. Struktura Plików

Utwórz następującą strukturę:

```
Dto/
└── CategoryDto.cs

Requests/
└── Categories/
    ├── Queries/
    │   ├── GetAllCategoriesQuery.cs
    │   └── GetCategoryByIdQuery.cs
    └── Commands/
        ├── CreateCategoryCommand.cs
        ├── UpdateCategoryCommand.cs
        └── DeleteCategoryCommand.cs

Handlers/
└── Categories/
    ├── GetAllCategoriesHandler.cs
    ├── GetCategoryByIdHandler.cs
    ├── CreateCategoryHandler.cs
    ├── UpdateCategoryHandler.cs
    └── DeleteCategoryHandler.cs

Controllers/
└── CategoryController.cs
```

### 6.2. CategoryDto

**Dto/CategoryDto.cs:**

```csharp
namespace SolutionOrdersReact.Server.Dto
{
    public class CategoryDto
    {
        public int IdCategory { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
```

### 6.3. Queries

**Requests/Categories/Queries/GetAllCategoriesQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Requests.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}
```

**Requests/Categories/Queries/GetCategoryByIdQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Requests.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public int Id { get; set; }

        public GetCategoryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
```

### 6.4. Query Handlers

**Handlers/Categories/GetAllCategoriesHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Models;
using SolutionOrdersReact.Server.Requests.Categories.Queries;

namespace SolutionOrdersReact.Server.Handlers.Categories
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllCategoriesHandler> _logger;

        public GetAllCategoriesHandler(
            ApplicationDbContext context,
            ILogger<GetAllCategoriesHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(
            GetAllCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie wszystkich kategorii");

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    IdCategory = c.IdCategory,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Pobrano {Count} kategorii", categories.Count);

            return categories;
        }
    }
}
```

**Handlers/Categories/GetCategoryByIdHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Models;
using SolutionOrdersReact.Server.Requests.Categories.Queries;

namespace SolutionOrdersReact.Server.Handlers.Categories
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetCategoryByIdHandler> _logger;

        public GetCategoryByIdHandler(
            ApplicationDbContext context,
            ILogger<GetCategoryByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CategoryDto?> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie kategorii o ID: {Id}", request.Id);

            var category = await _context.Categories
                .Where(c => c.IdCategory == request.Id)
                .Select(c => new CategoryDto
                {
                    IdCategory = c.IdCategory,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (category == null)
            {
                _logger.LogWarning("Kategoria o ID {Id} nie została znaleziona", request.Id);
            }

            return category;
        }
    }
}
```

### 6.5. Commands

**Requests/Categories/Commands/CreateCategoryCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Requests.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
```

**Requests/Categories/Commands/UpdateCategoryCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Requests.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<Unit>
    {
        public int IdCategory { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
```

**Requests/Categories/Commands/DeleteCategoryCommand.cs:**

```csharp
using MediatR;

namespace SolutionOrdersReact.Server.Requests.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int IdCategory { get; set; }

        public DeleteCategoryCommand(int idCategory)
        {
            IdCategory = idCategory;
        }
    }
}
```

### 6.6. Command Handlers

**Handlers/Categories/CreateCategoryHandler.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Models;
using SolutionOrdersReact.Server.Requests.Categories.Commands;

namespace SolutionOrdersReact.Server.Handlers.Categories
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateCategoryHandler> _logger;

        public CreateCategoryHandler(
            ApplicationDbContext context,
            ILogger<CreateCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie nowej kategorii: {Name}", request.Name);

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono kategorię o ID: {IdCategory}", category.IdCategory);

            return category.IdCategory;
        }
    }
}
```

**Handlers/Categories/UpdateCategoryHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Models;
using SolutionOrdersReact.Server.Requests.Categories.Commands;

namespace SolutionOrdersReact.Server.Handlers.Categories
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateCategoryHandler> _logger;

        public UpdateCategoryHandler(
            ApplicationDbContext context,
            ILogger<UpdateCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Aktualizacja kategorii o ID: {IdCategory}", request.IdCategory);

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == request.IdCategory, cancellationToken);

            if (category == null)
            {
                _logger.LogError("Kategoria o ID {IdCategory} nie została znaleziona", request.IdCategory);
                throw new KeyNotFoundException($"Kategoria o ID {request.IdCategory} nie istnieje");
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaktualizowano kategorię o ID: {IdCategory}", request.IdCategory);

            return Unit.Value;
        }
    }
}
```

**Handlers/Categories/DeleteCategoryHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Models;
using SolutionOrdersReact.Server.Requests.Categories.Commands;

namespace SolutionOrdersReact.Server.Handlers.Categories
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteCategoryHandler> _logger;

        public DeleteCategoryHandler(
            ApplicationDbContext context,
            ILogger<DeleteCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Usuwanie kategorii o ID: {IdCategory}", request.IdCategory);

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == request.IdCategory, cancellationToken);

            if (category == null)
            {
                _logger.LogError("Kategoria o ID {IdCategory} nie została znaleziona", request.IdCategory);
                throw new KeyNotFoundException($"Kategoria o ID {request.IdCategory} nie istnieje");
            }

            // Soft delete - tylko ustawiamy IsActive = false
            category.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto (soft delete) kategorię o ID: {IdCategory}", request.IdCategory);

            return Unit.Value;
        }
    }
}
```

### 6.7. Controller

**Controllers/CategoryController.cs:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Requests.Categories.Commands;
using SolutionOrdersReact.Server.Requests.Categories.Queries;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie aktywne kategorie
        /// </summary>
        /// <returns>Lista kategorii</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/category - Pobieranie wszystkich kategorii");

            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera kategorię po ID
        /// </summary>
        /// <param name="id">ID kategorii</param>
        /// <returns>Kategoria</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/category/{Id} - Pobieranie kategorii", id);

            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Kategoria o ID {id} nie została znaleziona" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nową kategorię
        /// </summary>
        /// <param name="command">Dane nowej kategorii</param>
        /// <returns>ID utworzonej kategorii</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            _logger.LogInformation("POST /api/category - Tworzenie nowej kategorii");

            var categoryId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = categoryId },
                new { id = categoryId, message = "Kategoria została utworzona" }
            );
        }

        /// <summary>
        /// Aktualizuje kategorię
        /// </summary>
        /// <param name="id">ID kategorii</param>
        /// <param name="command">Zaktualizowane dane kategorii</param>
        /// <returns>Status operacji</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
        {
            _logger.LogInformation("PUT /api/category/{Id} - Aktualizacja kategorii", id);

            if (id != command.IdCategory)
            {
                return BadRequest(new { message = "ID w URL różni się od ID w body" });
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
        }

        /// <summary>
        /// Usuwa kategorię (soft delete)
        /// </summary>
        /// <param name="id">ID kategorii</param>
        /// <returns>Status operacji</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/category/{Id} - Usuwanie kategorii", id);

            var command = new DeleteCategoryCommand(id);

            try
            {
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

### 6.8. Konfiguracja Mappera (opcjonalnie)

Jeśli używasz Mapster, dodaj konfigurację:

**Mappings/CategoryMappingConfig.cs:**

```csharp
using Mapster;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Mappings
{
    public static class CategoryMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Category, CategoryDto>
                .NewConfig()
                .Map(dest => dest.IdCategory, src => src.IdCategory)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => src.IsActive);
        }
    }
}
```

I dodaj w **Program.cs**:

```csharp
// Mapster Configuration
ItemMappingConfig.Configure();
OrderMappingConfig.Configure();
CategoryMappingConfig.Configure();  // Dodaj tę linię
```

### 6.9. Testowanie w Swagger

#### Test 1: GET /api/category
Pobierz wszystkie kategorie.

**Oczekiwany wynik:**
```json
[
  {
    "idCategory": 1,
    "name": "Elektronika",
    "description": "Urządzenia elektroniczne",
    "isActive": true
  },
  {
    "idCategory": 2,
    "name": "Żywność",
    "description": "Produkty spożywcze",
    "isActive": true
  }
]
```

#### Test 2: POST /api/category
Utwórz nową kategorię.

**Body:**
```json
{
  "name": "AGD",
  "description": "Artykuły gospodarstwa domowego"
}
```

**Oczekiwany wynik:**
```
HTTP 201 Created
{
  "id": 3,
  "message": "Kategoria została utworzona"
}
```

#### Test 3: PUT /api/category/3
Zaktualizuj kategorię.

**Body:**
```json
{
  "idCategory": 3,
  "name": "AGD i RTV",
  "description": "Artykuły gospodarstwa domowego i RTV",
  "isActive": true
}
```

**Oczekiwany wynik:**
```
HTTP 204 No Content
```

#### Test 4: DELETE /api/category/3
Usuń kategorię (soft delete).

**Oczekiwany wynik:**
```
HTTP 204 No Content
```

### 6.10. Podsumowanie CRUD dla Categories

**Wzorzec CQRS w praktyce:**
- ✅ **5 plików Request** (2 Queries + 3 Commands)
- ✅ **5 plików Handler** (logika biznesowa)
- ✅ **1 Controller** (5 endpointów REST API)
- ✅ **1 DTO** (model dla API)
- ✅ **Soft Delete** (IsActive = false)
- ✅ **Logging** (info o każdej operacji)
- ✅ **Error Handling** (KeyNotFoundException → 404)

**Endpointy:**
```
GET    /api/category       → GetAllCategoriesQuery
GET    /api/category/{id}  → GetCategoryByIdQuery
POST   /api/category       → CreateCategoryCommand
PUT    /api/category/{id}  → UpdateCategoryCommand
DELETE /api/category/{id}  → DeleteCategoryCommand
```

---

## CZĘŚĆ 7: Pełny CRUD dla UnitOfMeasurement

### 7.1. Struktura Plików

Analogicznie do Categories, utwórz:

```
Dto/
└── UnitOfMeasurementDto.cs

Requests/
└── UnitOfMeasurements/
    ├── Queries/
    │   ├── GetAllUnitOfMeasurementsQuery.cs
    │   └── GetUnitOfMeasurementByIdQuery.cs
    └── Commands/
        ├── CreateUnitOfMeasurementCommand.cs
        ├── UpdateUnitOfMeasurementCommand.cs
        └── DeleteUnitOfMeasurementCommand.cs

Handlers/
└── UnitOfMeasurements/
    ├── GetAllUnitOfMeasurementsHandler.cs
    ├── GetUnitOfMeasurementByIdHandler.cs
    ├── CreateUnitOfMeasurementHandler.cs
    ├── UpdateUnitOfMeasurementHandler.cs
    └── DeleteUnitOfMeasurementHandler.cs

Controllers/
└── UnitOfMeasurementController.cs

Mappings/
└── UnitOfMeasurementMappingConfig.cs
```

### 7.2. Controller

**Controllers/UnitOfMeasurementController.cs:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Requests.UnitOfMeasurements.Commands;
using SolutionOrdersReact.Server.Requests.UnitOfMeasurements.Queries;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitOfMeasurementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UnitOfMeasurementController> _logger;

        public UnitOfMeasurementController(IMediator mediator, ILogger<UnitOfMeasurementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UnitOfMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllUnitOfMeasurementsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UnitOfMeasurementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetUnitOfMeasurementByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Jednostka miary o ID {id} nie została znaleziona" });
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateUnitOfMeasurementCommand command)
        {
            var unitId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = unitId }, 
                new { id = unitId, message = "Jednostka miary została utworzona" });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUnitOfMeasurementCommand command)
        {
            if (id != command.IdUnitOfMeasurement)
            {
                return BadRequest(new { message = "ID w URL różni się od ID w body" });
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
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteUnitOfMeasurementCommand(id);

            try
            {
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

### 7.3. Mapper

**Mappings/UnitOfMeasurementMappingConfig.cs:**

```csharp
using Mapster;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Mappings
{
    public static class UnitOfMeasurementMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<UnitOfMeasurement, UnitOfMeasurementDto>
                .NewConfig()
                .Map(dest => dest.IdUnitOfMeasurement, src => src.IdUnitOfMeasurement)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => src.IsActive);
        }
    }
}
```

**Program.cs:**

```csharp
// Mapster Configuration
ItemMappingConfig.Configure();
OrderMappingConfig.Configure();
CategoryMappingConfig.Configure();
UnitOfMeasurementMappingConfig.Configure();  // Dodaj tę linię
```

### 7.4. Testowanie

**Test 1: GET /api/unitofmeasurement**

```json
[
  {
    "idUnitOfMeasurement": 1,
    "name": "szt",
    "description": "Sztuki",
    "isActive": true
  },
  {
    "idUnitOfMeasurement": 2,
    "name": "kg",
    "description": "Kilogramy",
    "isActive": true
  }
]
```

**Test 2: POST /api/unitofmeasurement**

```json
{
  "name": "l",
  "description": "Litry"
}
```

### 7.5. Podsumowanie CRUD

**Wzorzec CQRS w praktyce:**
- ✅ **3 endpointy** - Category, UnitOfMeasurement, Items
- ✅ **Jednolity wzorzec** - wszystkie używają CQRS
- ✅ **Soft Delete** - IsActive = false
- ✅ **Logging** - każda operacja logowana
- ✅ **RESTful API** - zgodność ze standardami

**Struktura projektu:**
```
Controllers/
├── CategoryController.cs         (5 endpoints)
├── UnitOfMeasurementController.cs (5 endpoints)
└── ItemsController.cs            (5 endpoints)

Dto/
├── CategoryDto.cs
├── UnitOfMeasurementDto.cs
└── ItemDto.cs

Requests/
├── Categories/
├── UnitOfMeasurements/
└── Items/

Handlers/
├── Categories/
├── UnitOfMeasurements/
└── Items/
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: CRUD dla Client
Zaimplementuj pełny CRUD (GetAll, GetById, Create, Update, Delete) dla klientów (`Client`).

### Zadanie 2: Query z Filtrowaniem
Dodaj `GetItemsByCategoryQuery` - zwraca produkty z danej kategorii.

### Zadanie 3: Paginacja
Rozszerz `GetAllItemsQuery` o parametry: `PageNumber`, `PageSize`.

---

## ➡️ Następna Lekcja

**[Lekcja 5: React Native - CRUD dla Słowników](./lekcja-05-react-native-crud-slowniki.md)**

---

**Gratulacje! 🎉 Masz działający CQRS z MediatR!**
