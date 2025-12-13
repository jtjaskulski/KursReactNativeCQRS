# Lekcja 1: TypeScript - Fundamenty

**Moduł:** TypeScript Podstawy  
**Czas trwania:** 3 godziny  
**Poziom:** Początkujący (dla osób znających JavaScript)

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Wyjaśnić czym jest TypeScript i dlaczego go używamy
- ✅ Zainstalować i skonfigurować TypeScript
- ✅ Używać typów podstawowych (string, number, boolean, array, tuple, enum)
- ✅ Tworzyć funkcje z typowaniem parametrów i wartości zwracanej
- ✅ Definiować interfejsy i type aliases
- ✅ Stosować klasy z modyfikatorami dostępu
- ✅ Używać generyków (generics)
- ✅ Pracować z Utility Types (Partial, Required, Pick, Omit)

---

## CZĘŚĆ 1: Wprowadzenie do TypeScript (30 minut)

### 1.1. Czym jest TypeScript?

**SCRIPT dla prowadzącego:**

> „TypeScript to JavaScript z supermocarstwy. Wyobraźcie sobie, że JavaScript to samochód bez deski rozdzielczej - jedzie, ale nie widzicie prędkości, paliwa, niczego. TypeScript dodaje tę deskę rozdzielczą - widzicie błędy ZANIM uruchomicie kod."

**TypeScript** = JavaScript + **System Typów**

TypeScript to język programowania stworzony przez **Microsoft** w 2012 roku, który rozszerza JavaScript o **statyczne typowanie**. Kod TypeScript jest **transpilowany** do czystego JavaScriptu, który może uruchomić każda przeglądarka lub Node.js.

**Kluczowe cechy TypeScript:**

| Cecha | Opis | Korzyść |
|-------|------|---------|
| Statyczne typowanie | Typy sprawdzane przed uruchomieniem | Błędy widoczne w edytorze |
| Autouzupełnianie | IDE wie jakie metody są dostępne | Szybsze pisanie kodu |
| Refaktoryzacja | Bezpieczna zmiana nazw i struktury | Mniej bugów |
| Samodokumentacja | Typy opisują co funkcja przyjmuje/zwraca | Łatwiejsze utrzymanie |
| Kompatybilność | Każdy JS to poprawny TS | Łatwa migracja |

### 1.2. JavaScript vs TypeScript - Przykład

**❌ JavaScript - błąd dopiero w runtime:**

```javascript
// Funkcja do obliczania całkowitej ceny
function calculateTotal(price, quantity) {
  return price * quantity;
}

console.log(calculateTotal(100, 5));        // 500 ✓ OK
console.log(calculateTotal(100, "5"));      // 500 ✓ OK (przypadkowo działa!)
console.log(calculateTotal(100, "five"));   // NaN ✗ BŁĄD w RUNTIME!
console.log(calculateTotal(100));           // NaN ✗ quantity = undefined!

// Te błędy zobaczysz dopiero gdy URUCHOMISZ kod
// W produkcji = crash dla użytkownika!
```

**✅ TypeScript - błąd od razu w edytorze:**

```typescript
// Typowana funkcja - TypeScript wie co przyjmuje i zwraca
function calculateTotal(price: number, quantity: number): number {
  return price * quantity;
}

console.log(calculateTotal(100, 5));        // 500 ✓ OK
console.log(calculateTotal(100, "5"));      // ❌ ERROR: Argument of type 'string' is not assignable
console.log(calculateTotal(100, "five"));   // ❌ ERROR: Argument of type 'string' is not assignable
console.log(calculateTotal(100));           // ❌ ERROR: Expected 2 arguments, but got 1

// Te błędy widzisz W EDYTORZE zanim uruchomisz!
// Czerwona podkreślenie = popraw zanim zbudujesz
```

### 1.3. Proces Kompilacji (Transpilacji)

**SCRIPT dla prowadzącego:**

> „TypeScript nie uruchamia się bezpośrednio. Przeglądarka nie rozumie TypeScript - rozumie tylko JavaScript. Dlatego kod TS musi być przetłumaczony (transpilowany) na JS. W trakcie tego tłumaczenia TypeScript sprawdza wszystkie typy."

