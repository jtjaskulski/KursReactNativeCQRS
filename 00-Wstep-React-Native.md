# WSTĘP: Czym jest React Native? Architektura i Historia

**Moduł:** Wprowadzenie do React Native  
**Czas trwania:** 2 godziny  
**Poziom:** Początkujący

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Wyjaśnić historię i ewolucję React Native (2015-2025)
- ✅ Opisać architekturę React Native (Bridge, Hermes, JSI)
- ✅ Zrozumieć proces transpilacji TypeScript → JavaScript → Native
- ✅ Porównać React Native z Flutter, Xamarin i innymi frameworkami
- ✅ Rozpoznać najnowsze trendy (New Architecture, Expo)
- ✅ Skonfigurować środowisko deweloperskie

---

## CZĘŚĆ 1: Czym Jest React Native i Jego Historia (30 minut)

### 1.1. Historia React Native (2015-2025)

**SCRIPT dla prowadzącego:**

> „Dzień dobry! Dziś zaczynamy naszą podróż po React Native – frameworku, który pozwoli nam pisać aplikacje mobilne w JavaScripcie. Zamiast uczyć się Kotlina na Androida i Swifta na iOS-ie, my będziemy pisać JEDEN kod w JavaScripcie, a React Native przetłumaczy go na natywne komponenty dla obu platform."

**React Native** został stworzony przez **Facebook** (Meta) w 2015 roku jako odpowiedź na rosnące koszty utrzymania dwóch osobnych zespołów mobilnych.

**Problem, który rozwiązywał:**

W 2015 roku tworzenie aplikacji mobilnych wyglądało tak:
- iOS aplikacje pisało się w Objective-C (później Swift)
- Android aplikacje pisało się w Java (później Kotlin)
- Ten sam feature = 2x więcej kodu, 2x więcej bugów
- Różne zespoły = droższa komunikacja, wolniejszy development
- Różne UI/UX między platformami

**Rozwiązanie - React Native:**
- Filozofia: "Learn once, write anywhere"
- Jeden kod JavaScript → iOS + Android
- Reuse komponentów, logiki biznesowej, stylów
- Szybszy development cycle dzięki Hot Reload
- Dostęp do natywnych API urządzenia

**Ewolucja wersji (2015-2025):**

```
┌──────────────────────────────────────────────────────────────┐
│                    TIMELINE REACT NATIVE                      │
├──────────────────────────────────────────────────────────────┤
│  2015 │ Początek (v0.1) - pierwsza wersja open source        │
│  2016 │ Stabilizacja - React Native 0.40+                    │
│  2018 │ Re-architektura rozpoczęta (Fabric, TurboModules)    │
│  2019 │ Hermes wprowadzony jako opcjonalny engine            │
│  2020 │ Nowa dokumentacja, rebranding                        │
│  2022 │ New Architecture dostępna eksperymentalnie           │
│  2023 │ New Architecture stabilna                            │
│  2024 │ Hermes domyślny engine, React Native 0.73+           │
│  2025 │ React Native 0.82+ - pełna dojrzałość                │
└──────────────────────────────────────────────────────────────┘
```

### 1.2. Porównanie: Setup 2015 vs 2025

**❌ Setup w 2015 - Pierwsza Wersja RN:**

```bash
# Krok 1: Zainstaluj Node (ręcznie z instalatora)
# Krok 2: Zainstaluj React Native CLI
npm install -g react-native

# Krok 3: Utwórz projekt (tylko JavaScript!)
react-native init MyApp

# Krok 4: Setup Android Studio (ręcznie)
# - Pobierz Android Studio (~2GB)
# - Zainstaluj SDK Manager
# - Skonfiguruj ANDROID_HOME
# - Dodaj do PATH
# - PAIN! Wiele błędów konfiguracji

# Krok 5: Uruchom (często nie działało za pierwszym razem)
react-native run-android
```

**Problemy w 2015:**
- ❌ Brak TypeScript (tylko JavaScript)
- ❌ Ręczna konfiguracja Android/iOS
- ❌ Słabe wsparcie IDE
- ❌ Wolna kompilacja (kilka minut)
- ❌ Częste "Red Screen of Death"
- ❌ Brak Hot Reload (tylko Live Reload)

