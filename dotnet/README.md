# SolutionOrdersReact - Backend .NET z CQRS

Backend API dla systemu zamówień zbudowany z:
- **ASP.NET Core 9** (Web API)
- **Entity Framework Core 10** (SQL Server)
- **MediatR 14** (CQRS Pattern)

## 🚀 Szybki Start

### 1. Uruchom SQL Server (Docker)

```bash
cd dotnet
docker-compose up -d
```

### 2. Uruchom API

```bash
cd SolutionOrdersReact.Server
dotnet run
```

### 3. Otwórz Swagger

```
https://localhost:5001/swagger
```

lub

```
http://localhost:5000/swagger
```

## 📁 Struktura Projektu (Vertical Slice Architecture)

```
SolutionOrdersReact.Server/
├── Controllers/           # REST API endpoints
│   ├── ItemsController.cs
│   ├── CategoriesController.cs
│   └── UnitOfMeasurementsController.cs
├── Data/                  # Entity Framework
│   └── ApplicationDbContext.cs
├── Dto/                   # Data Transfer Objects
│   ├── ItemDto.cs
│   ├── CategoryDto.cs
│   └── UnitOfMeasurementDto.cs
├── Features/              # CQRS - Vertical Slices
│   ├── Items/
│   │   ├── Queries/
│   │   │   ├── GetAllItems/
│   │   │   └── GetItemById/
│   │   └── Commands/
│   │       ├── CreateItem/
│   │       ├── UpdateItem/
│   │       └── DeleteItem/
│   ├── Categories/
│   └── UnitOfMeasurements/
└── Models/                # Encje bazy danych
    ├── Item.cs
    ├── Category.cs
    ├── UnitOfMeasurement.cs
    └── Client.cs
```

## 🔌 API Endpoints

### Items (Produkty)
| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET | `/api/items` | Pobiera wszystkie produkty |
| GET | `/api/items/{id}` | Pobiera produkt po ID |
| POST | `/api/items` | Tworzy nowy produkt |
| PUT | `/api/items/{id}` | Aktualizuje produkt |
| DELETE | `/api/items/{id}` | Usuwa produkt (soft delete) |

### Categories (Kategorie)
| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET | `/api/categories` | Pobiera wszystkie kategorie |
| GET | `/api/categories/{id}` | Pobiera kategorię po ID |
| POST | `/api/categories` | Tworzy nową kategorię |
| PUT | `/api/categories/{id}` | Aktualizuje kategorię |
| DELETE | `/api/categories/{id}` | Usuwa kategorię (soft delete) |

### UnitOfMeasurements (Jednostki miary)
| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET | `/api/unitofmeasurements` | Pobiera wszystkie jednostki |
| GET | `/api/unitofmeasurements/{id}` | Pobiera jednostkę po ID |
| POST | `/api/unitofmeasurements` | Tworzy nową jednostkę |
| PUT | `/api/unitofmeasurements/{id}` | Aktualizuje jednostkę |
| DELETE | `/api/unitofmeasurements/{id}` | Usuwa jednostkę (soft delete) |

## 🧪 Przykłady

### Tworzenie produktu
```bash
curl -X POST https://localhost:5001/api/items \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop Dell",
    "description": "Laptop Dell Inspiron 15",
    "idCategory": 1,
    "price": 3500,
    "quantity": 10,
    "idUnitOfMeasurement": 1,
    "code": "LAP001"
  }'
```

## 📦 Seed Data

Baza zawiera przykładowe dane:

**Kategorie:**
- Elektronika
- Żywność
- Odzież

**Jednostki miary:**
- szt (sztuki)
- kg (kilogramy)
- l (litry)

**Produkty:**
- Laptop Dell
- Monitor Samsung
- Mysz Logitech

## 🔧 Konfiguracja

### Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SolutionOrdersDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

## 📚 CQRS Pattern

- **Query** - odczyt danych (nie modyfikuje stanu)
- **Command** - modyfikacja danych (CREATE, UPDATE, DELETE)
- **Handler** - logika biznesowa dla Query/Command
- **MediatR** - dispatcher który łączy Request z Handlerem
