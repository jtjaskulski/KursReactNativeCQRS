# Lekcja 4: Docker + SQL Server + Migracje (2 godziny)

**Moduł:** Infrastruktura  
**Czas trwania:** 2 godziny  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Uruchomić SQL Server w Docker
- ✅ Zarządzać kontenerami (start/stop/logs)
- ✅ Zastosować migracje Entity Framework
- ✅ Debugować problemy z połączeniem
- ✅ Skonfigurować volumes dla trwałości danych

---

## CZĘŚĆ 1: Docker Basics (20 minut)

### 1.1. Czym jest Docker?

**Docker** to platforma do konteneryzacji aplikacji.

**Kontener** = lekka "wirtualna maszyna" z aplikacją i jej zależnościami, która:
- Działa identycznie na każdym środowisku (dev, test, prod)
- Jest odizolowana od systemu hosta
- Startuje w sekundach (nie minutach jak VM)

**Docker vs VM:**
```
┌─────────────────────┐  ┌─────────────────────┐
│   Virtual Machine   │  │      Docker         │
├─────────────────────┤  ├─────────────────────┤
│   App + Libraries   │  │   App + Libraries   │
│   Guest OS (GB!)    │  │   (Tylko App!)      │
├─────────────────────┤  ├─────────────────────┤
│   Hypervisor        │  │   Docker Engine     │
├─────────────────────┤  ├─────────────────────┤
│   Host OS           │  │   Host OS           │
└─────────────────────┘  └─────────────────────┘
```

### 1.2. Instalacja Docker Desktop

