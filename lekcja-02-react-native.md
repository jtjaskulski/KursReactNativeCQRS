# Lekcja 2: React Native + TypeScript – Setup i Podstawy

**Moduł:** React Native Podstawy  
**Czas trwania:** 4 godziny  
**Poziom:** Początkujący/Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Zainstalować React Native CLI z TypeScript
- ✅ Skonfigurować środowisko (pnpm, emulator)
- ✅ Tworzyć komponenty funkcyjne z TypeScript
- ✅ Używać Props i State z typowaniem
- ✅ Stylować komponenty (StyleSheet, Flexbox)
- ✅ Implementować nawigację (React Navigation)
- ✅ Obsługiwać formularze i input

---

## CZĘŚĆ 1: Setup Środowiska (45 minut)

### 1.1. Wymagania Wstępne

**SCRIPT dla prowadzącego:**

> „Zanim zaczniemy kodować, upewnijmy się że każdy ma zainstalowane wszystko co potrzebne. React Native wymaga więcej setupu niż zwykły React – musimy mieć emulatory."

**Zainstalowane:**
- ✅ Node.js 18+ (`node --version`)
- ✅ pnpm (`npm install -g pnpm`)
- ✅ Android Studio + Android SDK (dla Android)
- ✅ Xcode (dla iOS - tylko Mac)
- ✅ Java JDK 17+ (`java --version`)

**Diagram narzędzi:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    ŚRODOWISKO REACT NATIVE                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   TWÓJ KOMPUTER                                                             │
│   ┌───────────────────────────────────────────────────────────────────┐     │
│   │                                                                    │     │
│   │   Node.js          pnpm             React Native CLI              │     │
│   │   ┌─────┐         ┌─────┐          ┌─────────────────┐            │     │
│   │   │ v20 │   +     │pkg  │    +     │ npx @react-na.. │            │     │
│   │   └─────┘         │mngr │          └─────────────────┘            │     │
│   │                   └─────┘                                          │     │
│   │                                                                    │     │
│   │   ANDROID                          iOS (tylko Mac)                 │     │
│   │   ┌─────────────────┐              ┌─────────────────┐            │     │
│   │   │ Android Studio  │              │     Xcode       │            │     │
│   │   │ • SDK Platform  │              │ • Simulator     │            │     │
│   │   │ • SDK Tools     │              │ • CocoaPods     │            │     │
│   │   │ • Emulator      │              └─────────────────┘            │     │
│   │   │ • Java JDK 17   │                                             │     │
│   │   └─────────────────┘                                             │     │
│   │                                                                    │     │
│   └───────────────────────────────────────────────────────────────────┘     │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Utworzenie Projektu z TypeScript

**SCRIPT dla prowadzącego:**

> „W React Native 0.82+ TypeScript jest wbudowany – nie potrzebujemy osobnego template'u. Po prostu tworzymy projekt i dostajemy TypeScript out of the box."

```bash
# Nowy projekt (TypeScript jest domyślny w RN 0.82+)
npx @react-native-community/cli init SolutionOrdersMobile

# Przejdź do folderu
cd SolutionOrdersMobile

# Zainstaluj zależności przez pnpm
pnpm install
```

**⚠️ Uwaga:** Jeśli widzisz `template.config.js not found` - to normalne, kontynuuj.

### 1.3. Konfiguracja pnpm dla React Native

**Utwórz plik `.npmrc`:**

```ini
node-linker=hoisted
```

**Po utworzeniu .npmrc:**

```bash
pnpm install
```

**SCRIPT dla prowadzącego:**

> „pnpm domyślnie tworzy symlinki zamiast kopiować pakiety - to oszczędza miejsce. Ale React Native tego nie lubi, bo potrzebuje 'płaskiej' struktury node_modules. Dlatego ustawiamy node-linker=hoisted."

### 1.4. Struktura Projektu

```
SolutionOrdersMobile/
├── android/              # Kod Android (Java/Kotlin)
├── ios/                  # Kod iOS (Swift/Objective-C)
├── node_modules/
├── src/                  # Nasz kod źródłowy (utworzymy)
│   ├── api/              # Komunikacja z backendem
│   ├── components/       # Komponenty wielokrotnego użytku
│   ├── screens/          # Ekrany aplikacji
│   ├── hooks/            # Custom hooks
│   ├── types/            # Typy TypeScript
│   └── navigation/       # Konfiguracja nawigacji
├── App.tsx               # Główny komponent (TypeScript!)
├── tsconfig.json         # Konfiguracja TypeScript
├── package.json
├── .npmrc
└── babel.config.js
```