**✅ Setup w 2025 - Nowoczesny Sposób:**

```bash
# Krok 1: Node już zainstalowany (przez nvm/winget)
node --version  # v20+ lub v18 LTS

# Krok 2: pnpm zamiast npm (szybszy, wydajniejszy)
npm install -g pnpm

# Krok 3: Utwórz projekt (TypeScript domyślnie!)
npx @react-native-community/cli init SolutionOrdersMobile

# Krok 4: Konfiguracja pnpm dla RN
echo "node-linker=hoisted" > .npmrc
pnpm install

# Krok 5: Run (Android Studio auto-detected!)
pnpm react-native run-android
```

**Ulepszenia w 2025:**
- ✅ TypeScript wbudowany od startu
- ✅ Szybki setup (5-10 minut)
- ✅ Auto-detect Android SDK location
- ✅ Hermes engine domyślnie (szybszy startup)
- ✅ Fast Refresh (Hot Reload 2.0)
- ✅ Doskonałe wsparcie IDE (VS Code, IntelliJ)
- ✅ Lepsza dokumentacja i community

### 1.3. React Native Dzisiaj (2025)

**Statystyki i adopcja:**
- ✅ Używany przez ~30% aplikacji mobilnych (wg Statista)
- ✅ 2 miliony+ aktywnych projektów na GitHub
- ✅ Top 10 najczęściej używanych frameworków
- ✅ Wsparcie dla najnowszych iOS 17+ / Android 14+
- ✅ Pełna integracja z TypeScript

**Wersjonowanie React Native (2025):**

```
React Native 0.82.x (CURRENT)  ← Używamy w tym kursie
React Native 0.81.x            ← Previous stable
React Native 0.80.x            ← Still supported
React Native 0.79.x            ← Legacy
```

**Strategia wersjonowania:**
- Nowa minor wersja co ~8 tygodni
- Patch releases co tydzień (bug fixes)
- Breaking changes dokumentowane w CHANGELOG

**Wersje używane w tym kursie:**

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
    "@types/react": "^18.3.0",
    "@react-native-community/cli": "^13.6.7"
  }
}
```

---

## CZĘŚĆ 2: Architektura React Native (40 minut)

### 2.1. Jak Działa React Native - Podstawy

**SCRIPT dla prowadzącego:**

> „React Native to NIE jest WebView jak Cordova czy PhoneGap. To prawdziwe natywne komponenty! Gdy piszesz `<View>`, React Native tworzy prawdziwy `UIView` na iOS i `android.view.View` na Androidzie. Dlatego aplikacje RN są szybkie i wyglądają natywnie."

**Kluczowe koncepcje:**

| Koncept | Opis |
|---------|------|
| **Natywne komponenty** | `<View>`, `<Text>` → prawdziwe natywne widoki |
| **JavaScript Engine** | Hermes/JSC uruchamia Twój kod JS |
| **Bridge/JSI** | Komunikacja między JS a natywnym kodem |
| **Metro Bundler** | Pakuje JS do aplikacji (jak Webpack) |

**Diagram - Warstwy React Native:**

```
┌─────────────────────────────────────────────────────────────┐
│                 TWOJA APLIKACJA                              │
│  ┌───────────────────────────────────────────────────────┐  │
│  │            TypeScript / JavaScript                     │  │
│  │  - Komponenty React (View, Text, Button)              │  │
│  │  - Logika biznesowa                                   │  │
│  │  - State management (Context, Redux, Zustand)         │  │
│  │  - Stylowanie (StyleSheet)                            │  │
│  └───────────────────────────────────────────────────────┘  │
│                           │                                  │
│                    ┌──────▼──────┐                          │
│                    │   HERMES    │  JavaScript Engine        │
│                    │   (lub JSC) │  Uruchamia JS             │
│                    └──────┬──────┘                          │
│                           │                                  │
│              ┌────────────▼────────────┐                    │
│              │  JSI / BRIDGE / FABRIC  │                    │
│              │  Komunikacja JS ↔ Native │                    │
│              └────────────┬────────────┘                    │
│                           │                                  │
│         ┌─────────────────┴─────────────────┐               │
│         ↓                                   ↓               │
│  ┌─────────────────┐               ┌─────────────────┐      │
│  │   iOS Native    │               │  Android Native │      │
│  │  Swift / ObjC   │               │  Kotlin / Java  │      │
│  │  UIKit, SwiftUI │               │  Android SDK    │      │
│  └────────┬────────┘               └────────┬────────┘      │
│           │                                 │               │
│           ↓                                 ↓               │
│  ┌─────────────────┐               ┌─────────────────┐      │
│  │   iOS System    │               │ Android System  │      │
│  │  (iPhone/iPad)  │               │ (Pixel, Samsung)│      │
│  └─────────────────┘               └─────────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