```
┌─────────────────────────────────────────────────────────────┐
│                    PROCES TRANSPILACJI                       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────┐                                     │
│  │   app.ts           │  Kod źródłowy TypeScript            │
│  │   (TypeScript)     │  - Typy                             │
│  │                    │  - Interfejsy                       │
│  │   function add(    │  - Nowoczesna składnia              │
│  │     a: number,     │                                     │
│  │     b: number      │                                     │
│  │   ): number {      │                                     │
│  │     return a + b;  │                                     │
│  │   }                │                                     │
│  └─────────┬──────────┘                                     │
│            │                                                 │
│            │  Kompilacja (tsc)                              │
│            │  - Sprawdzenie typów                           │
│            │  - Usunięcie typów                             │
│            │  - Transpilacja do target JS                   │
│            ↓                                                 │
│  ┌────────────────────┐                                     │
│  │   app.js           │  Output JavaScript                  │
│  │   (JavaScript)     │  - Brak typów                       │
│  │                    │  - Czysty JS                        │
│  │   function add(    │  - Gotowy do uruchomienia           │
│  │     a,             │                                     │
│  │     b              │                                     │
│  │   ) {              │                                     │
│  │     return a + b;  │                                     │
│  │   }                │                                     │
│  └─────────┬──────────┘                                     │
│            │                                                 │
│            │  Uruchomienie                                  │
│            ↓                                                 │
│  ┌────────────────────┐                                     │
│  │   Przeglądarka     │                                     │
│  │   Node.js          │                                     │
│  │   React Native     │                                     │
│  └────────────────────┘                                     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

**Kluczowe punkty:**
- Typy istnieją TYLKO w czasie kompilacji
- W runtime (uruchomiony JS) nie ma już typów
- Błędy typów = błąd kompilacji (nie zbudujesz aplikacji)

---

## CZĘŚĆ 2: Instalacja i Konfiguracja (20 minut)

### 2.1. Instalacja Node.js

**Wymagania:**
- Node.js 18+ (LTS)
- npm 9+ lub pnpm 8+

```bash
# Sprawdź czy masz Node.js
node --version    # Powinno być v18.x.x lub nowszy
npm --version     # Powinno być 9.x.x lub nowszy
```

**Jeśli nie masz Node.js:**
1. Pobierz z [nodejs.org](https://nodejs.org) (wersja LTS)
2. Zainstaluj z domyślnymi opcjami
3. Restart terminala

### 2.2. Instalacja TypeScript

**Sposób 1: Globalnie (opcjonalnie)**
```bash
npm install -g typescript
tsc --version     # TypeScript 5.x.x
```

**Sposób 2: Lokalnie w projekcie (ZALECANE)**
```bash
# Utwórz folder projektu
mkdir typescript-learning
cd typescript-learning

# Inicjalizuj package.json
npm init -y

# Zainstaluj TypeScript jako dev dependency
npm install --save-dev typescript

# Sprawdź wersję
npx tsc --version   # TypeScript 5.x.x
```

### 2.3. Konfiguracja tsconfig.json

**SCRIPT dla prowadzącego:**

> „tsconfig.json to serce konfiguracji TypeScript. Określa jak ścisłe jest sprawdzanie typów, do jakiej wersji JS kompilujemy, gdzie są pliki źródłowe i wyjściowe."

**Utworzenie pliku konfiguracji:**
```bash
npx tsc --init
```

**Edytuj tsconfig.json:**
```json
{
  "compilerOptions": {
    // ========== TARGET ==========
    // Do jakiej wersji JS kompilujemy
    "target": "ES2020",
    
    // ========== MODUŁY ==========
    // System modułów (CommonJS dla Node, ESNext dla web)
    "module": "commonjs",
    
    // ========== STRICT MODE ==========
    // Ścisłe sprawdzanie typów - ZAWSZE WŁĄCZONE!
    "strict": true,
    
    // ========== ŚCIEŻKI ==========
    // Folder z kodem źródłowym
    "rootDir": "./src",
    
    // Folder na skompilowane pliki
    "outDir": "./dist",
    
    // ========== INTEROPERABILITY ==========
    // Kompatybilność z ES modules
    "esModuleInterop": true,
    
    // ========== PERFORMANCE ==========
    // Szybsza kompilacja (nie sprawdzaj bibliotek)
    "skipLibCheck": true,
    
    // ========== QUALITY ==========
    // Wymuszaj consistent casing w importach
    "forceConsistentCasingInFileNames": true,
    
    // Nie emituj jeśli są błędy
    "noEmitOnError": true
  },
  // Które pliki kompilować
  "include": ["src/**/*"],
  
  // Które wykluczyć
  "exclude": ["node_modules", "dist"]
}
```

### 2.4. Struktura Projektu

```bash
# Utwórz strukturę folderów
mkdir src
```

**Struktura po setup:**
```
typescript-learning/
├── src/                  # Kod źródłowy TypeScript
│   └── index.ts          # Główny plik
├── dist/                 # Skompilowane pliki JS (generowane)
├── node_modules/         # Zainstalowane pakiety
├── package.json          # Zależności projektu
└── tsconfig.json         # Konfiguracja TypeScript
```

### 2.5. Pierwszy Program

**src/index.ts:**
```typescript
// Typowana funkcja powitania
function greet(name: string): string {
  return `Cześć, ${name}!`;
}

// Wywołanie z poprawnym typem
const message = greet("TypeScript");
console.log(message);  // "Cześć, TypeScript!"

// Próba z niepoprawnym typem - BŁĄD KOMPILACJI!
// const wrongMessage = greet(123);
// Error: Argument of type 'number' is not assignable to parameter of type 'string'
```

**Kompilacja i uruchomienie:**
```bash
# Kompiluj TypeScript do JavaScript
npx tsc