### 1.5. Uruchomienie na Emulatorze Android

**Krok 1: Uruchom emulator**
- Otwórz Android Studio
- Tools → Device Manager
- Uruchom emulator (np. Pixel 7)

**Krok 2: Uruchom aplikację**

```bash
pnpm react-native run-android
```

**⏱️ Pierwszy build może potrwać 5-10 minut!**

**Jeśli się powiedzie:** aplikacja otworzy się na emulatorze z ekranem powitalnym.

### 1.6. Uruchomienie na iOS (tylko Mac)

```bash
cd ios
pod install
cd ..
pnpm react-native run-ios
```

### 1.7. Metro Bundler

**SCRIPT dla prowadzącego:**

> „Metro to serce development experience w React Native. Obserwuje pliki, bundluje JavaScript i wysyła do urządzenia. Hot Reload działa dzięki Metro."

**Metro** to bundler JavaScript dla React Native (jak Webpack dla web).

```bash
# Uruchom Metro osobno (jeśli potrzebne)
pnpm start

# Z czyszczeniem cache (gdy są problemy)
pnpm start --reset-cache
```

---

## CZĘŚĆ 2: Pierwszy Komponent z TypeScript (30 minut)

### 2.1. Podstawowy Komponent

**App.tsx:**

```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

function App(): React.JSX.Element {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Hello React Native!</Text>
      <Text style={styles.subtitle}>with TypeScript 🚀</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f5f5f5',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    marginTop: 8,
  },
});

export default App;
```

**SCRIPT dla prowadzącego:**

> „Zwróćcie uwagę na różnice od React Web: używamy View zamiast div, Text zamiast span, StyleSheet zamiast CSS. To podstawowe mapowanie."

**Podstawowe komponenty React Native:**

| Web (React) | Mobile (React Native) |
|-------------|----------------------|
| `<div>` | `<View>` |
| `<span>`, `<p>` | `<Text>` |
| `<button>` | `<Button>`, `<TouchableOpacity>` |
| `<input>` | `<TextInput>` |
| `<img>` | `<Image>` |
| `<ul>` | `<FlatList>`, `<ScrollView>` |

### 2.2. Komponent z Props

**src/components/Greeting.tsx:**

```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

// Interface dla Props (czyste typowanie!)
interface GreetingProps {
  name: string;
  age?: number;       // Opcjonalne
  isVip?: boolean;    // Opcjonalne z domyślną wartością
}

// Komponent z typowanymi Props
const Greeting: React.FC<GreetingProps> = ({ name, age, isVip = false }) => {
  return (
    <View style={styles.container}>
      <Text style={styles.greeting}>
        Cześć, {name}!
      </Text>
      {age && (
        <Text style={styles.age}>
          Masz {age} lat
        </Text>
      )}
      {isVip && (
        <Text style={styles.vip}>⭐ VIP</Text>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 16,
    backgroundColor: '#fff',
    borderRadius: 8,
    marginVertical: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,  // Android shadow
  },
  greeting: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
  },
  age: {
    fontSize: 14,
    color: '#666',
    marginTop: 4,
  },
  vip: {
    fontSize: 12,
    color: '#FFD700',
    marginTop: 4,
  },
});

export default Greeting;
```

**Użycie w App.tsx:**

```tsx
import Greeting from './src/components/Greeting';

function App(): React.JSX.Element {
  return (
    <View style={styles.container}>
      <Greeting name="Anna" age={25} />
      <Greeting name="Piotr" isVip={true} />
      <Greeting name="Kasia" />
    </View>
  );
}
```

### 2.3. Komponent ze State

**src/components/Counter.tsx:**

