# Lekcja 1: TypeScript - Fundamenty (3 godziny)

**Moduł:** TypeScript Podstawy  
**Czas trwania:** 3 godziny  
**Poziom:** Początkujący (dla osób znających JavaScript)

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Wyjaśnić czym jest TypeScript i dlaczego go używamy
- ✅ Zainstalować i skonfigurować TypeScript
- ✅ Używać typów podstawowych (string, number, boolean, array, tuple, enum)
- ✅ Tworzyć funkcje z typowaniem
- ✅ Definiować interfejsy i type aliases
- ✅ Stosować klasy z modyfikatorami dostępu
- ✅ Używać generyków (generics)
- ✅ Pracować z Utility Types

---

## CZĘŚĆ 1: Wprowadzenie do TypeScript (30 minut)

### 1.1. Czym jest TypeScript?

**TypeScript** = JavaScript + **System Typów**

TypeScript to język programowania stworzony przez Microsoft w 2012 roku, który rozszerza JavaScript o **statyczne typowanie**. Kod TypeScript jest transpilowany do czystego JavaScriptu, który może uruchomić każda przeglądarka lub Node.js.

**Kluczowe cechy:**
- ✅ **Statyczne typowanie** - błędy wykrywane przed uruchomieniem
- ✅ **Lepsze autouzupełnianie** w IDE (IntelliSense)
- ✅ **Refaktoryzacja** bezpieczniejsza
- ✅ **Samodokumentujący się kod** - typy jako dokumentacja
- ✅ **100% kompatybilny z JavaScript** - każdy JS to poprawny TS

### 1.2. JavaScript vs TypeScript - Przykład

**❌ JavaScript - błąd dopiero w runtime:**
```javascript
function calculateTotal(price, quantity) {
  return price * quantity;
}

console.log(calculateTotal(100, 5));        // 500 ✓
console.log(calculateTotal(100, "5"));      // 500 ✓ (ale to string!)
console.log(calculateTotal(100, "five"));   // NaN ✗ BŁĄD!
```

**✅ TypeScript - błąd od razu w edytorze:**
```typescript
function calculateTotal(price: number, quantity: number): number {
  return price * quantity;
}

console.log(calculateTotal(100, 5));        // 500 ✓
console.log(calculateTotal(100, "5"));      // ❌ ERROR: Argument of type 'string'...
console.log(calculateTotal(100, "five"));   // ❌ ERROR: Argument of type 'string'...
```

### 1.3. Proces Kompilacji

```
┌──────────────────┐
│  TypeScript      │  app.ts (kod źródłowy)
│  Kod + Typy      │
└────────┬─────────┘
         │
         │ Transpilacja (tsc)
         │ - Sprawdzenie typów
         │ - Usunięcie typów
         ↓
┌──────────────────┐
│  JavaScript      │  app.js (output)
│  Tylko kod       │
└────────┬─────────┘
         │
         │ Uruchomienie
         ↓
┌──────────────────┐
│  Przeglądarka    │
│  Node.js         │
└──────────────────┘
```

---

## CZĘŚĆ 2: Instalacja i Konfiguracja (20 minut)

### 2.1. Instalacja Node.js