# Uruchom skompilowany JavaScript
node dist/index.js
```

**Wynik:** `Cześć, TypeScript!`

---

## CZĘŚĆ 3: Typy Podstawowe (45 minut)

### 3.1. Typy Prymitywne

**SCRIPT dla prowadzącego:**

> „TypeScript ma kilka podstawowych typów, które odpowiadają typom JavaScript: string, number, boolean. Ale tu mamy pewność - jeśli zmienna jest typu string, to ZAWSZE będzie stringiem."

#### String

```typescript
// Jawna deklaracja typu
let firstName: string = "Anna";
let lastName: string = 'Kowalska';

// Template literals (backticks)
let fullName: string = `${firstName} ${lastName}`;

// Inferencja typu - TypeScript sam wywnioskuje typ
let city = "Warszawa";  // TypeScript wie że to string

// ❌ Błąd - nie możesz przypisać number do string
// firstName = 123;
// Error: Type 'number' is not assignable to type 'string'
```

#### Number

```typescript
// Wszystkie liczby to "number" (int, float, hex, binary)
let age: number = 25;
let price: number = 19.99;
let hex: number = 0xf00d;       // Hexadecimal = 61453
let binary: number = 0b1010;    // Binary = 10
let octal: number = 0o744;      // Octal = 484

console.log(binary);  // 10

// ❌ Błąd
// let wrongAge: number = "dwadzieścia pięć";
```

#### Boolean

```typescript
let isActive: boolean = true;
let hasError: boolean = false;

// Wyniki operacji logicznych
let isAdult: boolean = age >= 18;  // true

// ❌ Błąd - "true" jako string to NIE boolean
// isActive = "true";
// Error: Type 'string' is not assignable to type 'boolean'
```

#### Null i Undefined

```typescript
// W strict mode: null i undefined to osobne typy
let nothing: null = null;
let notDefined: undefined = undefined;

// Union type - zmienna może być string LUB null
let maybeString: string | null = null;
maybeString = "hello";  // ✓ OK
maybeString = null;     // ✓ OK
// maybeString = 123;   // ❌ Error
```

### 3.2. Array (Tablice)

**Dwa sposoby deklaracji:**

```typescript
// ========== SPOSÓB 1: Type[] ==========
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Anna", "Piotr", "Kasia"];

// Metody tablicy są typowane!
numbers.push(6);        // ✓ OK
// numbers.push("7");   // ❌ Error: Argument of type 'string'...

// Dostęp do elementów
console.log(numbers[0]);  // 1 (typ: number)


// ========== SPOSÓB 2: Array<Type> (Generic) ==========
let scores: Array<number> = [90, 85, 88, 92];
let cities: Array<string> = ["Warszawa", "Kraków", "Gdańsk"];


// ========== TABLICE MIESZANE (Union Type) ==========
let mixed: (string | number)[] = ["Anna", 25, "Piotr", 30];
mixed.push("Kasia");  // ✓ OK
mixed.push(35);       // ✓ OK
// mixed.push(true);  // ❌ Error: boolean nie jest w union
```

### 3.3. Tuple (Krotka)

**SCRIPT dla prowadzącego:**

> „Tuple to tablica o STAŁEJ długości i OKREŚLONYCH typach na każdej pozycji. Przydatne gdy funkcja musi zwrócić kilka wartości różnych typów."

```typescript
// Tuple: dokładnie 2 elementy [string, number]
let person: [string, number];
person = ["Anna", 25];  // ✓ OK - string na pozycji 0, number na pozycji 1

// ❌ Błędy:
// person = [25, "Anna"];        // Zła kolejność typów
// person = ["Anna"];            // Za mało elementów
// person = ["Anna", 25, true];  // Za dużo elementów

// Dostęp do elementów (TypeScript wie jaki typ!)
let name = person[0];  // typ: string
let age = person[1];   // typ: number

// Destrukturyzacja
let [personName, personAge] = person;
console.log(personName);  // "Anna"
console.log(personAge);   // 25


// ========== TUPLE Z OPCJONALNYMI ELEMENTAMI ==========
let coordinate: [number, number, number?];  // z opcjonalne
coordinate = [10, 20];       // ✓ OK
coordinate = [10, 20, 30];   // ✓ OK


// ========== NAMED TUPLES (TS 4.0+) ==========
type Point = [x: number, y: number];
const point: Point = [100, 200];
```

### 3.4. Enum (Wyliczenie)

**SCRIPT dla prowadzącego:**

> „Enum to typ z zamkniętym zbiorem wartości. Zamiast pamiętać że status 1 to active, 2 to inactive - używasz czytelnych nazw."

**Enum numeryczny (domyślny):**
```typescript
enum Direction {
  Up,       // 0
  Down,     // 1
  Left,     // 2
  Right     // 3
}

let playerDirection: Direction = Direction.Up;
console.log(playerDirection);  // 0

// Porównanie
if (playerDirection === Direction.Up) {
  console.log("Idzie w górę!");
}
```

**Enum z własnymi wartościami:**
```typescript
enum Status {
  Active = 1,
  Inactive = 0,
  Pending = 2,
  Deleted = -1
}

let userStatus: Status = Status.Active;
console.log(userStatus);  // 1
```

**Enum stringowy (ZALECANE):**
```typescript
// Lepsze do debugowania i logów!
enum Color {
  Red = "RED",
  Green = "GREEN",
  Blue = "BLUE"
}