**Windows/Mac:**
1. Pobierz [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Zainstaluj z domyślnymi opcjami
3. Uruchom Docker Desktop
4. Sprawdź instalację:

```bash
docker --version      # Docker version 24.x.x
docker-compose --version  # Docker Compose version v2.x.x
```

**Linux:**
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
```

---

## CZĘŚĆ 2: SQL Server w Docker (30 minut)

### 2.1. Utworzenie docker-compose-db.yml

W głównym katalogu projektu **SolutionOrdersReact/** utwórz plik `docker-compose-db.yml`:

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: testReactDb-sqlserver
    environment:
      # Akceptacja licencji
      - ACCEPT_EULA=Y
      
      # Hasło SA (administrator)
      - SA_PASSWORD=YourStrong@Password123
      
      # Edycja SQL Server (Developer = darmowa)
      - MSSQL_PID=Developer
    
    ports:
      # Host:Container
      - "1433:1433"
    
    volumes:
      # Trwałość danych (nie ginie po restart)
      - testReactDb-data:/var/opt/mssql
    
    networks:
      - dev-network
    
    # Restart policy
    restart: unless-stopped
    
    # Health check
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s

networks:
  dev-network:
    driver: bridge

volumes:
  testReactDb-data:
```

**Wyjaśnienie:**
- `image` - obraz Docker (SQL Server 2022)
- `container_name` - nazwa kontenera
- `environment` - zmienne środowiskowe
- `ports` - mapowanie portów (localhost:1433 → container:1433)
- `volumes` - trwałe przechowywanie danych
- `healthcheck` - sprawdza czy SQL Server działa

### 2.2. Uruchomienie Bazy Danych

```bash
# W folderze SolutionOrdersReact/
docker-compose -f docker-compose-db.yml up -d
```

**Flagi:**
- `-f docker-compose-db.yml` - plik konfiguracji
- `-d` - detached mode (w tle)

**Pierwszy raz może potrwać 2-3 minuty** (pobieranie obrazu ~1.5GB).

**Sprawdzenie:**
```bash
docker ps
```

Powinieneś zobaczyć:
```
CONTAINER ID   IMAGE                                        STATUS         PORTS                    NAMES
abc123def456   mcr.microsoft.com/mssql/server:2022-latest   Up 30 seconds  0.0.0.0:1433->1433/tcp   testReactDb-sqlserver
```

### 2.3. Logi Kontenera

**Jeśli coś nie działa:**
```bash
# Zobacz logi
docker logs testReactDb-sqlserver

# Logi na żywo (CTRL+C aby wyjść)
docker logs -f testReactDb-sqlserver
```

**Szukaj w logach:**
```
SQL Server is now ready for client connections.
```

### 2.4. Zatrzymanie i Restart

```bash
# Zatrzymaj kontener (dane zostają!)
docker-compose -f docker-compose-db.yml stop

# Uruchom ponownie
docker-compose -f docker-compose-db.yml start

# Zatrzymaj i usuń kontener (dane w volume zostają!)
docker-compose -f docker-compose-db.yml down

# Usuń WSZYSTKO (włącznie z danymi!)
docker-compose -f docker-compose-db.yml down -v
```

---

## CZĘŚĆ 3: Entity Framework Migrations (40 minut)

### 3.1. Sprawdzenie Connection String

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TestReactDb;User=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Pola:**
- `Server=localhost,1433` - gdzie jest SQL Server
- `Database=TestReactDb` - nazwa bazy (zostanie utworzona)
- `User=sa` - administrator
- `Password=...` - hasło z docker-compose
- `TrustServerCertificate=True` - akceptuj self-signed cert

### 3.2. Utworzenie Pierwszej Migracji

W **Package Manager Console** (Visual Studio):

**⚠️ WAŻNE:** Ustaw **Default project** na `SolutionOrdersReact.Server`!

```powershell
Add-Migration InitialCreate
```

**Co się stanie:**
1. EF Core przeskanuje `ApplicationDbContext`
2. Porówna z obecnym stanem bazy (jeszcze nie istnieje)
3. Wygeneruje kod migracji w folderze `Migrations/`

**Pliki:**
```
Migrations/
├── 20251114_InitialCreate.cs           # Migracja
└── ApplicationDbContextModelSnapshot.cs # Snapshot modelu
```

**Zawartość migracji (przykład):**
```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                IdCategory = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 200, nullable: true),
                Description = table.Column<string>(nullable: true),
                IsActive = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.IdCategory);
            });
        
        // ... więcej tabel
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Categories");
        // ... więcej tabel
    }
}
```

### 3.3. Zastosowanie Migracji

**Sposób 1: Package Manager Console**
```powershell
Update-Database
```

**Sposób 2: .NET CLI**
```bash
dotnet ef database update
```

**Co się stanie:**
1. EF Core połączy się z SQL Server
2. Utworzy bazę danych `TestReactDb` (jeśli nie istnieje)
3. Utworzy wszystkie tabele
4. Wstawi seed data (z `SeedData()`)
5. Utworzy tabelę `__EFMigrationsHistory` (historia migracji)

**Sprawdzenie:**
```powershell
# Lista migracji
Get-Migration

# Status migracji
dotnet ef migrations list
```

### 3.4. Automatyczne Migracje przy Starcie

**Program.cs** (już dodane w Lekcji 3):

```csharp
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate(); // ← Auto-migrate!
            
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Baza danych zmigrowana pomyślnie");
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Błąd podczas migracji bazy danych");
        }
    }
}
```

**⚠️ UWAGA:** To tylko dla DEV! Na produkcji migracje robimy ręcznie.

---

## CZĘŚĆ 4: Weryfikacja i Debugowanie (30 minut)

### 4.1. Podłączenie do SQL Server (SSMS/Azure Data Studio)

**Azure Data Studio (ZALECANE - darmowe, cross-platform):**
1. Pobierz [Azure Data Studio](https://aka.ms/azuredatastudio)
2. Otwórz i kliknij "New Connection"
3. Ustaw:
   - **Server:** `localhost,1433`
   - **Authentication:** SQL Login
   - **User:** `sa`
   - **Password:** `YourStrong@Password123`
4. Kliknij "Connect"

**SQL Server Management Studio (Windows):**
- Server name: `localhost,1433`
- Authentication: SQL Server Authentication
- Login: `sa`
- Password: `YourStrong@Password123`

### 4.2. Sprawdzenie Tabel

```sql
-- Lista tabel
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Powinno być:
-- Categories
-- Clients
-- Items
-- OrderItems
-- Orders
-- UnitOfMeasurements
-- Workers
-- __EFMigrationsHistory

-- Sprawdź dane seed
SELECT * FROM Categories;
SELECT * FROM UnitOfMeasurements;
SELECT * FROM Clients;
```

### 4.3. Najczęstsze Problemy

**Problem 1: "A network-related error occurred"**

**Przyczyny:**
- Docker nie działa
- Kontener SQL Server nie startował
- Zły port

**Rozwiązanie:**
```bash
# Sprawdź czy kontener działa
docker ps

# Jeśli nie ma - zobacz logi
docker logs testReactDb-sqlserver

# Restart
docker-compose -f docker-compose-db.yml restart
```

**Problem 2: "Login failed for user 'sa'"**

**Przyczyny:**
- Złe hasło w connection string
- Hasło w docker-compose inne niż w appsettings.json

**Rozwiązanie:**
Upewnij się że hasło jest TAKIE SAMO w obu miejscach.

**Problem 3: "Cannot create file ... because it already exists"**

**Przyczyna:** Baza już istnieje, ale migracje się nie zgadzają

**Rozwiązanie:**
```powershell
# Usuń bazę i utwórz od nowa
Drop-Database  # lub ręcznie w SSMS
Update-Database
```

### 4.4. Testowanie API

**Uruchom aplikację:** F5 w Visual Studio

**Otwórz Swagger:**
`https://localhost:7xxx/swagger`

**Sprawdź endpointy:**
- GET `/api/items` - powinno zwrócić seed data
- GET `/api/categories` - powinno zwrócić kategorie

---

## CZĘŚĆ 5: Docker Compose dla Całej Aplikacji (opcjonalnie, 30 min)

### 5.1. Dockerfile

W głównym katalogu **SolutionOrdersReact/** utwórz `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Instalacja Node.js dla React
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install -y nodejs

# Kopiowanie plików projektu
COPY ["SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj", "SolutionOrdersReact.Server/"]
COPY ["solutionordersreact.client/solutionordersreact.client.esproj", "solutionordersreact.client/"]
COPY ["solutionordersreact.client/package.json", "solutionordersreact.client/"]

# Restore
RUN dotnet restore "SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj"

# Kopiowanie reszty kodu
COPY . .

# Instalacja npm dla frontendu
WORKDIR "/src/solutionordersreact.client"
RUN npm install

# Build Server
WORKDIR "/src/SolutionOrdersReact.Server"
RUN dotnet build "SolutionOrdersReact.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SolutionOrdersReact.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolutionOrdersReact.Server.dll"]
```

### 5.2. docker-compose.yml (API + DB)

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: orders-react-api
    ports:
      - "5000:8080"
      - "5001:8081"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080;https://+:8081
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=TestReactDb;User=sa;Password=YourStrong@Password123;TrustServerCertificate=True
    depends_on:
      sqlserver:
        condition: service_healthy
    networks:
      - orders-network

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: orders-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - orders-network
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s

networks:
  orders-network:
    driver: bridge

volumes:
  sqlserver-data:
```

### 5.3. .dockerignore

```
**/bin
**/obj
**/node_modules
**/.vs
**/.vscode
**/TestResults
**/*.user
**/*.suo
**/logs
**/npm-debug.log
**/dist
**/.DS_Store
**/Dockerfile
**/docker-compose*.yml
**/.dockerignore
```

### 5.4. Uruchomienie Całości

```bash
# Build i uruchomienie
docker-compose up --build

# W tle
docker-compose up -d --build

# Logi
docker-compose logs -f

# Zatrzymanie
docker-compose down
```

API będzie dostępne: `http://localhost:5000`

---

## 📝 Zadania Praktyczne

### Zadanie 1: Backup Bazy
Utwórz backup bazy danych i przywróć go na nowym kontenerze.

### Zadanie 2: Druga Migracja
Dodaj nową kolumnę `Email` do tabeli `Worker` i utwórz migrację.

### Zadanie 3: Production Compose
Stwórz osobny `docker-compose.prod.yml` z HTTPS i tajnym hasłem z secrets.

---

## 🔍 Pytania Kontrolne

1. Czym różni się Docker od VM?
2. Co to jest volume i dlaczego go używamy?
3. Jak sprawdzić logi kontenera?
4. Co robi `Database.Migrate()`?
5. Dlaczego nie robimy auto-migrate na produkcji?

---

## ➡️ Następna Lekcja

**[Lekcja 5: React Native - CRUD dla Słowników](./lekcja-05-react-native-crud-slowniki.md)**

W następnej lekcji:
- Połączymy React Native z API
- Stworzymy API Service z TypeScript
- Zaimplementujemy CRUD dla UnitOfMeasurement
- Zbudujemy reużywalny komponent SimpleCrudList

---

**Gratulacje! 🎉 Masz działającą infrastrukturę!**