**Wyjaśnienie każdej warstwy:**

1. **Twój kod TS/JS** - tutaj piszesz komponenty, logikę, style
2. **Hermes** - silnik JavaScript, uruchamia Twój kod
3. **JSI/Bridge** - tłumaczy wywołania JS na natywne metody
4. **Native Layer** - prawdziwy kod iOS/Android
5. **System** - urządzenie użytkownika

### 2.2. JavaScript Engine - Hermes

**Czym jest JavaScript Engine?**

JavaScript Engine to program który uruchamia kod JavaScript. Przeglądarka Chrome używa V8, Safari używa JavaScriptCore, a React Native używa **Hermes**.

**Dostępne opcje:**

| Engine | Platforma | Charakterystyka |
|--------|-----------|-----------------|
| **Hermes** ⭐ | Android + iOS | Domyślny w RN 0.70+, stworzony przez Meta |
| **JavaScriptCore** | iOS | Natywny engine Apple |
| **V8** | Android | Engine Chrome'a (rzadko używany w RN) |

**Dlaczego Hermes jest lepszy?**

```
┌────────────────────────────────────────────────────────────┐
│              PORÓWNANIE: Hermes vs JavaScriptCore           │
├─────────────────────┬──────────────┬───────────────────────┤
│      Metryka        │    Hermes    │   JavaScriptCore      │
├─────────────────────┼──────────────┼───────────────────────┤
│ Czas startu (TTI)   │    ~1.5s     │       ~2.5s           │
│ Rozmiar APK         │    -10%      │      baseline         │
│ Zużycie RAM         │    -30%      │      baseline         │
│ Bytecode Precompile │      ✅       │         ❌            │
│ Wbudowany Debugger  │      ✅       │    Chrome DevTools    │
└─────────────────────┴──────────────┴───────────────────────┘
```

**Włączanie Hermes (domyślne od RN 0.70+):**

```groovy
// android/app/build.gradle
android {
    defaultConfig {
        // Hermes jest domyślnie włączony
    }
}

// Jeśli potrzebujesz wyłączyć (nie zalecane):
project.ext.react = [
    enableHermes: false
]
```

### 2.3. Bridge vs JSI - Stara i Nowa Architektura

**SCRIPT dla prowadzącego:**

> „Przez lata React Native używał czegoś co nazywamy 'Bridge' - mostu między JavaScriptem a kodem natywnym. Problem? Każda komunikacja musiała być serializowana do JSON, przesłana przez most, i deserializowana po drugiej stronie. To było wolne. Nowa Architektura z JSI rozwiązuje ten problem."

**Stara Architektura (Bridge):**

```
┌──────────────────┐          ┌──────────────────┐
│   JavaScript     │          │     Native       │
│                  │          │                  │
│  callNative({    │ ──JSON─► │ deserialize()    │
│    module: 'X',  │          │ call module X    │
│    method: 'Y',  │ ◄─JSON── │ serialize result │
│    args: [...]   │          │                  │
│  })              │          │                  │
└──────────────────┘          └──────────────────┘

Problem: Każde wywołanie = serializacja JSON!
- Wolne dla częstych operacji
- Nie można dzielić pamięci
- Asynchroniczne (opóźnienia w UI)
```

**Nowa Architektura (JSI - JavaScript Interface):**

```
┌──────────────────┐          ┌──────────────────┐
│   JavaScript     │          │     Native       │
│                  │          │                  │
│  // Bezpośredni  │ ──────►  │ // Synchroniczne │
│  // dostęp do    │          │ // wywołania     │
│  // natywnych    │ ◄──────  │                  │
│  // obiektów!    │          │                  │
└──────────────────┘          └──────────────────┘

Korzyści:
✅ Bezpośrednie wywołania (bez JSON)
✅ Synchroniczna komunikacja
✅ Dzielenie pamięci między JS i Native
✅ Szybsze renderowanie UI
```