let favoriteColor: Color = Color.Red;
console.log(favoriteColor);  // "RED" - czytelne w logach!

// Często używane w API
enum HttpStatus {
  OK = "200",
  Created = "201",
  BadRequest = "400",
  NotFound = "404",
  InternalServerError = "500"
}
```

### 3.5. Any (Dowolny typ)

**⚠️ ANY = wyłącza sprawdzanie typów - UNIKAJ!**

```typescript
let anything: any = "hello";
anything = 42;          // ✓ OK
anything = true;        // ✓ OK
anything = { x: 1 };    // ✓ OK
anything.toUpperCase(); // ✓ OK w kompilacji, ale może crashować!

// Użyj TYLKO gdy:
// - Migrujesz z JS do TS (tymczasowo)
// - Integrujesz z biblioteką bez typów
// - Prototypujesz (i zamienisz na prawdziwy typ)
```

### 3.6. Unknown (Bezpieczniejszy Any)

**SCRIPT dla prowadzącego:**

> „Unknown to bezpieczna wersja any. Możesz do niej przypisać cokolwiek, ale żeby użyć wartości - musisz SPRAWDZIĆ jej typ."

```typescript
let userInput: unknown;
userInput = "hello";
userInput = 123;

// ❌ Nie możesz od razu użyć:
// let text: string = userInput;
// Error: Type 'unknown' is not assignable to type 'string'

// ✅ Musisz sprawdzić typ (Type Guard):
if (typeof userInput === "string") {
  let text: string = userInput;  // ✓ OK - TypeScript wie że to string
  console.log(text.toUpperCase());
}

if (typeof userInput === "number") {
  let num: number = userInput;   // ✓ OK
  console.log(num * 2);
}
```

### 3.7. Void (Brak wartości zwracanej)

```typescript
// Funkcja która nic nie zwraca
function logMessage(message: string): void {
  console.log(message);
  // brak 'return' lub 'return;' bez wartości
}

logMessage("Hello TypeScript!");

// void możesz przypisać tylko undefined
let result: void = undefined;
```

### 3.8. Never (Nigdy się nie kończy)

```typescript
// Funkcja która wyrzuca błąd (nigdy nie zwraca)
function throwError(message: string): never {
  throw new Error(message);
}

// Funkcja z nieskończoną pętlą
function infiniteLoop(): never {
  while (true) {
    console.log("Looping...");
  }
}

// Użycie w exhaustiveness checking (switch)
type Shape = "circle" | "square" | "triangle";

function getArea(shape: Shape): number {
  switch (shape) {
    case "circle":
      return 3.14;
    case "square":
      return 1;
    case "triangle":
      return 0.5;
    default:
      // Jeśli dodasz nowy shape, a nie obsłużysz go wyżej
      // TypeScript pokaże błąd tutaj!
      const _exhaustive: never = shape;
      return _exhaustive;
  }
}
```

---

## CZĘŚĆ 4: Funkcje (30 minut)

### 4.1. Typowanie Funkcji

**SCRIPT dla prowadzącego:**

> „W TypeScript typujemy TRZY rzeczy w funkcji: parametry, typ zwracany, i opcjonalnie całą sygnaturę funkcji."

```typescript
// ========== ZWYKŁA FUNKCJA ==========
function add(a: number, b: number): number {
  return a + b;
}

// ========== ARROW FUNCTION ==========
const multiply = (a: number, b: number): number => {
  return a * b;
};

// Skrócony zapis (implicit return)
const divide = (a: number, b: number): number => a / b;

// ========== INFERENCJA TYPU ZWRACANEGO ==========
// TypeScript sam wywnioskuje że zwracamy number
const subtract = (a: number, b: number) => a - b;

// Użycie
console.log(add(5, 3));       // 8
console.log(multiply(5, 3));  // 15
console.log(divide(15, 3));   // 5
```

### 4.2. Parametry Opcjonalne

```typescript
// Parametr opcjonalny oznaczamy ? (musi być na końcu!)
function greet(name: string, greeting?: string): string {
  if (greeting !== undefined) {
    return `${greeting}, ${name}!`;
  }
  return `Cześć, ${name}!`;
}

console.log(greet("Anna"));           // "Cześć, Anna!"
console.log(greet("Piotr", "Witaj")); // "Witaj, Piotr!"

// Wiele opcjonalnych parametrów
function createUser(
  name: string,
  age?: number,
  email?: string
): string {
  let result = `User: ${name}`;
  if (age) result += `, Age: ${age}`;
  if (email) result += `, Email: ${email}`;
  return result;
}
```

### 4.3. Parametry Domyślne

```typescript
// Parametr domyślny - użyty gdy nie przekazano wartości
function introduce(name: string, age: number = 18): string {
  return `${name} ma ${age} lat`;
}

console.log(introduce("Kasia"));        // "Kasia ma 18 lat"
console.log(introduce("Marek", 30));    // "Marek ma 30 lat"