```tsx
import React, { useState } from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

const Counter: React.FC = () => {
  // useState z typem (inferencja automatyczna)
  const [count, setCount] = useState<number>(0);

  const increment = (): void => setCount(prev => prev + 1);
  const decrement = (): void => setCount(prev => prev - 1);
  const reset = (): void => setCount(0);

  return (
    <View style={styles.container}>
      <Text style={styles.count}>{count}</Text>

      <View style={styles.buttons}>
        <TouchableOpacity style={styles.button} onPress={decrement}>
          <Text style={styles.buttonText}>−</Text>
        </TouchableOpacity>

        <TouchableOpacity style={[styles.button, styles.resetButton]} onPress={reset}>
          <Text style={styles.buttonText}>Reset</Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.button} onPress={increment}>
          <Text style={styles.buttonText}>+</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
    borderRadius: 12,
    alignItems: 'center',
  },
  count: {
    fontSize: 64,
    fontWeight: 'bold',
    color: '#007AFF',
    marginBottom: 20,
  },
  buttons: {
    flexDirection: 'row',
    gap: 12,
  },
  button: {
    width: 60,
    height: 60,
    backgroundColor: '#007AFF',
    borderRadius: 30,
    justifyContent: 'center',
    alignItems: 'center',
  },
  resetButton: {
    width: 80,
    backgroundColor: '#FF3B30',
    borderRadius: 8,
  },
  buttonText: {
    color: '#fff',
    fontSize: 24,
    fontWeight: 'bold',
  },
});

export default Counter;
```

---

## CZĘŚĆ 3: Stylowanie w React Native (30 minut)

### 3.1. StyleSheet vs Inline Styles

**SCRIPT dla prowadzącego:**

> „W React Native ZAWSZE używamy StyleSheet.create() zamiast inline styles. Jest to zoptymalizowane - style są przetwarzane raz i cache'owane."

**✅ StyleSheet (ZALECANE):**

```tsx
const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
  },
});

<View style={styles.container} />
```

**⚠️ Inline styles (dla dynamicznych wartości):**

```tsx
<View style={{ padding: dynamicPadding, backgroundColor: isActive ? '#green' : '#gray' }} />
```

**✅ Łączenie stylów:**

```tsx
<View style={[styles.container, styles.centered, { marginTop: 10 }]} />
```

### 3.2. Flexbox - Layout System

**Diagram Flexbox:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          FLEXBOX W REACT NATIVE                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   flexDirection: 'column' (domyślnie)    flexDirection: 'row'               │
│   ┌───────────────────┐                  ┌───────────────────┐              │
│   │ ┌───────────────┐ │                  │ ┌───┐ ┌───┐ ┌───┐ │              │
│   │ │     Item 1    │ │                  │ │ 1 │ │ 2 │ │ 3 │ │              │
│   │ └───────────────┘ │                  │ └───┘ └───┘ └───┘ │              │
│   │ ┌───────────────┐ │                  └───────────────────┘              │
│   │ │     Item 2    │ │                                                      │
│   │ └───────────────┘ │                                                      │
│   │ ┌───────────────┐ │                                                      │
│   │ │     Item 3    │ │                                                      │
│   │ └───────────────┘ │                                                      │
│   └───────────────────┘                                                      │
│                                                                              │
│   justifyContent (główna oś)         alignItems (poprzeczna oś)             │
│   ─────────────────────────          ───────────────────────────            │
│   'flex-start'   │ na początku       'flex-start'   │ na początku          │
│   'center'       │ na środku         'center'       │ na środku            │
│   'flex-end'     │ na końcu          'flex-end'     │ na końcu             │
│   'space-between'│ równe odstępy     'stretch'      │ rozciągnij           │
│   'space-around' │ odstępy dookoła                                          │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

**Przykład:**

```tsx
const styles = StyleSheet.create({
  container: {
    flex: 1,                    // Zajmuje całą dostępną przestrzeń
    flexDirection: 'column',    // Elementy jeden pod drugim (domyślnie)
    justifyContent: 'center',   // Wyśrodkowane pionowo
    alignItems: 'center',       // Wyśrodkowane poziomo
    padding: 20,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%',
  },
});
```

### 3.3. Responsywne Wymiary

**Dimensions API:**

```tsx
import { Dimensions, useWindowDimensions } from 'react-native';

// Statyczne (nie aktualizuje się przy obrocie)
const { width, height } = Dimensions.get('window');

// Hook (aktualizuje się przy obrocie) - ZALECANE
const MyComponent: React.FC = () => {
  const { width, height } = useWindowDimensions();

  return (
    <View style={{ width: width * 0.9, height: height / 3 }}>
      {/* 90% szerokości, 1/3 wysokości */}
    </View>
  );
};
```

---

## CZĘŚĆ 4: Lista i FlatList (30 minut)

### 4.1. FlatList - Zoptymalizowana Lista

**SCRIPT dla prowadzącego:**

> „FlatList to JEDYNY sposób na wyświetlanie długich list w React Native. Używa virtualizacji - renderuje tylko widoczne elementy. Dla 10 000 produktów zużyje tyle samo pamięci co dla 10."

**src/components/ItemList.tsx:**

