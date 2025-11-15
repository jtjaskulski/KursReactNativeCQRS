# WSTĘP: Czym jest React Native? Architektura i Historia

## 🎯 W Tej Sekcji Nauczysz Się

- ✅ Historii React Native (2015-2025)
- ✅ Architektury React Native
- ✅ Jak działa transpilacja TS → JS → Native
- ✅ Porównania z Flutter, Xamarin, itp.
- ✅ Najnowszych trendów (New Architecture, Expo)

---

## CZĘŚĆ 1: Historia React Native (10 minut)

### 1.1. Jak to Się Zaczęło (2015)

**React Native** został stworzony przez **Facebook** (Meta) w 2015 roku.

**Problem:** W tym czasie:
- iOS aplikacje pisało się w Objective-C
- Android aplikacje pisało się w Java
- Duplikacja kodu między platformami
- Różne zespoły = drożej, wolniej

**Rozwiązanie:** React Native
- "Learn once, write anywhere"
- Jeden kod JavaScript → iOS + Android
- Reuse komponentów, logiki biznesowej
- Szybszy development cycle

**Historia wersji:**
```
2015: Początek (v0.1)
2016-2018: Stabilizacja (React Native 0.40+)
2020: Rebranding (nowa dokumentacja)
2023: New Architecture (próba modernizacji)
2024-2025: Hermes 0.15+ jako domyślny engine
```

### 1.2. Współczesne Podejście (2025)

**React Native dzisiaj:**
- ✅ Używany przez 30% mobilnych aplikacji
- ✅ Wsparcie dla najnowszych iOS/Android
- ✅ Integracja z TypeScript na starcie
- ✅ Expo dla szybkich prototypów
- ✅ New Architecture dla lepszej wydajności

---
## Segment 1.1: Czym jest React Native? (15 minut)

**SCRIPT:**

> „Dzień dobry! Dziś zaczynamy naszą 10-godzinną podróż po React Native – frameworku, który pozwoli nam pisać aplikacje mobilne w JavaScripcie. Zamiast uczyć się Kotlina na Androida i Swifta na iOS-ie, my będziemy pisać JEDEN kod w JavaScripcie, a React Native „przetłumaczy" go na natywne komponenty dla obu platform."

**Wyjaśnienie koncepcji:**

React Native to biblioteka stworzona przez Facebook (Meta) pozwalająca budować natywne aplikacje mobilne dla Androida i iOS-a za pomocą JavaScriptu. Pod spodem React Native używa **React Bridgea** – mostu komunikacyjnego między kodem JavaScript a kodem natywnym (Java, Kotlin na Androidzie; Swift, Objective-C na iOS-ie).

**Diagram do narysowania na tablicy/ekranie:**

```
┌─────────────────────────────────┐
│   JavaScript (React Native)      │
├─────────────────────────────────┤
│      React Native Bridge         │ ← Łączy JS z native'em
├─────────────────────────────────┤
│  Android (Java/Kotlin)  iOS      │
│                          (Swift)  │
└─────────────────────────────────┘
```

**Kluczowe punkty:**
- React Native NIE to WebView (jak Cordova) – to prawdziwe komponenty natywne
- Jeden kod JavaScript = dwie platformy (Android + iOS)
- Hot Reload – zmian widać od razu bez rebuild'u
- Dostęp do hardware'u urządzenia (kamera, GPS, itd.)

---

## Segment 1.2: Instalacja Node.js i pnpm (25 minut)

**SCRIPT:**

> „Zanim zaczniemy pisać, musimy zainstalować narzędzia. Będziemy używać pnpm zamiast tradycyjnego npm. pnpm jest szybszy, oszczędza miejsce na dysku i ma lepsze zarządzanie zależnościami."

**Kroki instalacji (dla Windows):**

1. **Pobierz Node.js** (wersja LTS):
   - Przejdź na nodejs.org
   - Pobierz wersję oznaczoną LTS (Long-Term Support)
   - Zainstaluj ze wszystkimi domyślnymi opcjami

2. **Sprawdź instalację Node.js:**
   ```bash
   node --version
   npm --version
   ```

3. **Zainstaluj pnpm globalnie:**
   ```bash
   npm install -g pnpm
   ```

4. **Sprawdź instalację pnpm:**
   ```bash
   pnpm --version
   ```

**Wyjaśnienie:** Node.js to środowisko JavaScript na serwerze (i lokalnie na naszych komputerach). npm to domyślny menedżer pakietów. pnpm to zaawansowana alternatywa.