// Domyślna wartość może być wyrażeniem
function createId(prefix: string = "ID", timestamp: number = Date.now()): string {
  return `${prefix}-${timestamp}`;
}
```

### 4.4. Rest Parameters

```typescript
// Rest parameter - przyjmuje dowolną liczbę argumentów jako tablicę
function sum(...numbers: number[]): number {
  return numbers.reduce((total, n) => total + n, 0);
}

console.log(sum(1, 2, 3));           // 6
console.log(sum(10, 20, 30, 40));    // 100
console.log(sum());                   // 0

// Można łączyć z innymi parametrami
function logItems(prefix: string, ...items: string[]): void {
  items.forEach(item => console.log(`${prefix}: ${item}`));
}

logItems("TODO", "Kupić mleko", "Zadzwonić do mamy", "Nauczyć się TS");
```

### 4.5. Function Type (Typ Funkcji)

```typescript
// Deklaracja typu funkcji
let calculate: (x: number, y: number) => number;

// Przypisanie różnych funkcji o tej samej sygnaturze
calculate = (a, b) => a + b;
console.log(calculate(5, 3));  // 8

calculate = (a, b) => a * b;
console.log(calculate(5, 3));  // 15

calculate = (a, b) => a - b;
console.log(calculate(5, 3));  // 2


// ========== CALLBACK JAKO PARAMETR ==========
function processNumbers(
  nums: number[],
  callback: (n: number) => number
): number[] {
  return nums.map(callback);
}

const doubled = processNumbers([1, 2, 3], n => n * 2);
console.log(doubled);  // [2, 4, 6]

const squared = processNumbers([1, 2, 3], n => n * n);
console.log(squared);  // [1, 4, 9]
```

---

## CZĘŚĆ 5: Interfejsy i Type Aliases (30 minut)

### 5.1. Interfejsy

**SCRIPT dla prowadzącego:**

> „Interfejs definiuje KSZTAŁT obiektu. Mówi TypeScriptowi: ten obiekt MUSI mieć te pola z tymi typami."

```typescript
// Definicja interfejsu
interface User {
  id: number;
  name: string;
  email: string;
  isActive: boolean;
}

// Użycie - obiekt MUSI mieć wszystkie pola
const user1: User = {
  id: 1,
  name: "Anna Kowalska",
  email: "anna@example.com",
  isActive: true
};

console.log(user1.name);  // "Anna Kowalska"

// ❌ Błąd - brakuje pola
// const user2: User = {
//   id: 2,
//   name: "Piotr"
// };
// Error: Property 'email' is missing in type...

// ❌ Błąd - nadmiarowe pole
// const user3: User = {
//   id: 3,
//   name: "Kasia",
//   email: "kasia@example.com",
//   isActive: true,
//   age: 25  // Error: 'age' does not exist in type 'User'
// };
```

### 5.2. Pola Opcjonalne

```typescript
interface Product {
  id: number;
  name: string;
  description?: string;  // Opcjonalne - może być undefined
  price: number;
}

// OK - bez description
const product1: Product = {
  id: 1,
  name: "Laptop",
  price: 3000
};

// OK - z description
const product2: Product = {
  id: 2,
  name: "Monitor",
  description: "24 cale Full HD",
  price: 800
};
```

### 5.3. Readonly (Tylko do odczytu)

```typescript
interface Config {
  readonly apiUrl: string;
  readonly timeout: number;
}

const config: Config = {
  apiUrl: "https://api.example.com",
  timeout: 5000
};

// ❌ Nie można zmienić readonly
// config.apiUrl = "https://new-url.com";
// Error: Cannot assign to 'apiUrl' because it is a read-only property
```

### 5.4. Metody w Interfejsie

```typescript
interface Calculator {
  add(a: number, b: number): number;
  subtract(a: number, b: number): number;
  multiply(a: number, b: number): number;
  divide(a: number, b: number): number;
}

const calc: Calculator = {
  add: (a, b) => a + b,
  subtract: (a, b) => a - b,
  multiply: (a, b) => a * b,
  divide: (a, b) => a / b
};

console.log(calc.add(10, 5));      // 15
console.log(calc.subtract(10, 5)); // 5
console.log(calc.multiply(10, 5)); // 50
console.log(calc.divide(10, 5));   // 2
```

### 5.5. Rozszerzanie Interfejsów (extends)

```typescript
// Interfejs bazowy
interface Person {
  name: string;
  age: number;
}

// Rozszerzenie - Employee ma wszystko z Person + dodatkowe pola
interface Employee extends Person {
  companyName: string;
  salary: number;
  department: string;
}

const employee: Employee = {
  name: "Jan Nowak",
  age: 30,
  companyName: "Tech Corp",
  salary: 8000,
  department: "IT"
};

// Wielokrotne rozszerzenie
interface Manager extends Employee {
  teamSize: number;
  isExecutive: boolean;
}
```

### 5.6. Type Alias

**SCRIPT dla prowadzącego:**

> „Type alias to alternatywa dla interfejsu. Główna różnica: interface można rozszerzać przez extends, type alias używa intersection (&). Dla obiektów - wybierz co wolisz. Dla union types - musisz użyć type."

```typescript
// ========== TYPE ALIAS DLA PROSTEGO TYPU ==========
type ID = number;
let userId: ID = 123;