```tsx
import React from 'react';
import { View, Text, FlatList, StyleSheet, TouchableOpacity } from 'react-native';

interface Item {
  id: string;
  name: string;
  price: number;
  category: string;
}

interface ItemListProps {
  items: Item[];
  onItemPress?: (item: Item) => void;
}

const ItemList: React.FC<ItemListProps> = ({ items, onItemPress }) => {
  const renderItem = ({ item }: { item: Item }) => (
    <TouchableOpacity
      style={styles.item}
      onPress={() => onItemPress?.(item)}
    >
      <View style={styles.itemContent}>
        <Text style={styles.name}>{item.name}</Text>
        <Text style={styles.category}>{item.category}</Text>
      </View>
      <Text style={styles.price}>{item.price.toFixed(2)} zł</Text>
    </TouchableOpacity>
  );

  const renderSeparator = () => <View style={styles.separator} />;

  const renderEmpty = () => (
    <View style={styles.empty}>
      <Text style={styles.emptyText}>Brak produktów</Text>
    </View>
  );

  return (
    <FlatList
      data={items}
      renderItem={renderItem}
      keyExtractor={(item) => item.id}
      ItemSeparatorComponent={renderSeparator}
      ListEmptyComponent={renderEmpty}
      contentContainerStyle={styles.list}
    />
  );
};

const styles = StyleSheet.create({
  list: {
    padding: 16,
  },
  item: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 16,
    backgroundColor: '#fff',
    borderRadius: 8,
  },
  itemContent: {
    flex: 1,
  },
  name: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
  },
  category: {
    fontSize: 12,
    color: '#999',
    marginTop: 4,
  },
  price: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#007AFF',
  },
  separator: {
    height: 8,
  },
  empty: {
    padding: 40,
    alignItems: 'center',
  },
  emptyText: {
    color: '#999',
    fontSize: 16,
  },
});

export default ItemList;
```

---

## CZĘŚĆ 5: Formularze i TextInput (30 minut)

### 5.1. Prosty Formularz

**src/components/SimpleForm.tsx:**

```tsx
import React, { useState } from 'react';
import {
  View,
  TextInput,
  TouchableOpacity,
  Text,
  StyleSheet,
  Alert,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';

interface FormData {
  name: string;
  email: string;
  phone: string;
}

const SimpleForm: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    name: '',
    email: '',
    phone: '',
  });

  const [errors, setErrors] = useState<Partial<FormData>>({});

  const validate = (): boolean => {
    const newErrors: Partial<FormData> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Imię jest wymagane';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email jest wymagany';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Nieprawidłowy email';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (): void => {
    if (validate()) {
      Alert.alert('Sukces', `Wysłano: ${formData.name} (${formData.email})`);
      setFormData({ name: '', email: '', phone: '' });
    }
  };

  const updateField = (field: keyof FormData, value: string): void => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Wyczyść błąd przy edycji
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
    >
      <View style={styles.form}>
        <Text style={styles.title}>Formularz kontaktowy</Text>

        {/* Imię */}
        <View style={styles.field}>
          <Text style={styles.label}>Imię *</Text>
          <TextInput
            style={[styles.input, errors.name && styles.inputError]}
            value={formData.name}
            onChangeText={(text) => updateField('name', text)}
            placeholder="Wpisz imię"
            placeholderTextColor="#999"
          />
          {errors.name && <Text style={styles.error}>{errors.name}</Text>}
        </View>

        {/* Email */}
        <View style={styles.field}>
          <Text style={styles.label}>Email *</Text>
          <TextInput
            style={[styles.input, errors.email && styles.inputError]}
            value={formData.email}
            onChangeText={(text) => updateField('email', text)}
            placeholder="Wpisz email"
            keyboardType="email-address"
            autoCapitalize="none"
            autoCorrect={false}
            placeholderTextColor="#999"
          />
          {errors.email && <Text style={styles.error}>{errors.email}</Text>}
        </View>

        {/* Telefon */}
        <View style={styles.field}>
          <Text style={styles.label}>Telefon</Text>
          <TextInput
            style={styles.input}
            value={formData.phone}
            onChangeText={(text) => updateField('phone', text)}
            placeholder="Wpisz telefon"
            keyboardType="phone-pad"
            placeholderTextColor="#999"
          />
        </View>

        {/* Submit */}
        <TouchableOpacity style={styles.button} onPress={handleSubmit}>
          <Text style={styles.buttonText}>Wyślij</Text>
        </TouchableOpacity>
      </View>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1 },
  form: { padding: 20 },
  title: { fontSize: 24, fontWeight: 'bold', marginBottom: 24, color: '#333' },
  field: { marginBottom: 16 },
  label: { fontSize: 14, fontWeight: '600', marginBottom: 8, color: '#333' },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 12,
    borderRadius: 8,
    fontSize: 16,
    backgroundColor: '#fff',
    color: '#333',
  },
  inputError: { borderColor: '#FF3B30', borderWidth: 2 },
  error: { color: '#FF3B30', fontSize: 12, marginTop: 4 },
  button: {
    backgroundColor: '#007AFF',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 8,
  },
  buttonText: { color: '#fff', fontSize: 16, fontWeight: '600' },
});

export default SimpleForm;
```