1. Pobierz Node.js LTS z [nodejs.org](https://nodejs.org)
2. Zainstaluj z domyślnymi opcjami
3. Sprawdź instalację:

```bash
node --version    # v18.x.x lub nowszy
npm --version     # 9.x.x lub nowszy
```

### 2.2. Instalacja TypeScript

**Globalnie (opcjonalnie):**
```bash
npm install -g typescript
tsc --version     # 5.x.x
```

**Lokalnie w projekcie (ZALECANE):**
```bash
mkdir typescript-learning
cd typescript-learning
npm init -y
npm install --save-dev typescript
```

### 2.3. Konfiguracja tsconfig.json

Utworzenie pliku konfiguracji:
```bash
npx tsc --init
```

**Edytuj tsconfig.json:**
```json
{
  "compilerOptions": {
    /* Wersja JavaScript do której kompilujemy */
    "target": "ES2020",
    
    /* System modułów */
    "module": "commonjs",
    
    /*Ścisłe sprawdzanie typów */
    "strict": true,
    
    /* Gdzie zapisać skompilowane pliki */
    "outDir": "./dist",
    
    /* Gdzie są pliki źródłowe */
    "rootDir": "./src",
    
    /* Kompatybilność z ES modules */
    "esModuleInterop": true,
    
    /* Szybsza kompilacja */
    "skipLibCheck": true,
    
    /* Sprawdzaj wielkość liter w nazwach plików */
    "forceConsistentCasingInFileNames": true
  },
  "include": ["src/**/*"],
  "exclude": ["node_modules", "dist"]
}
```

### 2.4. Struktura projektu

```bash
mkdir src
touch src/index.ts
```

Struktura:
```
typescript-learning/
├── src/
│   └── index.ts
├── dist/              (wygenerowane przez tsc)
├── node_modules/
├── package.json
└── tsconfig.json
```

### 2.5. Pierwszy program

**src/index.ts:**
```typescript
function greet(name: string): string {
  return `Cześć, ${name}!`;
}

const message = greet("TypeScript");
console.log(message);

// Błąd - próba przekazania number zamiast string
// const wrongMessage = greet(123); // ❌ ERROR
```

**Kompilacja i uruchomienie:**
```bash
npx tsc                    # Kompiluje wszystkie pliki .ts
node dist/index.js         # Uruchamia skompilowany JS
```

**Wynik:** `Cześć, TypeScript!`

---

## CZĘŚĆ 3: Typy Podstawowe (45 minut)

### 3.1. Typy Prymitywne

#### String
```typescript
let firstName: string = "Anna";
let lastName: string = 'Kowalska';
let fullName: string = `${firstName} ${lastName}`;

// ❌ Błąd:
// firstName = 123; // ERROR: Type 'number' is not assignable to type 'string'
```

#### Number
```typescript
let age: number = 25;
let price: number = 19.99;
let hex: number = 0xf00d;       // Hexadecimal
let binary: number = 0b1010;    // Binary = 10
let octal: number = 0o744;      // Octal = 484

console.log(binary); // 10
```

#### Boolean
```typescript
let isActive: boolean = true;
let hasError: boolean = false;

// ❌ Błąd:
// isActive = "true"; // ERROR: Type 'string' is not assignable
```

#### Null i Undefined
```typescript
let nothing: null = null;
let notDefined: undefined = undefined;

// W strict mode: null i undefined to osobne typy
let maybeString: string | null = null;  // OK
maybeString = "hello";                   // OK
```

### 3.2. Array (Tablice)

**Sposób 1: Type[]**
```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Anna", "Piotr", "Kasia"];

numbers.push(6);        // ✓ OK
// numbers.push("7");   // ❌ ERROR

console.log(numbers[0]); // 1
```

**Sposób 2: Array<Type> (Generic)**
```typescript
let scores: Array<number> = [90, 85, 88, 92];
let cities: Array<string> = ["Warszawa", "Kraków", "Gdańsk"];
```

**Tablice mieszane (Union Type):**
```typescript
let mixed: (string | number)[] = ["Anna", 25, "Piotr", 30];
mixed.push("Kasia");   // ✓ OK
mixed.push(35);        // ✓ OK
// mixed.push(true);   // ❌ ERROR
```

### 3.3. Tuple (Krotka)

**Tuple = tablica o stałej długości i określonych typach**

```typescript
// Tuple: dokładnie 2 elementy [string, number]
let person: [string, number];
person = ["Anna", 25];  // ✓ OK

// ❌ Błędy:
// person = [25, "Anna"];     // Zła kolejność typów
// person = ["Anna"];         // Za mało elementów
// person = ["Anna", 25, 30]; // Za dużo elementów

// Dostęp do elementów:
console.log(person[0]);  // "Anna"
console.log(person[1]);  // 25

// Destrukturyzacja:
let [name, age] = person;
console.log(name);  // "Anna"
console.log(age);   // 25
```

**Tuple z opcjonalnymi elementami:**
```typescript
let coordinate: [number, number, number?]; // z opcjonalne
coordinate = [10, 20];       // ✓ OK
coordinate = [10, 20, 30];   // ✓ OK
```

### 3.4. Enum (Wyliczenie)

**Enum numeryczny:**
```typescript
enum Direction {
  Up,       // 0
  Down,     // 1
  Left,     // 2
  Right     // 3
}

let playerDirection: Direction = Direction.Up;
console.log(playerDirection);  // 0

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
enum Color {
  Red = "RED",
  Green = "GREEN",
  Blue = "BLUE"
}

let favoriteColor: Color = Color.Red;
console.log(favoriteColor);  // "RED"

// Lepsze w debugowaniu i logach
```

### 3.5. Any (Dowolny typ)

**⚠️ ANY = wyłącza sprawdzanie typów - UNIKAJ!**

```typescript
let anything: any = "hello";
anything = 42;          // OK
anything = true;        // OK
anything = { x: 1 };    // OK
anything.toUpperCase(); // Brak błędu, ale może crashować!

// Użyj TYLKO gdy:
// - Migracja z JS do TS
// - Integracja z biblioteką bez typów
// - Prototypowanie (tymczasowo)
```

### 3.6. Unknown (Bezpieczniejszy Any)

```typescript
let userInput: unknown;
userInput = "hello";
userInput = 123;

// ❌ Nie możesz od razu użyć:
// let text: string = userInput;  // ERROR

// ✅ Musisz sprawdzić typ:
if (typeof userInput === "string") {
  let text: string = userInput;  // ✓ OK
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
  // brak 'return'
}

logMessage("Hello TypeScript!");

// void można przypisać tylko undefined
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

// Never w switch (exhaustiveness checking)
type Shape = "circle" | "square";

function getArea(shape: Shape): number {
  switch (shape) {
    case "circle":
      return 3.14;
    case "square":
      return 1;
    default:
      const _exhaustive: never = shape; // Sprawdza czy obsłużyliśmy wszystkie
      return _exhaustive;
  }
}
```

---

## CZĘŚĆ 4: Funkcje (30 minut)

### 4.1. Typowanie Funkcji

```typescript
// Funkcja z typowanymi parametrami i return type
function add(a: number, b: number): number {
  return a + b;
}

// Arrow function
const multiply = (a: number, b: number): number => {
  return a * b;
};

// Krótszy zapis
const divide = (a: number, b: number): number => a / b;

console.log(add(5, 3));       // 8
console.log(multiply(5, 3));  // 15
console.log(divide(15, 3));   // 5
```

### 4.2. Parametry Opcjonalne

```typescript
function greet(name: string, age?: number): string {
  if (age !== undefined) {
    return `Cześć ${name}, masz ${age} lat`;
  }
  return `Cześć ${name}`;
}

console.log(greet("Anna"));           // "Cześć Anna"
console.log(greet("Piotr", 25));      // "Cześć Piotr, masz 25 lat"
```

### 4.3. Parametry Domyślne

```typescript
function introduce(name: string, age: number = 18): string {
  return `${name} ma ${age} lat`;
}

console.log(introduce("Kasia"));        // "Kasia ma 18 lat"
console.log(introduce("Marek", 30));    // "Marek ma 30 lat"
```

### 4.4. Rest Parameters

```typescript
function sum(...numbers: number[]): number {
  return numbers.reduce((total, n) => total + n, 0);
}

console.log(sum(1, 2, 3));           // 6
console.log(sum(10, 20, 30, 40));    // 100
```

### 4.5. Function Type (Typ Funkcji)

```typescript
// Deklaracja typu funkcji
let calculate: (x: number, y: number) => number;

// Przypisanie
calculate = (a, b) => a + b;
console.log(calculate(5, 3));  // 8

calculate = (a, b) => a * b;
console.log(calculate(5, 3));  // 15
```

---

## CZĘŚĆ 5: Interfejsy i Type Aliases (30 minut)

### 5.1. Interfejsy

```typescript
// Definicja interfejsu
interface User {
  id: number;
  name: string;
  email: string;
  isActive: boolean;
}

// Użycie
const user1: User = {
  id: 1,
  name: "Anna Kowalska",
  email: "anna@example.com",
  isActive: true
};

console.log(user1.name);  // "Anna Kowalska"

// ❌ Błąd gdy brakuje pola:
// const user2: User = {
//   id: 2,
//   name: "Piotr"
// }; // ERROR: Property 'email' is missing
```

### 5.2. Pola Opcjonalne

```typescript
interface Product {
  id: number;
  name: string;
  description?: string;  // Opcjonalne
  price: number;
}

const product1: Product = {
  id: 1,
  name: "Laptop",
  price: 3000
}; // ✓ OK - description nie jest wymagane

const product2: Product = {
  id: 2,
  name: "Monitor",
  description: "24 cale Full HD",
  price: 800
}; // ✓ OK
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

// ❌ Nie można zmienić:
// config.apiUrl = "https://new-url.com"; // ERROR: Cannot assign to 'apiUrl'
```

### 5.4. Metody w Interfejsie

```typescript
interface Calculator {
  add(a: number, b: number): number;
  subtract(a: number, b: number): number;
}

const calc: Calculator = {
  add: (a, b) => a + b,
  subtract: (a, b) => a - b
};

console.log(calc.add(10, 5));      // 15
console.log(calc.subtract(10, 5)); // 5
```

### 5.5. Rozszerzanie Interfejsów (extends)

```typescript
interface Person {
  name: string;
  age: number;
}

interface Employee extends Person {
  companyName: string;
  salary: number;
}

const employee: Employee = {
  name: "Jan Nowak",
  age: 30,
  companyName: "Tech Corp",
  salary: 5000
};
```

### 5.6. Type Alias

```typescript
// Alias dla prostego typu
type ID = number;
let userId: ID = 123;

// Alias dla obiektu
type Point = {
  x: number;
  y: number;
};

const point: Point = { x: 10, y: 20 };
```

### 5.7. Union Types

```typescript
type StringOrNumber = string | number;

let value: StringOrNumber;
value = "hello";  // ✓ OK
value = 42;       // ✓ OK
// value = true;  // ❌ ERROR
```

### 5.8. Literal Types

```typescript
type Status = "active" | "inactive" | "pending";

let userStatus: Status;
userStatus = "active";   // ✓ OK
userStatus = "pending";  // ✓ OK
// userStatus = "deleted"; // ❌ ERROR
```

---

## CZĘŚĆ 6: Klasy (25 minut)

### 6.1. Podstawowa Klasa

```typescript
class Person {
  name: string;
  age: number;

  constructor(name: string, age: number) {
    this.name = name;
    this.age = age;
  }

  greet(): string {
    return `Cześć, jestem ${this.name}`;
  }
}

const person1 = new Person("Anna", 25);
console.log(person1.greet());  // "Cześć, jestem Anna"
```

### 6.2. Modyfikatory Dostępu

```typescript
class BankAccount {
  public accountNumber: string;    // Publiczne (domyślne)
  private balance: number;         // Prywatne
  protected owner: string;         // Chronione

  constructor(accountNumber: string, initialBalance: number, owner: string) {
    this.accountNumber = accountNumber;
    this.balance = initialBalance;
    this.owner = owner;
  }

  public getBalance(): number {
    return this.balance;
  }

  private calculateInterest(): number {
    return this.balance * 0.05;
  }

  public deposit(amount: number): void {
    this.balance += amount;
  }
}

const account = new BankAccount("123456", 1000, "Jan");
console.log(account.getBalance());  // 1000
// console.log(account.balance);    // ❌ ERROR: Private
```

### 6.3. Skrócony Zapis Konstruktora

```typescript
// Zamiast długiego:
class UserLong {
  name: string;
  email: string;

  constructor(name: string, email: string) {
    this.name = name;
    this.email = email;
  }
}

// Można krótko:
class UserShort {
  constructor(
    public name: string,
    public email: string
  ) {}
}

const user = new UserShort("Anna", "anna@example.com");
console.log(user.name);  // "Anna"
```

---

## CZĘŚĆ 7: Generyki (Generics) (30 minut)

### 7.1. Generyczna Funkcja

```typescript
// Funkcja identity - zwraca to co dostanie
function identity<T>(arg: T): T {
  return arg;
}

let output1 = identity<string>("hello");  // string
let output2 = identity<number>(42);       // number
let output3 = identity("world");          // string (inferencja)

console.log(output1);  // "hello"
```

### 7.2. Generyczna Funkcja z Tablicą

```typescript
function getFirstElement<T>(arr: T[]): T | undefined {
  return arr[0];
}

const firstNum = getFirstElement([1, 2, 3]);      // number
const firstStr = getFirstElement(["a", "b"]);     // string

console.log(firstNum);  // 1
console.log(firstStr);  // "a"
```

### 7.3. Generyczny Interfejs

```typescript
interface Box<T> {
  content: T;
}

const stringBox: Box<string> = { content: "hello" };
const numberBox: Box<number> = { content: 42 };

console.log(stringBox.content);  // "hello"
```

### 7.4. Generyczna Klasa

```typescript
class DataStorage<T> {
  private data: T[] = [];

  addItem(item: T): void {
    this.data.push(item);
  }

  getItems(): T[] {
    return [...this.data];
  }
}

const numberStorage = new DataStorage<number>();
numberStorage.addItem(10);
numberStorage.addItem(20);
console.log(numberStorage.getItems());  // [10, 20]

const stringStorage = new DataStorage<string>();
stringStorage.addItem("Anna");
console.log(stringStorage.getItems());  // ["Anna"]
```

---

## CZĘŚĆ 8: Utility Types (20 minut)

### 8.1. Partial<T>

```typescript
interface User {
  id: number;
  name: string;
  email: string;
}

// Partial - wszystkie pola opcjonalne
function updateUser(id: number, updates: Partial<User>): void {
  console.log(`Aktualizuję ${id}:`, updates);
}

updateUser(1, { name: "Nowa nazwa" });  // ✓ OK
updateUser(2, { email: "new@example.com" });  // ✓ OK
```

### 8.2. Required<T>

```typescript
interface Config {
  apiUrl?: string;
  timeout?: number;
}

const fullConfig: Required<Config> = {
  apiUrl: "https://api.com",
  timeout: 5000
}; // Musi mieć wszystkie pola
```

### 8.3. Pick<T, K>

```typescript
interface User {
  id: number;
  name: string;
  email: string;
  password: string;
}

type UserPreview = Pick<User, "id" | "name">;

const preview: UserPreview = {
  id: 1,
  name: "Anna"
};
```

### 8.4. Omit<T, K>

```typescript
type UserWithoutPassword = Omit<User, "password">;

const user: UserWithoutPassword = {
  id: 1,
  name: "Anna",
  email: "anna@example.com"
};
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Kalkulator
Stwórz interfejs `Calculator` z metodami add, subtract, multiply, divide. Zaimplementuj klasę.

### Zadanie 2: Generic Stack
Stwórz generyczną klasę `Stack<T>` z metodami: push, pop, peek, isEmpty.

### Zadanie 3: User System
Stwórz typy dla systemu użytkowników z różnymi rolami (admin, user, guest).

---

## ➡️ Następna Lekcja

**[Lekcja 2: React Native Podstawy](./lekcja-02-react-native.md)**

---

**Gratulacje! 🎉 Ukończyłeś podstawy TypeScript!**