// ========== TYPE ALIAS DLA OBIEKTU ==========
type Point = {
  x: number;
  y: number;
};

const point: Point = { x: 10, y: 20 };

// ========== UNION TYPES (tylko type, nie interface!) ==========
type StringOrNumber = string | number;

let value: StringOrNumber;
value = "hello";  // ✓ OK
value = 42;       // ✓ OK
// value = true;  // ❌ Error

// ========== LITERAL TYPES ==========
type Status = "active" | "inactive" | "pending";

let userStatus: Status;
userStatus = "active";   // ✓ OK
userStatus = "pending";  // ✓ OK
// userStatus = "deleted"; // ❌ Error: Type '"deleted"' is not assignable
```

### 5.7. Interface vs Type Alias

```
┌─────────────────────────────────────────────────────────────┐
│              INTERFACE vs TYPE ALIAS                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  INTERFACE                      TYPE ALIAS                   │
│  ──────────                     ──────────                   │
│  • Rozszerzanie: extends        • Rozszerzanie: &            │
│  • Declaration merging          • Brak mergingu              │
│  • Tylko dla obiektów           • Dla wszystkiego            │
│  • Preferowane dla API          • Preferowane dla union      │
│                                                              │
│  Przykład rozszerzania:                                      │
│                                                              │
│  interface A { x: number }      type A = { x: number }       │
│  interface B extends A {        type B = A & {               │
│    y: number                      y: number                  │
│  }                              }                            │
│                                                              │
│  Użyj interface gdy:            Użyj type gdy:               │
│  • Definiujesz kształt obiektu  • Potrzebujesz union types  │
│  • Tworzysz publiczne API       • Potrzebujesz tuple        │
│  • Chcesz rozszerzać            • Potrzebujesz mapped types │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## CZĘŚĆ 6: Klasy (25 minut)

### 6.1. Podstawowa Klasa

```typescript
class Person {
  // Pola klasy
  name: string;
  age: number;

  // Konstruktor - wywoływany przy tworzeniu instancji
  constructor(name: string, age: number) {
    this.name = name;
    this.age = age;
  }

  // Metoda
  greet(): string {
    return `Cześć, jestem ${this.name}`;
  }

  // Metoda z parametrem
  celebrateBirthday(): void {
    this.age++;
    console.log(`${this.name} ma teraz ${this.age} lat!`);
  }
}

// Tworzenie instancji
const person1 = new Person("Anna", 25);
console.log(person1.greet());        // "Cześć, jestem Anna"
person1.celebrateBirthday();         // "Anna ma teraz 26 lat!"
```

### 6.2. Modyfikatory Dostępu

**SCRIPT dla prowadzącego:**

> „TypeScript ma trzy modyfikatory dostępu: public (domyślny), private (tylko wewnątrz klasy), protected (klasa + dzieci). To pomaga enkapsulować logikę."

```typescript
class BankAccount {
  // PUBLIC - dostępne wszędzie (domyślne)
  public accountNumber: string;
  
  // PRIVATE - dostępne TYLKO wewnątrz tej klasy
  private balance: number;
  
  // PROTECTED - dostępne w klasie i klasach dziedziczących
  protected owner: string;

  constructor(accountNumber: string, initialBalance: number, owner: string) {
    this.accountNumber = accountNumber;
    this.balance = initialBalance;
    this.owner = owner;
  }

  // Publiczna metoda - może być wywołana z zewnątrz
  public getBalance(): number {
    return this.balance;
  }

  // Prywatna metoda - tylko wewnętrznie
  private calculateInterest(): number {
    return this.balance * 0.05;
  }

  // Publiczna metoda używająca prywatnej
  public deposit(amount: number): void {
    if (amount > 0) {
      this.balance += amount;
    }
  }

  public withdraw(amount: number): boolean {
    if (amount > 0 && amount <= this.balance) {
      this.balance -= amount;
      return true;
    }
    return false;
  }
}

const account = new BankAccount("123456", 1000, "Jan");

// ✓ Dostęp do public
console.log(account.accountNumber);  // "123456"
console.log(account.getBalance());   // 1000

// ❌ Brak dostępu do private
// console.log(account.balance);
// Error: Property 'balance' is private

// ❌ Brak dostępu do protected (z zewnątrz)
// console.log(account.owner);
// Error: Property 'owner' is protected
```

### 6.3. Skrócony Zapis Konstruktora

```typescript
// ❌ Długi zapis (tradycyjny)
class UserLong {
  name: string;
  email: string;
  age: number;

  constructor(name: string, email: string, age: number) {
    this.name = name;
    this.email = email;
    this.age = age;
  }
}

// ✅ Skrócony zapis - parametry z modyfikatorem
class UserShort {
  constructor(
    public name: string,
    public email: string,
    public age: number
  ) {}
  // TypeScript automatycznie tworzy pola i przypisuje wartości!
}

const user = new UserShort("Anna", "anna@example.com", 25);
console.log(user.name);   // "Anna"
console.log(user.email);  // "anna@example.com"
console.log(user.age);    // 25
```

