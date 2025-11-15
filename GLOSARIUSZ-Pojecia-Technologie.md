# GLOSARIUSZ: React Native + TypeScript + .NET CQRS

**Kompletny słowniczek pojęć, technologii i wzorców z kursu**

---

## A

### Activity (Android)
Podstawowa jednostka aplikacji Android. Ekran z interfejsem użytkownika. W React Native abstrahowany przez `Screen`.

### API (Application Programming Interface)
Interfejs komunikacji między aplikacją mobilną a serwerem. W naszym kursie: RESTful API backendowe w `.NET`.

### APK (Android Package Kit)
Format pliku do dystrybucji aplikacji Android. Odpowiednik `.exe` na Androidzie. Budujemy go komendą `./gradlew assembleRelease`.

### AsyncStorage
React Native biblioteka do przechowywania danych lokalnie na urządzeniu (jak localStorage w przeglądarce). Asynchroniczna.

### Async/Await
Syntaktyka JavaScript do obsługi operacji asynchronicznych. Zamiast `.then()` - czystszy kod.

```typescript
async function fetchData() {
  const data = await fetch('/api/items').then(r => r.json());
  return data;
}
```

---

## B

### Backend
Część serwerowa aplikacji (logika biznesowa, baza danych). W kursie: **ASP.NET Core 8 z CQRS**.

### Behavior (MediatR)
Pośredni handler w pipeline MediatR (np. ValidationBehavior, LoggingBehavior). Wykonuje się przed głównym handlem.

### Binding
Powiązanie danych z interfejsem. W React Native - automatyczne przy state changes.

### Bootstrap
Startup aplikacji - wczytanie konfiguracji, DI container, baza danych.

---

## C

### CLI (Command Line Interface)
Interfejs wiersza poleceń. W kursie: `react-native-cli`, `dotnet-cli`.

### Command (CQRS)
Request zmieniający stan systemu (CREATE, UPDATE, DELETE). Zwraca wynik (ID, status).

```csharp
public class CreateItemCommand : IRequest<int> { ... }
```

### Component (React)
Blok kodu renderujący UI. Funkcyjny (`React.FC`) lub klasowy.

```tsx
const Greeting: React.FC<Props> = ({ name }) => <Text>{name}</Text>;
```

### Constructor
Metoda wywoływana przy tworzeniu obiektu. W C# - inicjalizacja pól. W React Native - NIE UŻYWAMY (hooks zamiast tego).