---

## CZĘŚĆ 6: Nawigacja z React Navigation (45 minut)

### 6.1. Instalacja React Navigation

**SCRIPT dla prowadzącego:**

> „React Navigation to de facto standard do nawigacji w React Native. Ma kilka typów nawigatorów: Stack (ekrany nakładane), Tab (dolne zakładki), Drawer (menu wysuwane). Zaczynamy od Stack."

**Krok 1: Zainstaluj główne pakiety**

```bash
pnpm add @react-navigation/native @react-navigation/native-stack
```

**Krok 2: Zainstaluj zależności natywne**

```bash
pnpm add react-native-screens react-native-safe-area-context
```

**Krok 3: Android - konfiguracja natywna**

Edytuj `android/app/src/main/java/com/solutionordersmobile/MainActivity.kt`:

```kotlin
package com.solutionordersmobile

import android.os.Bundle
import com.facebook.react.ReactActivity
import com.facebook.react.ReactActivityDelegate
import com.facebook.react.defaults.DefaultNewArchitectureEntryPoint.fabricEnabled
import com.facebook.react.defaults.DefaultReactActivityDelegate

class MainActivity : ReactActivity() {

  override fun getMainComponentName(): String = "SolutionOrdersMobile"

  override fun onCreate(savedInstanceState: Bundle?) {
    super.onCreate(null)  // WAŻNE: null zamiast savedInstanceState
  }

  override fun createReactActivityDelegate(): ReactActivityDelegate =
      DefaultReactActivityDelegate(this, mainComponentName, fabricEnabled)
}
```

**Krok 4: iOS - instalacja podów (tylko Mac)**

```bash
cd ios && pod install && cd ..
```

**Krok 5: Rebuild aplikacji**

```bash
pnpm react-native run-android
```

**⚠️ WAŻNE:** Po instalacji natywnych pakietów ZAWSZE trzeba przebudować aplikację!

### 6.2. Typy dla Nawigacji

**src/navigation/types.ts:**

```typescript
export type RootStackParamList = {
  Home: undefined;
  Details: { itemId: number; itemName: string };
  Profile: { userId: number };
};
```

### 6.3. Stack Navigator

**src/navigation/RootNavigator.tsx:**

```tsx
import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

import HomeScreen from '../screens/HomeScreen';
import DetailsScreen from '../screens/DetailsScreen';
import { RootStackParamList } from './types';

const Stack = createNativeStackNavigator<RootStackParamList>();

function RootNavigator(): React.JSX.Element {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="Home"
        screenOptions={{
          headerStyle: { backgroundColor: '#007AFF' },
          headerTintColor: '#fff',
          headerTitleStyle: { fontWeight: 'bold' },
        }}
      >
        <Stack.Screen
          name="Home"
          component={HomeScreen}
          options={{ title: 'Strona Główna' }}
        />
        <Stack.Screen
          name="Details"
          component={DetailsScreen}
          options={{ title: 'Szczegóły' }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

export default RootNavigator;
```

### 6.4. Ekran Home z Nawigacją

**src/screens/HomeScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Home'>;

const HomeScreen: React.FC<Props> = ({ navigation }) => {
  const goToDetails = (): void => {
    navigation.navigate('Details', {
      itemId: 123,
      itemName: 'Laptop Dell',
    });
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Ekran Główny</Text>
      <Text style={styles.subtitle}>Witaj w aplikacji!</Text>

      <TouchableOpacity style={styles.button} onPress={goToDetails}>
        <Text style={styles.buttonText}>Zobacz szczegóły produktu</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 20 },
  title: { fontSize: 28, fontWeight: 'bold', marginBottom: 8, color: '#333' },
  subtitle: { fontSize: 16, color: '#666', marginBottom: 32 },
  button: { backgroundColor: '#007AFF', paddingHorizontal: 24, paddingVertical: 12, borderRadius: 8 },
  buttonText: { color: '#fff', fontSize: 16, fontWeight: '600' },
});