**Czemu pnpm?**
- Szybszy niż npm
- Mniej miejsca na dysku (cache globalny + hardlinki)
- Lepszy lock file (`pnpm-lock.yaml`)
- Wsparcie dla monorepo

---

## Segment 1.3: Podstawowa Architektura React Native (10 minut)

**SCRIPT:**

> „React Native ma strukturę podobną do Reacta web'owego, ale zamiast HTML komponentów (div, button), używamy komponentów mobilnych (View, Text, Button, TextInput). To są wrappery wokół natywnych komponentów."

**Podstawowe komponenty:**

```
React Web              React Native
─────────────          ────────────────
<div>                  <View>
<span>, <p>            <Text>
<input>                <TextInput>
<button>               <Button> / <Pressable>
<img>                  <Image>
<ul>, <li>             <FlatList>, <ScrollView>
```

**Struktura komponentu React Native (bez TypeScript, czysta funkcja):**

```javascript
import React, { useState } from 'react';
import { View, Text, Button } from 'react-native';

function MyComponent() {
  const [count, setCount] = useState(0);

  return (
    <View style={{ padding: 20 }}>
      <Text style={{ fontSize: 18, marginBottom: 10 }}>
        Licznik: {count}
      </Text>
      <Button 
        title="Kliknij mnie" 
        onPress={() => setCount(count + 1)} 
      />
    </View>
  );
}

export default MyComponent;
```

**Wyjaśnienie kodu:**
- `View` = kontener (jak `<div>`)
- `Text` = tekst (wie się nie go do samodzielnego wrappowania)
- `useState` = hook do zarządzania stanem
- `onPress` = zdarzenie kliknięcia (zamiast `onClick`)
- `style` = obiekty JS (zamiast CSS)

---

# GODZINA 2: Tworzenie Projektu bez Expo (CLI Development Build)

## Segment 2.1: Inicjalizacja Projektu (20 minut)

**SCRIPT:**

> „Teraz stworzymy nowy projekt React Native BEZPOŚREDNIO za pomocą React Native CLI, bez Expo. Expo jest fajne na początku, ale my chcemy pełnej kontroli – będziemy móc tworzyć natywne moduły, customować konfigurację, wszystko."

**Kroki:**

1. **Otwórz terminal/PowerShell i przejdź do folderu, gdzie chcesz projekt:**
   ```bash
   cd C:\projekty
   ```

2. **Utwórz projekt za pomocą pnpm (zamiast npx):**
   ```bash
   pnpm create react-native-app OrderManagementApp
   ```
   
   **Wyjaśnienie:** `create-react-native-app` to narzędzie które automatycznie stawiamy projekt z konfiguracją. `OrderManagementApp` to nazwa projektu.

3. **Czekaj na instalację** (~2-5 minut, pnpm będzie pobierać zależności)

4. **Przejdź do folderu projektu:**
   ```bash
   cd OrderManagementApp
   ```

5. **Sprawdzenie struktury:**
   ```bash
   dir
   ```
   
   Powinieneś zobaczyć:
   ```
   android/              ← Kod Android (Java/Kotlin)
   ios/                  ← Kod iOS (Swift)
   node_modules/         ← Zainstalowane pakiety
   App.js                ← Główny komponent
   app.json              ← Konfiguracja aplikacji
   package.json          ← Zależności
   babel.config.js       ← Konfiguracja Babel (transpiler)
   ```

---

## Segment 2.2: Uruchomienie na Emulatorze Android (25 minut)

**SCRIPT:**

> „Teraz będziemy chcieli zobaczyć naszą aplikację na emulatorze Android. Na początek potrzebujemy emulator. Możemy go zainstalować za pomocą Android Studio."