**Komponenty New Architecture:**

| Komponent | Funkcja | Status 2025 |
|-----------|---------|-------------|
| **JSI** | JavaScript Interface - bezpośrednia komunikacja | ✅ Stabilny |
| **Fabric** | Nowy renderer UI - synchroniczny | ✅ Stabilny |
| **TurboModules** | Lazy-loading modułów natywnych | ✅ Stabilny |
| **Codegen** | Generowanie typów z JS do Native | ✅ Stabilny |

### 2.4. Podstawowe Komponenty React Native

**SCRIPT dla prowadzącego:**

> „React Native ma strukturę podobną do Reacta webowego, ale zamiast elementów HTML używamy komponentów mobilnych. Zamiast `<div>` mamy `<View>`, zamiast `<p>` mamy `<Text>`. To są wrappery wokół prawdziwych natywnych komponentów."

**Mapowanie React Web → React Native:**

```
┌─────────────────────────────────────────────────────────┐
│          REACT WEB              REACT NATIVE            │
├─────────────────────────────────────────────────────────┤
│  <div>                    →     <View>                  │
│  <span>, <p>, <h1>        →     <Text>                  │
│  <input type="text">      →     <TextInput>             │
│  <button>                 →     <Button>, <Pressable>   │
│  <img>                    →     <Image>                 │
│  <ul>, <li>               →     <FlatList>, <ScrollView>│
│  <a href="...">           →     <Pressable> + Linking   │
│  <form>                   →     Nie istnieje (ręcznie)  │
│  CSS classes              →     StyleSheet.create()     │
│  onClick                  →     onPress                 │
└─────────────────────────────────────────────────────────┘
```

**Przykład prostego komponentu:**

```tsx
import React, { useState } from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';

// Komponent funkcyjny z TypeScript
function Counter(): React.JSX.Element {
  // Hook useState - zarządzanie stanem
  const [count, setCount] = useState<number>(0);

  return (
    // View = kontener (jak <div>)
    <View style={styles.container}>
      {/* Text = tekst (WSZYSTKIE teksty muszą być w <Text>!) */}
      <Text style={styles.countText}>
        Licznik: {count}
      </Text>
      
      {/* Button = przycisk natywny */}
      <Button 
        title="Zwiększ" 
        onPress={() => setCount(count + 1)}  // onPress zamiast onClick!
      />
    </View>
  );
}

// StyleSheet = zoptymalizowane style (jak CSS)
const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f5f5f5',
  },
  countText: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
    color: '#333',
  },
});

export default Counter;
```

**Wyjaśnienie kluczowych różnic:**

| Aspekt | React Web | React Native |
|--------|-----------|--------------|
| Kontener | `<div>` | `<View>` |
| Tekst | Bezpośrednio w JSX | Musi być w `<Text>` |
| Style | CSS classes/inline | `StyleSheet.create()` |
| Zdarzenia | `onClick` | `onPress` |
| Layout | CSS (Flexbox/Grid) | Flexbox (domyślny) |
| Units | px, em, rem, % | Tylko liczby (dp) |

### 2.5. Platform-Specific Code

React Native pozwala pisać kod specyficzny dla platformy:

**Sposób 1: Platform.OS**
```tsx
import { Platform, Text } from 'react-native';

function PlatformExample() {
  return (
    <Text>
      {Platform.OS === 'android' 
        ? 'Witaj Android!' 
        : 'Witaj iOS!'}
    </Text>
  );
}
```

**Sposób 2: Platform.select()**
```tsx
import { Platform, StyleSheet } from 'react-native';

const styles = StyleSheet.create({
  container: {
    padding: Platform.select({
      ios: 20,
      android: 16,
      default: 12,
    }),
    // Shadow - różny na iOS i Android
    ...Platform.select({
      ios: {
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.25,
        shadowRadius: 4,
      },
      android: {
        elevation: 4,
      },
    }),
  },
});
```

**Sposób 3: Osobne pliki (.ios.tsx / .android.tsx)**
```
components/
├── Button.ios.tsx      ← iOS wersja
├── Button.android.tsx  ← Android wersja
└── Button.tsx          ← Fallback
```