export default HomeScreen;
```

### 6.5. Ekran Details z Parametrami

**src/screens/DetailsScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Details'>;

const DetailsScreen: React.FC<Props> = ({ route, navigation }) => {
  // Typowane parametry z route
  const { itemId, itemName } = route.params;

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Szczegóły Produktu</Text>

      <View style={styles.card}>
        <Text style={styles.label}>ID:</Text>
        <Text style={styles.value}>{itemId}</Text>

        <Text style={styles.label}>Nazwa:</Text>
        <Text style={styles.value}>{itemName}</Text>
      </View>

      <TouchableOpacity
        style={styles.button}
        onPress={() => navigation.goBack()}
      >
        <Text style={styles.buttonText}>← Wróć</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, padding: 20 },
  title: { fontSize: 24, fontWeight: 'bold', marginBottom: 20, color: '#333' },
  card: { backgroundColor: '#fff', padding: 16, borderRadius: 8, marginBottom: 20 },
  label: { fontSize: 12, color: '#999', marginTop: 8 },
  value: { fontSize: 18, color: '#333', fontWeight: '500' },
  button: { backgroundColor: '#f0f0f0', padding: 12, borderRadius: 8, alignItems: 'center' },
  buttonText: { color: '#007AFF', fontSize: 16, fontWeight: '600' },
});

export default DetailsScreen;
```

### 6.6. Główny App.tsx

```tsx
import React from 'react';
import RootNavigator from './src/navigation/RootNavigator';

const App: React.FC = () => {
  return <RootNavigator />;
};

export default App;
```

---

## CZĘŚĆ 7: useEffect i Lifecycle (30 minut)

### 7.1. Podstawy useEffect

**SCRIPT dla prowadzącego:**

> „useEffect to odpowiednik componentDidMount, componentDidUpdate i componentWillUnmount w klasowych komponentach. Drugi argument (tablica zależności) kontroluje kiedy się wykonuje."

```tsx
import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';

const DataLoader: React.FC = () => {
  const [data, setData] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    // Wykonuje się po montowaniu komponentu
    console.log('Component mounted');

    // Symulacja ładowania danych
    const timer = setTimeout(() => {
      setData('Dane załadowane!');
      setLoading(false);
    }, 2000);

    // Cleanup function
    return () => {
      console.log('Component unmounted');
      clearTimeout(timer);
    };
  }, []); // [] = wykonaj raz na starcie

  if (loading) {
    return <ActivityIndicator size="large" color="#007AFF" />;
  }

  return <Text>{data}</Text>;
};
```

### 7.2. useEffect z Zależnościami

```tsx
const [count, setCount] = useState<number>(0);
const [message, setMessage] = useState<string>('');

// Wykonaj gdy count się zmieni
useEffect(() => {
  setMessage(`Licznik: ${count}`);
  console.log(`Count changed to: ${count}`);
}, [count]);

// Wykonaj przy każdym renderze (bez tablicy)
useEffect(() => {
  console.log('Rendered');
});
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Lista Produktów
Stwórz FlatList z listą produktów (nazwa, cena, kategoria). Dodaj nawigację do ekranu szczegółów.

### Zadanie 2: Formularz Logowania
Stwórz formularz z email i password, walidacją i przyciskiem "Zaloguj".

### Zadanie 3: Counter z Historią
Rozszerz Counter o historię zmian (lista poprzednich wartości).

---

## 🔍 Pytania Kontrolne

1. Czym różni się `View` od `div` w React?
2. Dlaczego używamy `StyleSheet.create()` zamiast inline styles?
3. Jak działa `flex: 1` w React Native?
4. Co to jest FlatList i dlaczego jest lepsza od map()?
5. Jak przekazujemy parametry między ekranami w React Navigation?
6. Kiedy wykonuje się useEffect z pustą tablicą zależności `[]`?

---

## ➡️ Następna Lekcja

**[Lekcja 3: .NET Backend – CQRS Setup](./lekcja-03-dotnet-cqrs.md)**

W następnej lekcji:
- Vertical Slice Architecture
- MediatR i CQRS pattern
- Entity Framework Core
- Pierwszy Query i Command

---

**Gratulacje! 🎉 Umiesz już React Native z TypeScript!**