### Controller (ASP.NET)
Klasa obsługująca HTTP requesty. Punkt wejścia do API.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase { ... }
```

### Context (React)
Globalne state dla całej aplikacji (bez prop drilling). W kursie: `ItemsContext`, `AuthContext`.

### CRUD
Create (POST), Read (GET), Update (PUT), Delete (DELETE) - podstawowe operacje na danych.

### CQRS (Command Query Responsibility Segregation)
Wzorzec architektoniczny rozdzielający Commands (zapis) od Queries (odczyt).

**Zalety:**
- Separation of Concerns
- Lepsze skalowanie
- Łatwiejsze testowanie

---

## D

### DTO (Data Transfer Object)
Model przesyłany przez API (vs encja w bazie). Mapuje dane do odpowiedniego formatu.

```csharp
public class ItemDto {
  public int Id { get; set; }
  public string Name { get; set; }
}
```

### DbContext (EF Core)
Klasa reprezentująca sesję z bazą danych. Śledzenie zmian, SaveChanges().

```csharp
public class ApplicationDbContext : DbContext {
  public DbSet<Item> Items { get; set; }
}
```

### Dependency Injection (DI)
Wstrzykiwanie zależności zamiast `new()`. Luźniejsze powiązanie, łatwiejsze testowanie.

```csharp
public ItemsController(IMediator mediator) => _mediator = mediator;
```

### DevTools
Narzędzia debugowania React Native (React DevTools, Flipper).

### Dispatch
Wysyłanie akcji do reducer'a (Redux style). W MediatR: `_mediator.Send()`.

---

## E

### EF Core (Entity Framework Core)
ORM (Object-Relational Mapping) dla .NET. Mapuje obiekty C# na tabele SQL.

### Emulator
Wirtualne urządzenie mobilne (Android Emulator, iOS Simulator). Testujemy na nim bez fizycznego telefonu.

### Enum (Enumeration)
Typ z zdefiniowanym zbiorem wartości.

```typescript
enum Status {
  Active = "ACTIVE",
  Inactive = "INACTIVE"
}
```

### Event
Zdarzenie (np. klik, zmiana tekstu). Handler obsługuje event: `onPress`, `onChange`.

---

## F

### fetch API
Wbudowana funkcja do HTTP requestów. Zwraca Promise.

```typescript
const response = await fetch('/api/items');
const data = await response.json();
```

### Flexbox
System layoutu (CSS, React Native). Rozmieszczanie elementów w rzędach/kolumnach.

```typescript
container: {
  flexDirection: 'row',
  justifyContent: 'space-between',
}
```

### FlatList
Wydajny komponent do renderowania list w React Native. Virtualizuje (renderuje tylko widoczne).

### Frontend
Część kliencka aplikacji (UI, interakcje). W kursie: **React Native na mobilce**.

### Function Component
Komponent jako zwykła funkcja zwracająca JSX. Nowoczesne podejście (zamiast Class Components).

---

## G

### Generics (TypeScript/C#)
Parametryczne typy. Komponenty/funkcje pracują z dowolnym typem.

```typescript
function identity<T>(arg: T): T { return arg; }
function DataStorage<T> { ... }
```

### Gradle
Build system dla Android. Kompiluje Java/Kotlin na APK.

### Graphical User Interface (GUI)
Interfejs użytkownika - przyciski, teksty, input fieldy.

---

## H

### Handler (MediatR)
Funkcja obsługująca Command/Query. Zawiera logikę biznesową.

```csharp
public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, List<ItemDto>> {
  public async Task<List<ItemDto>> Handle(GetAllItemsQuery request, CancellationToken ct) { ... }
}
```

### Hot Reload
Przeładowanie kodu bez restartowania aplikacji. Zmiany widać natychmiast.

### Hook (React)
Funkcje w komponentach: `useState`, `useEffect`, `useContext`. Zarządzanie state i lifecycle.

---

## I

### IPA (iOS App Archive)
Format pliku do dystrybucji aplikacji iOS (odpowiednik APK dla Apple).

### Interface (TypeScript/C#)
Kontrakt definiujący strukturę obiektu. Zmusza do implementacji określonych properties/metod.

```typescript
interface User {
  id: number;
  name: string;
}
```

### Intent (Android)
Wiadomość między komponentami Android (jak nawigacja). W React Native: `react-navigation`.

### Injection
Wstrzykiwanie zależności w konstruktorze (vs `new` bezpośrednio).

---

## J

### JavaScript
Język programowania dla web/mobilki. React Native to JavaScript dla mobilnych platform.

### JSX
Składnia HTML-like w JavaScript/TypeScript. Renderuje UI.

```tsx
<Text style={styles.title}>Hello</Text>
```

### JSON (JavaScript Object Notation)
Format wymiany danych między API a klientem.

```json
{ "id": 1, "name": "Laptop", "price": 3000 }
```

---

## K

### Kotlin
Język dla Android (zamiennik Java). W React Native abstrahowany - piszemy TypeScript.

### keyExtractor (FlatList)
Funkcja zwracająca unikalny klucz dla każdego elementu listy. Ważne dla performance.

```tsx
keyExtractor={(item) => item.id.toString()}
```

---

## L

### Lifecycle
Cykl życia komponentu: mount → update → unmount. Obsługujemy via `useEffect()`.

### Lint/Linter
Narzędzie sprawdzające błędy kodu (ESLint). Wymusza code style.

### Localhost
Twoja maszyna. W React Native emulator != localhost (10.0.2.2 dla Android).

---

## M

### MediatR
Biblioteka implementująca wzorzec Mediatora w .NET. Wysyłamy Request → Handler.

```csharp
var result = await _mediator.Send(new GetAllItemsQuery());
```

### Method (metoda)
Funkcja wewnątrz klasy. `GetAllItems()`, `CreateItem()`.

### Migration (EF Core)
Historia zmian schematu bazy danych. `Add-Migration InitialCreate` → `Update-Database`.

### Model
Struktura danych. W EF Core: klasa mapująca się na tabelę. W React: dane displayane w UI.

```csharp
public class Item {
  public int Id { get; set; }
  public string Name { get; set; }
}
```

### Module (npm)
Pakiet z kodu (np. `react-native`, `@react-navigation/native`). Instalujemy via `pnpm install`.

---

## N

### Native (natively)
Natywnie dla platformy. Kod Android (Java/Kotlin), iOS (Swift). React Native abstrahuje - piszemy raz.

### Navigation
Przechodzenie między ekranami. Stack Navigator, Tab Navigator, Drawer Navigator.

### NuGet
Package manager dla .NET (odpowiednik npm). Instalujemy pakiety: `MediatR`, `EntityFrameworkCore`.

---

## O

### Obfuscation
Zaciemnianie kodu (zmienne/funkcje → `a`, `b`). Na produkcji - ochrona kodu.

### ORM (Object-Relational Mapping)
Mapowanie obiektów na tabele SQL. EF Core robi to za nas.

### Overflow
Zawartość przekracza dostępną przestrzeń. `ScrollView` załatwia.

---

## P

### Package.json
Plik z listą zależności projektu. Wersje pakietów, skrypty (`pnpm start`).

### Pagination
Dzielenie dużych list na strony. Query: `PageNumber`, `PageSize`.

```csharp
public class GetAllItemsQuery : IRequest<PaginatedList<ItemDto>> {
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
```

### Picker
Komponent do wyboru z listy (dropdown, select). W React Native: `<Picker>`.

### Pipeline (MediatR)
Łańcuch Behaviors: Request → Validation → Logging → Handler → Response.

### Property (właściwość)
Pole klasy z getterem/setterem. W C#: `{ get; set; }`.

### Props
Parametry komponentu (read-only dane przekazane od rodzica).

```tsx
<Greeting name="Anna" age={25} />
```

### Protocol (network)
HTTP, HTTPS, WebSocket - sposoby komunikacji. API REST używa HTTP/HTTPS.

---

## Q

### Query (CQRS)
Request do odczytu danych (GET). Nie zmienia stanu.

```csharp
public class GetAllItemsQuery : IRequest<List<ItemDto>> { }
```

### Queue (kolejka)
FIFO struktura (First In First Out). Asynchroniczne tasking, event loop.

---

## R

### React
Biblioteka do budowania UI. Virtual DOM, komponenty, state management.

### React Native
React dla mobilki (iOS + Android). Jeden kod → dwie aplikacje.

**Architektura:**
```
React Native (TypeScript)
    ↓
JavaScript Engine (Hermes/JavaScriptCore)
    ↓
Native Code (Java/Swift)
    ↓
Android/iOS Platform
```

### Reducer
Funkcja: (state, action) → newState. Redux pattern (w kursie: Context API zamiast).

### Ref
Bezpośredni dostęp do elementu DOM/Native. `useRef()` - rzadko używamy.

### Reload
Przeładowanie - zamknięcie i otwarcie aplikacji. (vs Hot Reload - bez zamykania).

### REST API
RESTful API - HTTP endpoints zwracające JSON. GET /items, POST /items, etc.

### Render
Rysowanie UI na ekranie. Reaction na state/props changes.

---

## S

### Scope (DI Container)
Czas życia obiektu. Singleton (cały czas), Transient (za każdym razem), Scoped (per request).

```csharp
builder.Services.AddTransient<IItemService, ItemService>();
```

### ScrollView
Komponent scrollujący zawartość. Dla małych ilości (vs FlatList dla dużych).

### Separator
Linia dzieląca elementy listy. `ItemSeparatorComponent` w FlatList.

### Shell (CLI)
Powłoka poleceń. Bash, PowerShell, Zsh. Wpisujemy komendy.

### Snapshot (Testing)
Zrzut stanu komponentu - porównanie zmian.

### SQL Server
Relacyjna baza danych od Microsoftu. W kursie w Docker.

### State
Dynamiczne dane komponentu. `useState()` - zmiana state → re-render.

```typescript
const [count, setCount] = useState<number>(0);
```

### StyleSheet
Tworzenie zoptymalizowanych stylów React Native (zamiast inline).

```typescript
const styles = StyleSheet.create({
  container: { flex: 1 }
});
```

### Subscription
Słuchanie na zmiany (event listener). Kiedyś: `addEventListener`, teraz: hooks.

### Syntax
Reguły pisania kodu w języku (JS, TS, C#).

---

## T

### Template
Szablonowy projekt startowy. React Native 0.82 ma TypeScript wbudowany (nie potrzebuje template'u).

### TextInput
Komponenta do wpisywania tekstu. `<TextInput placeholder="..." />`.

### Theme
Zestaw kolorów, fontów, spacing. Consistency UI.

### Thread
Wątek wykonania. Aplikacja może mieć wiele threadów (UI thread, background thread).

### Type (TypeScript)
Typ zmiennej: `string`, `number`, `boolean`, custom interfaces.

### Type Annotation
Podanie typu zmiennej.

```typescript
let name: string = "Anna";
let age: number = 25;
```

### TypeScript
JavaScript z typami. Statyczne typowanie - błędy w edytorze (vs runtime).

---

## U

### UI (User Interface)
Interfejs użytkownika - wszystko co widzi user. Buttons, texts, inputs.

### Union Type
Typ może być jedną z wielu opcji.

```typescript
type Status = "active" | "inactive" | "pending";
```

### Update-Database
Komenda EF Core: `Update-Database`. Aplikuje migracje do SQL.

### UX (User Experience)
Doświadczenie użytkownika - flow, responsywność, intuicyjność.

---

## V

### Validator (FluentValidation)
Klasa do walidacji danych Commands. Reguły: `NotEmpty()`, `MaximumLength()`.

```csharp
public class CreateItemValidator : AbstractValidator<CreateItemCommand> {
  public CreateItemValidator() {
    RuleFor(x => x.Name).NotEmpty();
  }
}
```

### Virtual DOM
Reprezentacja UI w pamięci. React porównuje i updatuje tylko zmienione.

### View
Komponent kontenerowy. Odpowiada HTML `<div>`.

```tsx
<View style={styles.container}>
  <Text>Hello</Text>
</View>
```

---

## W

### Webpack
Bundler modułów JavaScript (vs Metro dla React Native).

### Widget
Komponent UI (Button, Text, Input).

---

## X

### XCode
IDE dla iOS (Apple). Budujemy IPA, symulator.

---

## Y

### YAML
Format konfiguracji (docker-compose.yml). Hierarchiczny, czytelny.

```yaml
version: '3.8'
services:
  api:
    image: myapi:latest
```

---

## Z

### Zustand / Redux / Jotai
State management biblioteki. W kursie: Context API zamiast (prostsze).

---

## SKRÓTY

| Skrót | Pełna nazwa | Opis |
|-------|-----------|------|
| **API** | Application Programming Interface | Interfejs komunikacji |
| **APK** | Android Package Kit | Plik do instalacji Android |
| **CLI** | Command Line Interface | Wiersz poleceń |
| **CRUD** | Create Read Update Delete | Podstawowe operacje |
| **CQRS** | Command Query Responsibility Segregation | Wzorzec architektoniczny |
| **DI** | Dependency Injection | Wstrzykiwanie zależności |
| **DTO** | Data Transfer Object | Model API |
| **EF** | Entity Framework | ORM dla .NET |
| **FPS** | Frames Per Second | Płynność animacji |
| **HTTP** | HyperText Transfer Protocol | Protokół web |
| **IDE** | Integrated Development Environment | Edytor kodu |
| **IoC** | Inversion of Control | Kontener DI |
| **IPA** | iOS App Archive | Plik do instalacji iOS |
| **JSON** | JavaScript Object Notation | Format danych |
| **JWT** | JSON Web Token | Token autentykacji |
| **ORM** | Object-Relational Mapping | Mapowanie obiekt↔SQL |
| **REST** | Representational State Transfer | Styl API |
| **SQL** | Structured Query Language | Zapytania do bazy |
| **SSR** | Server-Side Rendering | Rendering na serwerze |
| **UI** | User Interface | Interfejs użytkownika |
| **UX** | User Experience | Doświadczenie użytkownika |
| **VCS** | Version Control System | Git, GitHub |
| **VM** | Virtual Machine | Maszyna wirtualna |

---

## TECHNOLOGIE W KURSIE

### Frontend
- **React Native** 0.82+ - Framework mobilny
- **TypeScript** - Typowany JavaScript
- **React Navigation** - Nawigacja między ekranami
- **@react-native-async-storage/async-storage** - Local storage
- **pnpm** - Package manager

### Backend
- **ASP.NET Core 8** - Framework .NET
- **Entity Framework Core** - ORM
- **MediatR** - Mediator pattern
- **FluentValidation** - Walidacja
- **SQL Server** - Baza danych

### DevOps
- **Docker** - Konteneryzacja
- **docker-compose** - Orkiestracja kontenerów
- **Gradle** - Build system Android

---

## ŚCIEŻKA NAUKI

```
Lekcja 1: TypeScript
    ↓ (3h)
Lekcja 2: React Native Setup
    ↓ (4h)
Lekcja 3-3.3: .NET CQRS Backend
    ↓ (6h)
Lekcja 4: Docker + SQL
    ↓ (2h)
Lekcja 5: Integracja Mobile ↔ API
    ↓ (3h)
Lekcja 6-7: Relacje, zamówienia
    ↓ (5h)
Lekcja 8-9: Walidacja, zaawansowane
    ↓ (4h)
Lekcja 10-11: Native, deployment
    ↓ (4h)
KONIEC: Aplikacja produkcyjna! 🚀
```

---

## LINKI DOKUMENTACJI

- React Native: https://reactnative.dev
- TypeScript: https://www.typescriptlang.org
- .NET: https://learn.microsoft.com/en-us/dotnet
- Entity Framework: https://learn.microsoft.com/en-us/ef/core
- MediatR: https://github.com/jbogard/MediatR
- React Navigation: https://reactnavigation.org
- FluentValidation: https://fluentvalidation.net
- Docker: https://docs.docker.com

---

**Glosariusz Complete! 📚 Używaj jako referencji podczas kursu!**