### 6.4. Dziedziczenie

```typescript
class Animal {
  constructor(public name: string) {}

  move(): void {
    console.log(`${this.name} się porusza`);
  }
}

class Dog extends Animal {
  constructor(name: string, public breed: string) {
    super(name);  // Wywołanie konstruktora rodzica
  }

  // Nadpisanie metody
  move(): void {
    console.log(`${this.name} biega!`);
  }

  // Nowa metoda
  bark(): void {
    console.log(`${this.name} szczeka: Hau hau!`);
  }
}

const dog = new Dog("Burek", "Labrador");
dog.move();  // "Burek biega!"
dog.bark();  // "Burek szczeka: Hau hau!"
```

---

## CZĘŚĆ 7: Generyki (Generics) (30 minut)

### 7.1. Czym są Generyki?

**SCRIPT dla prowadzącego:**

> „Generyki pozwalają pisać kod który działa z RÓŻNYMI typami, zachowując type safety. Zamiast pisać osobną funkcję dla string[], number[], object[] - piszemy jedną generyczną."

```typescript
// ❌ BEZ generics - duplikacja kodu
function getFirstString(arr: string[]): string | undefined {
  return arr[0];
}

function getFirstNumber(arr: number[]): number | undefined {
  return arr[0];
}

// ✅ Z generics - jedna funkcja dla wszystkich typów
function getFirst<T>(arr: T[]): T | undefined {
  return arr[0];
}

// TypeScript wnioskuje typ z argumentu
const firstNum = getFirst([1, 2, 3]);      // typ: number | undefined
const firstStr = getFirst(["a", "b"]);     // typ: string | undefined
const firstBool = getFirst([true, false]); // typ: boolean | undefined

// Lub jawne podanie typu
const first = getFirst<number>([1, 2, 3]); // typ: number | undefined
```

### 7.2. Generyczna Funkcja - Więcej Przykładów

```typescript
// Funkcja identity - zwraca to co dostanie
function identity<T>(arg: T): T {
  return arg;
}

let output1 = identity("hello");  // string
let output2 = identity(42);       // number
let output3 = identity(true);     // boolean

// Funkcja swap - zamienia elementy tuple
function swap<T, U>(tuple: [T, U]): [U, T] {
  return [tuple[1], tuple[0]];
}

const result = swap([1, "hello"]);  // [string, number] = ["hello", 1]
console.log(result);  // ["hello", 1]
```

### 7.3. Generyczny Interfejs

```typescript
// Interfejs "pudełka" na dowolny typ
interface Box<T> {
  content: T;
  label: string;
}

const stringBox: Box<string> = {
  content: "Hello",
  label: "Greeting"
};

const numberBox: Box<number> = {
  content: 42,
  label: "The Answer"
};

const userBox: Box<{ name: string; age: number }> = {
  content: { name: "Anna", age: 25 },
  label: "User Data"
};


// ========== ODPOWIEDŹ API ==========
interface ApiResponse<T> {
  data: T;
  status: number;
  message: string;
}

// Użycie z różnymi typami danych
interface User {
  id: number;
  name: string;
}

interface Product {
  id: number;
  name: string;
  price: number;
}

const userResponse: ApiResponse<User> = {
  data: { id: 1, name: "Anna" },
  status: 200,
  message: "Success"
};

const productsResponse: ApiResponse<Product[]> = {
  data: [
    { id: 1, name: "Laptop", price: 3000 },
    { id: 2, name: "Phone", price: 2000 }
  ],
  status: 200,
  message: "Success"
};
```

### 7.4. Generyczna Klasa

```typescript
class DataStorage<T> {
  private data: T[] = [];

  addItem(item: T): void {
    this.data.push(item);
  }

  removeItem(item: T): void {
    const index = this.data.indexOf(item);
    if (index !== -1) {
      this.data.splice(index, 1);
    }
  }

  getItems(): T[] {
    return [...this.data];  // Zwraca kopię
  }

  getItem(index: number): T | undefined {
    return this.data[index];
  }
}

// Storage dla liczb
const numberStorage = new DataStorage<number>();
numberStorage.addItem(10);
numberStorage.addItem(20);
numberStorage.addItem(30);
console.log(numberStorage.getItems());  // [10, 20, 30]

// Storage dla stringów
const textStorage = new DataStorage<string>();
textStorage.addItem("Hello");
textStorage.addItem("World");
console.log(textStorage.getItems());  // ["Hello", "World"]
```

### 7.5. Ograniczenia Generyczne (Constraints)

```typescript
// Ograniczenie: T musi mieć property 'length'
interface Lengthwise {
  length: number;
}

function logLength<T extends Lengthwise>(arg: T): void {
  console.log(`Length: ${arg.length}`);
}

logLength("Hello");           // ✓ OK - string ma length
logLength([1, 2, 3]);         // ✓ OK - array ma length
logLength({ length: 10 });    // ✓ OK - obiekt z length

// logLength(123);            // ❌ Error - number nie ma length


// ========== KEYOF CONSTRAINT ==========
function getProperty<T, K extends keyof T>(obj: T, key: K): T[K] {
  return obj[key];
}

const person = { name: "Anna", age: 25, city: "Warsaw" };

const name = getProperty(person, "name");  // string
const age = getProperty(person, "age");    // number

// getProperty(person, "job");  // ❌ Error: "job" nie istnieje w person
```