---

## CZĘŚĆ 3: React Native vs Konkurencja (20 minut)

### 3.1. Porównanie Frameworków Cross-Platform

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    PORÓWNANIE FRAMEWORKÓW MOBILNYCH                         │
├──────────────┬──────────────┬──────────────┬──────────────┬────────────────┤
│   Aspekt     │ React Native │   Flutter    │   Xamarin    │     Ionic      │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Język        │ JavaScript   │    Dart      │     C#       │  HTML/CSS/TS   │
│              │ TypeScript   │              │              │                │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Wydajność    │ ⭐⭐⭐⭐      │  ⭐⭐⭐⭐⭐    │   ⭐⭐⭐⭐     │    ⭐⭐⭐        │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Łatwość      │ ⭐⭐⭐⭐⭐     │   ⭐⭐⭐⭐     │   ⭐⭐⭐      │   ⭐⭐⭐⭐⭐     │
│ nauki        │ (znasz JS)   │ (nowy język) │  (znasz C#)  │  (znasz web)   │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Ekosystem    │  Ogromny     │   Rosnący    │ Zmniejszający│    Mature      │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Hot Reload   │     ✅        │      ✅       │      ❌       │       ✅        │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Native Feel  │  Doskonale   │  Doskonale   │  Doskonale   │    Średnio     │
│              │ (natywne UI) │ (własne UI)  │ (natywne UI) │   (WebView)    │
├──────────────┼──────────────┼──────────────┼──────────────┼────────────────┤
│ Znane apps   │ Instagram    │ Google Ads   │ Microsoft    │   Wikimedia    │
│              │ Discord      │ eBay Motors  │ apps         │   MarketWatch  │
│              │ Uber Eats    │ Alibaba      │              │                │
└──────────────┴──────────────┴──────────────┴──────────────┴────────────────┘
```

### 3.2. Dlaczego React Native w Tym Kursie?

**Argumenty za React Native:**

1. **JavaScript/TypeScript** 
   - Najpopularniejszy język programowania
   - Każdy frontend developer już go zna
   - Łatwa ścieżka z web do mobile

2. **React Ecosystem**
   - React developers mają naturalny start
   - Znane koncepcje: komponenty, hooks, state
   - Duża ilość bibliotek

3. **Community i wsparcie**
   - 100k+ pytań na StackOverflow
   - Aktywny Discord i GitHub
   - Regularne aktualizacje od Meta

4. **Rynek pracy**
   - Wysoka płaca (senior: 25-45k PLN/mies.)
   - Duży popyt (setki ofert w Polsce)
   - Wiele dużych firm używa RN

5. **Znane aplikacje**
   - Facebook, Instagram, Messenger
   - Discord, Pinterest
   - Uber Eats, Shopify
   - Coinbase, Bloomberg

---

## CZĘŚĆ 4: Ecosystem React Native 2025 (15 minut)

### 4.1. Kluczowe Biblioteki

**State Management:**
```
┌────────────────────────────────────────────────────┐
│  Zustand       │ Prosty, lekki          │ ⭐⭐⭐⭐⭐  │
│  Redux Toolkit │ Złożone aplikacje      │ ⭐⭐⭐⭐   │
│  Jotai         │ Atomic state           │ ⭐⭐⭐⭐   │
│  MobX          │ Observable pattern     │ ⭐⭐⭐    │
│  Context API   │ Built-in React         │ ⭐⭐⭐    │
└────────────────────────────────────────────────────┘
```

**Networking:**
```
┌────────────────────────────────────────────────────┐
│  fetch()       │ Built-in, wystarczający │ ⭐⭐⭐⭐  │
│  Axios         │ Popularne, features      │ ⭐⭐⭐⭐  │
│  TanStack Query│ Cache, mutations, stale  │ ⭐⭐⭐⭐⭐ │
│  SWR           │ Stale-while-revalidate   │ ⭐⭐⭐⭐  │
└────────────────────────────────────────────────────┘
```

**UI Components:**
```
┌────────────────────────────────────────────────────┐
│  React Native Paper │ Material Design    │ ⭐⭐⭐⭐⭐ │
│  NativeBase         │ Customizable       │ ⭐⭐⭐⭐  │
│  Tamagui            │ Fast, cross-plat   │ ⭐⭐⭐⭐  │
│  Gluestack          │ Modern approach    │ ⭐⭐⭐⭐  │
└────────────────────────────────────────────────────┘
```

**Navigation:**
```
┌────────────────────────────────────────────────────┐
│  React Navigation ⭐│ STANDARD, używamy   │ ⭐⭐⭐⭐⭐ │
│  Expo Router        │ File-based routing  │ ⭐⭐⭐⭐  │
│  React Native Nav   │ Wyman's alternative │ ⭐⭐⭐   │
└────────────────────────────────────────────────────┘
```

### 4.2. Developer Tools

**IDE i Edytory:**
- **Visual Studio Code** (FREE) - najpopularniejszy
- **Cursor** - AI-powered VS Code fork
- **WebStorm** - pełne IDE (płatne)

**Debugging:**
- **Flipper** - Desktop debugger od Meta
- **React DevTools** - inspekcja komponentów
- **Hermes Debugger** - debugging Hermes engine
- **Chrome DevTools** - sieć, console

**Performance:**
- **React Native Performance** - wbudowane metryki
- **Flashlight** - performance testing
- **Android Profiler** - natywne metryki Android

---

## CZĘŚĆ 5: Expo vs React Native CLI (10 minut)

### 5.1. Expo SDK

**Expo** to warstwa abstrakcji nad React Native:

```bash
# Utworzenie projektu Expo
npx create-expo-app MyApp
cd MyApp
npx expo start
```

**Zalety Expo:**
- ✅ Setup w 2 minuty
- ✅ Gotowe biblioteki (camera, location, notifications)
- ✅ Cloud builds (EAS Build)
- ✅ Over-the-air updates
- ✅ Expo Go app - testowanie bez build

**Wady Expo:**
- ❌ Mniej kontroli nad native code
- ❌ Większy rozmiar bundle
- ❌ Ograniczone custom native modules
- ❌ Niektóre biblioteki nie działają

### 5.2. React Native CLI (Bare)

**W tym kursie używamy: React Native CLI (bez Expo)**

```bash
# Utworzenie projektu bare RN
npx @react-native-community/cli init SolutionOrdersMobile
```

**Dlaczego bare CLI?**
- ✅ Pełna kontrola nad native code
- ✅ Możliwość pisania custom native modules
- ✅ Nauka internals React Native
- ✅ Lepsze przygotowanie do enterprise projektów
- ✅ Mniejszy bundle size

**Porównanie:**

| Aspekt | Expo | Bare CLI |
|--------|------|----------|
| Setup time | 2 min | 10 min |
| Native modules | Ograniczone | Pełne |
| Bundle size | Większy | Mniejszy |
| Learning curve | Łatwiejszy | Trudniejszy |
| Enterprise | Rzadziej | Częściej |

---

## CZĘŚĆ 6: Setup Środowiska - Przegląd (25 minut)

### 6.1. Wymagania i Instalacja

**Wymagania wstępne:**

| Narzędzie | Wersja | Instalacja |
|-----------|--------|------------|
| Node.js | 18+ LTS | nodejs.org lub nvm |
| pnpm | 8+ | `npm install -g pnpm` |
| Git | 2.30+ | git-scm.com |
| Android Studio | Latest | developer.android.com/studio |
| Java JDK | 17+ | Przez Android Studio |

**Krok 1: Instalacja Node.js**
```bash
# Windows (winget)
winget install OpenJS.NodeJS.LTS

# Weryfikacja
node --version   # v18.x.x lub v20.x.x
npm --version    # 9.x.x lub 10.x.x
```

**Krok 2: Instalacja pnpm**
```bash
npm install -g pnpm
pnpm --version   # 8.x.x lub 9.x.x
```

**Krok 3: Utworzenie projektu**
```bash
# Utwórz projekt React Native z TypeScript
npx @react-native-community/cli init SolutionOrdersMobile

# Przejdź do folderu
cd SolutionOrdersMobile

# Konfiguracja pnpm dla React Native
echo "node-linker=hoisted" > .npmrc

# Instalacja zależności
pnpm install
```

### 6.2. Struktura Projektu

```
SolutionOrdersMobile/
├── android/                 # Kod Android (Java/Kotlin)
│   ├── app/
│   │   ├── src/main/
│   │   └── build.gradle
│   └── build.gradle
├── ios/                     # Kod iOS (Swift/ObjC)
│   ├── SolutionOrdersMobile/
│   └── Podfile
├── node_modules/            # Zainstalowane pakiety
├── src/                     # 📁 NASZ KOD (utworzymy)
│   ├── api/                 # Komunikacja z API
│   ├── components/          # Komponenty UI
│   ├── screens/             # Ekrany aplikacji
│   ├── navigation/          # Nawigacja
│   ├── hooks/               # Custom hooks
│   ├── context/             # React Context
│   ├── types/               # TypeScript types
│   └── utils/               # Pomocnicze funkcje
├── App.tsx                  # Główny komponent
├── package.json             # Zależności
├── tsconfig.json            # Konfiguracja TypeScript
├── babel.config.js          # Konfiguracja Babel
├── metro.config.js          # Metro bundler config
└── .npmrc                   # Konfiguracja pnpm
```

### 6.3. Uruchomienie na Emulatorze

**Android:**
```bash
# 1. Uruchom emulator (przez Android Studio > Device Manager)
# 2. Uruchom aplikację
pnpm react-native run-android
```

**iOS (tylko Mac):**
```bash
# 1. Zainstaluj CocoaPods
cd ios && pod install && cd ..

# 2. Uruchom aplikację
pnpm react-native run-ios
```

**Pierwszy build potrwa 5-10 minut!**

---

## CZĘŚĆ 7: Najnowsze Trendy 2024-2025 (10 minut)

### 7.1. TypeScript Everywhere

```typescript
// 2015: JavaScript bez typów
function add(a, b) {
  return a + b;  // Co jeśli a="5" i b=3? "53"!
}

// 2025: TypeScript z pełnym typowaniem
function add(a: number, b: number): number {
  return a + b;  // TypeScript nie pozwoli na błąd
}
```

### 7.2. Hermes jako Standard

- ✅ Domyślny engine od RN 0.70+
- ✅ 10-20% szybszy startup
- ✅ 30% mniej RAM
- ✅ Bytecode precompilation
- ✅ Wbudowany debugger

### 7.3. New Architecture

- ✅ JSI - bezpośrednia komunikacja JS ↔ Native
- ✅ Fabric - synchroniczny renderer
- ✅ TurboModules - lazy loading
- ✅ Codegen - automatyczne generowanie typów

---

## 📝 Zadania Praktyczne

### Zadanie 1: Instalacja środowiska
Zainstaluj Node.js, pnpm, Android Studio i utwórz projekt testowy.

### Zadanie 2: Hello World
Zmodyfikuj `App.tsx` aby wyświetlał Twoje imię i datę.

### Zadanie 3: Eksploracja struktury
Przejrzyj folder `android/` i `ios/` - zidentyfikuj kluczowe pliki.

---

## 🔍 Pytania Kontrolne

1. Czym różni się React Native od Cordova/Ionic?
2. Co to jest Hermes i dlaczego jest lepszy?
3. Jak działa komunikacja między JS a kodem natywnym?
4. Kiedy użyć Expo, a kiedy bare React Native CLI?
5. Jakie są główne komponenty New Architecture?

---

## ➡️ Następna Lekcja

**[Lekcja 1: TypeScript - Fundamenty](./lekcja-01-typescript.md)**

W następnej lekcji:
- Typy podstawowe (string, number, boolean)
- Interfejsy i type aliases
- Funkcje z typowaniem
- Klasy i generyki
- Utility Types

---

## PODSUMOWANIE

**React Native w 2025:**
- ✅ 10 lat dojrzałości (2015-2025)
- ✅ Wspierany przez Meta (Facebook)
- ✅ TypeScript domyślnie
- ✅ Ogromna społeczność i ekosystem
- ✅ New Architecture = lepsza wydajność
- ✅ Świetna przyszłość

**Dlaczego warto się uczyć?**
1. Wiele firm używa (Discord, Instagram, Uber Eats...)
2. Jedna umiejętność = 2 platformy
3. JavaScript - łatwo się nauczyć
4. Wysokie zarobki
5. Szybki development cycle

---

**Gotowy na pełny kurs? Lecimy do TypeScript! 🚀**