**Wymagania:**
- Android Studio zainstalowany (https://developer.android.com/studio)
- Emulator Android uruchomiony lub fizyczne urządzenie podłączone

**Uruchomienie aplikacji:**

1. **Upewnij się, że emulator jest uruchomiony** (lub urządzenie podłączone)

2. **Uruchom aplikację:**
   ```bash
   pnpm run android
   ```

3. **Co się będzie działo:**
   - React Native CLI skompiluje kod Java/Kotlin
   - Zbuduje APK
   - Zainstaluje go na emulatorze
   - Metro Bundler uruchomi się (serwer bundlujący JS)
   - Zobaczysz aplikację na ekranie

4. **Jeśli zobaczysz ekran powitalny** – gratulacje! Projekt działa!

**Jeśli pojawią się błędy:**
- Sprawdź czy JAVA_HOME jest ustawiony: `echo %JAVA_HOME%` (Windows)
- Sprawdź czy Android SDK jest zainstalowany
- Uruchom ponownie emulator

**Wyjaśnienie Development Build:**

Development build to wersja aplikacji z hot reloading, debug narzędziami, i łatwościami do developmentu. To NIE jest production build (nie byłby na App Store). Benefit: zmienisz kod w JS, a aplikacja przeładuje się sama.

---

## Segment 2.3: Struktura Projektu i Konfiguracja pnpm (15 minut)

**SCRIPT:**

> „Zanim pójdziemy dalej, musimy zrozumieć strukturę projektu i ustawić pnpm dla React Native."

**Ważne ustawienie dla React Native + pnpm:**

Otwórz plik `.npmrc` w głównym folderze projektu (jeśli nie istnieje – utwórz go):

```ini
# .npmrc
node-linker=hoisted
```

**Wyjaśnienie:** React Native potrzebuje „spłaszczonej" struktury `node_modules` (wszystkie pakiety w jednym poziomie). pnpm domyślnie tworzy strukturę „symlinków" które są lepsze, ale RN nie zawsze je lubi. `node-linker=hoisted` mówi pnpm aby robił to jak npm.

**Struktura folderów którą będziemy budować:**

```
OrderManagementApp/
├── android/              (Nie będziemy tutaj edytować)
├── ios/                  (Nie będziemy tutaj edytować)
├── src/
│   ├── api/             ← Komunikacja z .NET API
│   │   └── apiService.js
│   ├── screens/         ← Ekrany aplikacji
│   │   ├── UnitOfMeasurementScreen.js
│   │   ├── CategoryScreen.js
│   │   ├── ClientScreen.js
│   │   ├── WorkerScreen.js
│   │   ├── ItemScreen.js
│   │   ├── OrderScreen.js
│   │   └── OrderItemScreen.js
│   ├── components/      ← Reużywalne komponenty
│   │   ├── FormInput.js
│   │   ├── ListItem.js
│   │   └── ActionButtons.js
│   ├── hooks/           ← Custom hooki
│   │   └── useFetch.js
│   ├── utils/           ← Funkcje pomocnicze
│   │   └── validation.js
│   └── constants/       ← Stałe
│       └── apiEndpoints.js
├── App.js
├── app.json
├── package.json
└── .npmrc
```

**Punkt nauczania:**
Dzielenie kodu na moduły jest KLUCZOWE w większych projektach. Każdy folder ma jedną odpowiedzialność. To ułatwia testowanie, debugowanie, i maintenance.

---

## CZĘŚĆ 2: Architektura React Native

### 2.1. Warstwy React Native

```
┌─────────────────────────────────────────────┐
│        Aplikacja (TypeScript/React)         │
│  - Komponenty (Button, Text, View)          │
│  - Logika biznesowa                         │
│  - State management                         │
└────────────────┬────────────────────────────┘
                 │
         React Native Bridge
         (komunikacja JS ↔ Native)
                 │
    ┌────────────┴────────────┐
    ↓                         ↓
┌────────────┐          ┌─────────────┐
│  iOS App   │          │ Android App │
│  (Swift)   │          │  (Kotlin)   │
└────────────┘          └─────────────┘
    ↓                         ↓
┌────────────┐          ┌─────────────┐
│  UIView    │          │  View       │
│  Foundation│          │  Activity   │
└────────────┘          └─────────────┘
    ↓                         ↓
┌────────────────────────────────────────┐
│      iOS/Android System Libraries      │
└────────────────────────────────────────┘
```

### 2.2. JavaScript Engine

W React Native do uruchomienia kodu JavaScript:

**Opcje:**
1. **Hermes** (2025 - domyślny)
   - Stworzony przez Meta
   - Lekki (mniej RAM)
   - Szybszy startup
   - Wbudowany debugger

2. **JavaScriptCore** (iOS)
   - Natywny dla iOS
   - Wydajny

3. **V8** (Android - rzadko)
   - Ciężki
   - Powoli

**W kursie używamy: Hermes** (domyślnie w React Native 0.82+)

### 2.3. Bridge - Łącznik JS ↔ Native

```
JavaScript Engine
        │
        │ (Binary data)
        ↓
    BRIDGE
        │
        │ (Native methods)
        ↓
Native Modules
(Android Java/Kotlin, iOS Swift)
```

**Przykład:**
```typescript
// TS - pytamy o dane z kamery
import ImagePicker from 'react-native-image-picker';
launchCamera(...);

// Bridge konwertuje na natywny kod
// Android: kód Java aktywuje kamerę
// iOS: Swift UIImagePickerController

// Wynik wraca przez bridge
// TS otrzymuje URI zdjęcia
```

---

## CZĘŚĆ 3: React Native vs Konkurencja

| Aspekt | React Native | Flutter | Xamarin | Ionic |
|--------|-------------|---------|---------|-------|
| **Język** | JavaScript/TS | Dart | C# | HTML/TS |
| **Wydajność** | Bardzo dobra | Najlepsza | Dobra | Średnia |
| **Łatwość** | Łatwe (JS) | Średnie (Dart) | Średnie (C#) | Łatwe (Web) |
| **Ekosystem** | Ogromny | Rosnący | Zmniejszający | Mature |
| **Popularne Apps** | Facebook, Instagram, Discord | Google Ads, eBay, Alibaba | Microsoft apps | Wikimedia |
| **Hot Reload** | ✅ Tak | ✅ Tak | ❌ Nie | ✅ Tak |
| **Native Feel** | ✅ Doskonale | ✅ Doskonale | ✅ Doskonale | ❌ Słabiej |

**Dlaczego React Native w tym kursie?**
- ✅ JavaScript (znany większości frontend devów)
- ✅ React ecosystem (React dev mają naturalnie)
- ✅ Ogromna społeczność
- ✅ Dużo bibliotek i narzędzi
- ✅ Łatwo znaleźć specjalistów

---

## CZĘŚĆ 4: Najnowsze Trendy (2024-2025)

### 4.1. Hermes Engine

```bash
# Włącz Hermes w projekcie
# android/app/build.gradle
enableHermes = true;  # domyślnie true!
```

**Korzyści:**
- 10-20% szybszy startup
- 30% mniej RAM
- Szybszy first render

### 4.2. New Architecture (Bridgeless)

Nowa architektura React Native (0.73+):
- ✅ Bezpośrednia komunikacja (bez Bridge)
- ✅ Lepsze performance
- ✅ Łatwiejsze integrowanie native code
- ✅ Faster rendering

**Status (2025):** Stabilny, ale nie domyślny. Migracja w toku.

### 4.3. Expo (Alternatywa)

**Expo SDK** - warstwa abstraktu nad React Native:

```bash
# Zamiast setup React Native
npx create-expo-app MyApp
cd MyApp
npx expo start
```

**Zalety:**
- ✅ Szybki setup (5 minut)
- ✅ Gotowe biblioteki (camera, location, etc.)
- ✅ Cloud builds (EAS)
- ✅ Over-the-air updates

**Wady:**
- ❌ Mniej kontroli
- ❌ Nieco większy bundle
- ❌ Ograniczone custom native modules

**W tym kursie:** Używamy **bare React Native** (nie Expo) aby nauczyć się internals.

### 4.4. TypeScript Everywhere

```typescript
// 2015: JavaScript wszędzie
// 2025: TypeScript domyślnie
const App: React.FC = () => { ... };
```

**Status:** TypeScript jest domyślny (tsconfig.json na starcie).

---

## CZĘŚĆ 5: Versioning React Native (2025)

### 5.1. Najnowsze Wersje

```
React Native 0.82.x (LTS)   ← CURRENT (Nov 2025)
React Native 0.81.x         ← Previous
React Native 0.80.x         ← Stable
React Native 0.79.x         ← Old
```

**Strategia wersjonowania:**
- Nowa minor wersja ~co 2 miesiące
- Patch releases co tydzień
- Long-Term Support (LTS) wersje

### 5.2. Wersje w Tym Kursie

```json
{
  "dependencies": {
    "react": "^18.3.1",
    "react-native": "0.82.1",
    "@react-navigation/native": "^6.1.12",
    "@react-navigation/native-stack": "^6.9.18"
  },
  "devDependencies": {
    "typescript": "^5.3.3",
    "@types/react-native": "^0.82.0",
    "@react-native-community/cli": "^13.6.7"
  }
}
```

**Upewnij się że twój `package.json` ma te wersje!**

---

## CZĘŚĆ 6: Ecosystem React Native (2025)

### 6.1. Kluczowe Biblioteki

```
State Management:
  - Zustand (prosty)
  - Redux Toolkit (complex)
  - Jotai (atomic)

Networking:
  - fetch API (built-in)
  - Axios
  - React Query / TanStack Query

UI Components:
  - React Native Paper (Material)
  - NativeBase
  - Tamagui

Navigation:
  - React Navigation (⭐ UŻYWAMY)
  - React Native Navigation

Testing:
  - Jest (default)
  - Detox (E2E)
  - React Native Testing Library
```

### 6.2. Developer Tools

```
IDE:
  - Visual Studio Code (FREE)
  - Android Studio (FREE)
  - Xcode (FREE, Mac only)

Debugging:
  - Flipper (Desktop debugger)
  - React Native Debugger
  - Chrome DevTools

Performance:
  - Profiler (React)
  - Hermes Debugger
  - Android Profiler
```

---

## CZĘŚĆ 7: Porównanie Setup 2015 vs 2025

### 2015 - Pierwsza Wersja RN

```bash
# Krok 1: Zainstaluj Node
brew install node

# Krok 2: Zainstaluj React Native CLI
npm install -g react-native

# Krok 3: Utwórz projekt
react-native init MyApp

# Krok 4: Setup Android Studio (ręcznie)
# Pobierz, zainstaluj, konfiguruj PATH...
# PAIN!

# Krok 5: Uruchom
react-native run-android
```

**Problemy:**
- ❌ Brak TypeScript
- ❌ Ręczna konfiguracja Android
- ❌ Słabe IDE support
- ❌ Wolne compilation

### 2025 - Nowoczesny Setup

```bash
# Krok 1: Node już zainstalowany
node --version  # v18+

# Krok 2: pnpm zamiast npm
npm install -g pnpm

# Krok 3: CLI init (auto-detect Android)
npx @react-native-community/cli init MyApp

# Krok 4: TypeScript ready! ✅
# - Wszystkie pliki to .tsx
# - tsconfig.json na miejscu

# Krok 5: pnpm install (szybciej!)
pnpm install

# Krok 6: Run
pnpm react-native run-android
```

**Ulepszenia:**
- ✅ TypeScript built-in
- ✅ Szybki setup (5 minut)
- ✅ Auto-detect Android location
- ✅ Hermes by default
- ✅ Hot reload lightning fast
- ✅ Excellent IDE support

---

## CZĘŚĆ 8: React Native Fundamentals

### 8.1. Core Concepts

**1. Components**
```tsx
const MyComponent: React.FC = () => {
  return <Text>Hello</Text>;
};
```

**2. State & Props**
```tsx
const [count, setCount] = useState(0);
<Button title={count.toString()} />
```

**3. Native Modules**
```tsx
import { Vibration } from 'react-native';
Vibration.vibrate(100);
```

**4. Platform-Specific Code**
```tsx
import { Platform } from 'react-native';

if (Platform.OS === 'android') {
  // Android code
} else if (Platform.OS === 'ios') {
  // iOS code
}
```

### 8.2. Event Loop & Async

```typescript
// TS/JS Event Loop w React Native

// Synchroniczny kod - blokuje UI
function slowSync() {
  for (let i = 0; i < 1000000000; i++) { }
}

// Asynchroniczny - nie blokuje
async function fetchData() {
  const data = await fetch('/api');
  return data;
}

// setTimeout - za iteracją event loop
setTimeout(() => {
  console.log('After other tasks');
}, 0);
```

---

## CZĘŚĆ 9: Ścieżka Naukowa

```
Fundamenty React Native
        ↓
    (Część ta - wstęp)
        ↓
Lekcja 2: Setup + komponenty
        ↓
Lekcja 5: Integracja z API
        ↓
Lekcja 10-11: Native modules
        ↓
Wdrażanie na produkcję! 🚀
```

---

## PODSUMOWANIE

**React Native w 2025:**
- ✅ Dojrzała technologia (10 lat)
- ✅ Wyprodukowana przez Meta (duży backing)
- ✅ TypeScript z pudełka
- ✅ Ogromna społeczność
- ✅ Wciąż się ewoluuje (New Architecture)
- ✅ Przyszłość mobilnego developmentu

**Dlaczego się uczyć RN?**
1. Wiele firm używa (Discord, Instagram, Uber Eats...)
2. Jedna umiejętność = 2 platformy
3. JavaScript - łatwo się uczyć
4. High paying jobs
5. Szybki development cycle

---

**Gotowy? Lecimy do Lekcji 2! 🚀**