---

## CZĘŚĆ 8: Utility Types (20 minut)

### 8.1. Partial<T>

**Wszystkie pola stają się opcjonalne.**

```typescript
interface User {
  id: number;
  name: string;
  email: string;
  age: number;
}

// Partial<User> = wszystkie pola opcjonalne
function updateUser(id: number, updates: Partial<User>): void {
  console.log(`Aktualizuję użytkownika ${id}:`, updates);
}

// Możesz przekazać dowolny podzbiór pól
updateUser(1, { name: "Nowa nazwa" });
updateUser(2, { email: "new@example.com", age: 30 });
updateUser(3, {});  // Nawet pusty obiekt OK
```

### 8.2. Required<T>

**Wszystkie pola stają się wymagane.**

```typescript
interface Config {
  apiUrl?: string;
  timeout?: number;
  debug?: boolean;
}

// Required<Config> = wszystkie pola wymagane
const fullConfig: Required<Config> = {
  apiUrl: "https://api.com",
  timeout: 5000,
  debug: true
};

// Musisz podać wszystkie pola!
```

### 8.3. Pick<T, K>

**Wybierz tylko określone pola.**

```typescript
interface User {
  id: number;
  name: string;
  email: string;
  password: string;
  createdAt: Date;
}

// Wybierz tylko id i name
type UserPreview = Pick<User, "id" | "name">;

const preview: UserPreview = {
  id: 1,
  name: "Anna"
  // Nie możesz dodać innych pól!
};
```

### 8.4. Omit<T, K>

**Wyklucz określone pola.**

```typescript
// Wyklucz password z User
type UserWithoutPassword = Omit<User, "password">;

const safeUser: UserWithoutPassword = {
  id: 1,
  name: "Anna",
  email: "anna@example.com",
  createdAt: new Date()
  // password nie istnieje!
};

// Często używane przy wysyłaniu danych do klienta
type PublicUser = Omit<User, "password" | "createdAt">;
```

### 8.5. Record<K, V>

**Obiekt z kluczami typu K i wartościami typu V.**

```typescript
// Mapa statusów
type Status = "pending" | "approved" | "rejected";

const statusLabels: Record<Status, string> = {
  pending: "Oczekujący",
  approved: "Zatwierdzony",
  rejected: "Odrzucony"
};

// Mapa użytkowników po ID
type UsersById = Record<number, User>;

const users: UsersById = {
  1: { id: 1, name: "Anna", email: "a@a.com", password: "xxx", createdAt: new Date() },
  2: { id: 2, name: "Piotr", email: "p@p.com", password: "xxx", createdAt: new Date() }
};
```

### 8.6. Readonly<T>

**Wszystkie pola tylko do odczytu.**

```typescript
interface Settings {
  theme: string;
  language: string;
}

const settings: Readonly<Settings> = {
  theme: "dark",
  language: "pl"
};

// settings.theme = "light";  // ❌ Error: Cannot assign to 'theme'
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Kalkulator
Stwórz interfejs `Calculator` z metodami add, subtract, multiply, divide. Zaimplementuj klasę `BasicCalculator`.

```typescript
// Twój kod tutaj
interface Calculator {
  // ...
}

class BasicCalculator implements Calculator {
  // ...
}
```

### Zadanie 2: Generic Stack
Stwórz generyczną klasę `Stack<T>` z metodami: push, pop, peek, isEmpty, size.

```typescript
// Twój kod tutaj
class Stack<T> {
  // ...
}

const numberStack = new Stack<number>();
numberStack.push(1);
numberStack.push(2);
console.log(numberStack.pop());  // 2
```

### Zadanie 3: User System
Stwórz system typów dla użytkowników z różnymi rolami (admin, user, guest). Użyj union types i type guards.

```typescript
// Twój kod tutaj
type Role = "admin" | "user" | "guest";

interface BaseUser {
  // ...
}

// Różne typy użytkowników...
```

---

## 🔍 Pytania Kontrolne

1. Czym różni się TypeScript od JavaScript?
2. Co to jest inferencja typów?
3. Kiedy użyć `interface`, a kiedy `type`?
4. Czym różni się `any` od `unknown`?
5. Co to są generyki i do czego służą?
6. Wymień 3 Utility Types i ich zastosowanie.
7. Jakie są modyfikatory dostępu w klasach?
8. Co to jest union type?

---

## ➡️ Następna Lekcja

**[Lekcja 2: React Native + TypeScript - Setup i Podstawy](./lekcja-02-react-native.md)**

W następnej lekcji:
- Utworzymy projekt React Native z TypeScript
- Skonfigurujemy środowisko (pnpm, emulator)
- Stworzymy pierwsze komponenty
- Nauczymy się nawigacji (React Navigation)

---

**Gratulacje! 🎉 Ukończyłeś podstawy TypeScript! Teraz przejdziemy do React Native!**
