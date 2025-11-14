# Lekcja 2: React Native + TypeScript - Setup i Podstawy (4 godziny)

**Moduł:** React Native Podstawy  
**Czas trwania:** 4 godziny  
**Poziom:** Początkujący/Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Zainstalować React Native CLI z TypeScript
- ✅ Skonfigurować środowisko (pnpm, emulatdor)
- ✅ Tworzyć komponenty funkcyjne z TypeScript
- ✅ Używać Props i State z typowaniem
- ✅ Stylować komponenty (StyleSheet, Flexbox)
- ✅ Implementować nawigację (React Navigation)
- ✅ Obsługiwać formularze i input

---

## CZĘŚĆ 1: Setup Środowiska (45 minut)

### 1.1. Wymagania Wstępne

**Zainstalowane:**
- ✅ Node.js 18+ (`node --version`)
- ✅ pnpm (`npm install -g pnpm`)
- ✅ Android Studio + Android SDK (dla Android)
- ✅ Xcode (dla iOS - tylko Mac)

### 1.2. Instalacja React Native CLI

```bash
# Zainstaluj globalnie
npm install -g react-native-cli

# Sprawdź
react-native --version
```

### 1.3. Utworzenie Projektu z TypeScript

```bash
# Nowy projekt z TypeScript template
npx @react-native-community/cli init SolutionOrdersMobile --template react-native-template-typescript

# Przejdź do folderu
cd SolutionOrdersMobile

# Zainstaluj zależności przez pnpm
pnpm install
```

### 1.4. Konfiguracja pnpm dla React Native

**Utwórz plik `.npmrc`:**
```ini
node-linker=hoisted
```

**Dlaczego?** React Native potrzebuje "płaskiej" struktury `node_modules`. pnpm domyślnie tworzy symlinki, ale `node-linker=hoisted` mówi aby działać jak npm.

### 1.5. Struktura Projektu

```
SolutionOrdersMobile/
├── android/              # Kod Android (Java/Kotlin)
├── ios/                  # Kod iOS (Swift/Objective-C)
├── node_modules/
├── src/                  # Nasz kod źródłowy (utworzymy)
│   ├── api/
│   ├── components/
│   ├── screens/
│   ├── types/
│   └── navigation/
├── App.tsx               # Główny komponent (TypeScript!)
├── tsconfig.json         # Konfiguracja TypeScript
├── package.json
├── .npmrc
└── babel.config.js
```

### 1.6. Uruchomienie na Emulatorze Android

**Krok 1: Uruchom emulator**
- Otwórz Android Studio
- Tools → Device Manager
- Uruchom emulator (np. Pixel 7)

**Krok 2: Uruchom aplikację**
```bash
pnpm react-native run-android
```

**Pierwszy build może potrwać 5-10 minut!**

**Jeśli się powiedzie:** aplikacja otworzy się na emulatorze z ekranem powitalnym.

### 1.7. Uruchomienie na iOS (tylko Mac)

```bash
cd ios
pod install
cd ..
pnpm react-native run-ios
```

### 1.8. Metro Bundler

**Metro** to bundler JavaScript dla React Native (jak Webpack dla web).

Uruchamia się automatycznie z `run-android`/`run-ios`, ale możesz też:
```bash
pnpm start
# lub z czyszczeniem cache:
pnpm start --reset-cache
```

---

## CZĘŚĆ 2: Pierwszy Komponent z TypeScript (30 minut)

### 2.1. Podstawowy Komponent

**App.tsx:**
```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

function App(): JSX.Element {
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

**Wyjaśnienie:**
- `JSX.Element` - typ zwracany przez komponent React
- `StyleSheet.create()` - tworzy zoptymalizowane style
- `flex: 1` - komponent zajmuje całą dostępną przestrzeń

### 2.2. Komponent z Props

**Utwórz:** `src/components/Greeting.tsx`

```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

// Interface dla Props
interface GreetingProps {
  name: string;
  age?: number;  // Opcjonalne
  isVip?: boolean;
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

function App(): JSX.Element {
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
import { View, Text, Button, StyleSheet } from 'react-native';

const Counter: React.FC = () => {
  // useState z typem (inferencja automatyczna)
  const [count, setCount] = useState<number>(0);

  const increment = (): void => {
    setCount(count + 1);
  };

  const decrement = (): void => {
    setCount(count - 1);
  };

  const reset = (): void => {
    setCount(0);
  };

  return (
    <View style={styles.container}>
      <Text style={styles.count}>{count}</Text>
      <View style={styles.buttons}>
        <Button title="+" onPress={increment} />
        <Button title="-" onPress={decrement} />
        <Button title="Reset" onPress={reset} />
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
    borderRadius: 8,
    alignItems: 'center',
  },
  count: {
    fontSize: 48,
    fontWeight: 'bold',
    color: '#007AFF',
    marginBottom: 20,
  },
  buttons: {
    flexDirection: 'row',
    gap: 10,
  },
});

export default Counter;
```

---

## CZĘŚĆ 3: Stylowanie w React Native (30 minut)

### 3.1. StyleSheet vs Inline Styles

**StyleSheet (ZALECANE):**
```tsx
const styles = StyleSheet.create({
  container: {
    padding: 20,
  },
});

<View style={styles.container} />
```

**Inline styles:**
```tsx
<View style={{ padding: 20, backgroundColor: '#fff' }} />
```

### 3.2. Flexbox - Layout System

React Native używa **Flexbox** do layoutu (podobnie jak CSS Flexbox).

**Podstawy:**
```tsx
const styles = StyleSheet.create({
  container: {
    flex: 1,                    // Zajmuje całą przestrzeń
    flexDirection: 'column',    // 'row' | 'column' (domyślnie column)
    justifyContent: 'center',   // Wyrównanie głównej osi
    alignItems: 'center',       // Wyrównanie poprzecznej osi
  },
});
```

**Przykład - 3 przyciski obok siebie:**
```tsx
<View style={styles.row}>
  <Button title="A" />
  <Button title="B" />
  <Button title="C" />
</View>

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    padding: 20,
  },
});
```

### 3.3. Responsywne Wymiary

**Dimensions API:**
```tsx
import { Dimensions } from 'react-native';

const { width, height } = Dimensions.get('window');

const styles = StyleSheet.create({
  container: {
    width: width * 0.9,   // 90% szerokości ekranu
    height: height / 2,   // Połowa wysokości
  },
});
```

**Relative units:**
```tsx
const styles = StyleSheet.create({
  container: {
    width: '90%',       // Procent rodzica
    padding: 20,        // Pixels (pt)
  },
});
```

---

## CZĘŚĆ 4: Lista i FlatList (30 minut)

### 4.1. Prosty Map

**⚠️ NIE ZALECANE dla długich list:**
```tsx
const items = ['Jabłko', 'Banan', 'Pomarańcza'];

{items.map((item, index) => (
  <Text key={index}>{item}</Text>
))}
```

### 4.2. FlatList (ZALECANE)

**FlatList** = zoptymalizowana lista (lazy loading, virtualizacja)

```tsx
import React from 'react';
import { View, Text, FlatList, StyleSheet } from 'react-native';

interface Item {
  id: string;
  name: string;
  price: number;
}

const ItemList: React.FC = () => {
  const items: Item[] = [
    { id: '1', name: 'Laptop', price: 3000 },
    { id: '2', name: 'Monitor', price: 800 },
    { id: '3', name: 'Mysz', price: 50 },
  ];

  const renderItem = ({ item }: { item: Item }) => (
    <View style={styles.item}>
      <Text style={styles.name}>{item.name}</Text>
      <Text style={styles.price}>{item.price} zł</Text>
    </View>
  );

  return (
    <FlatList
      data={items}
      renderItem={renderItem}
      keyExtractor={(item) => item.id}
      style={styles.list}
    />
  );
};

const styles = StyleSheet.create({
  list: {
    flex: 1,
  },
  item: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  name: {
    fontSize: 16,
    fontWeight: '500',
  },
  price: {
    fontSize: 16,
    color: '#007AFF',
  },
});

export default ItemList;
```

---

## CZĘŚĆ 5: Formularze i TextInput (30 minut)

### 5.1. Prosty Formularz

```tsx
import React, { useState } from 'react';
import { View, TextInput, Button, Text, StyleSheet } from 'react-native';

interface FormData {
  name: string;
  email: string;
}

const SimpleForm: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    name: '',
    email: '',
  });

  const [submitted, setSubmitted] = useState<boolean>(false);

  const handleSubmit = (): void => {
    console.log('Submitted:', formData);
    setSubmitted(true);
  };

  return (
    <View style={styles.container}>
      <Text style={styles.label}>Imię:</Text>
      <TextInput
        style={styles.input}
        value={formData.name}
        onChangeText={(text) => setFormData({ ...formData, name: text })}
        placeholder="Wpisz imię"
      />

      <Text style={styles.label}>Email:</Text>
      <TextInput
        style={styles.input}
        value={formData.email}
        onChangeText={(text) => setFormData({ ...formData, email: text })}
        placeholder="Wpisz email"
        keyboardType="email-address"
        autoCapitalize="none"
      />

      <Button title="Wyślij" onPress={handleSubmit} />

      {submitted && (
        <Text style={styles.success}>
          Wysłano: {formData.name} ({formData.email})
        </Text>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
  },
  label: {
    fontSize: 16,
    fontWeight: '500',
    marginBottom: 8,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 12,
    borderRadius: 8,
    marginBottom: 16,
    fontSize: 16,
  },
  success: {
    marginTop: 16,
    color: 'green',
    fontSize: 14,
  },
});

export default SimpleForm;
```

---

## CZĘŚĆ 6: Nawigacja z React Navigation (45 minut)

### 6.1. Instalacja React Navigation

```bash
pnpm add @react-navigation/native
pnpm add @react-navigation/native-stack
pnpm add react-native-screens react-native-safe-area-context
```

**Android - dodatkowa konfiguracja:**

Edytuj `android/app/src/main/java/.../MainActivity.java`:

```java
import android.os.Bundle;

public class MainActivity extends ReactActivity {
  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(null);
  }
  // ...
}
```

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
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

import HomeScreen from '../screens/HomeScreen';
import DetailsScreen from '../screens/DetailsScreen';
import { RootStackParamList } from './types';

const Stack = createNativeStackNavigator<RootStackParamList>();

const RootNavigator: React.FC = () => {
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
};

export default RootNavigator;
```

### 6.4. Ekran Home z Nawigacją

**src/screens/HomeScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';
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
      <Button title="Zobacz szczegóły" onPress={goToDetails} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
  },
});

export default HomeScreen;
```

### 6.5. Ekran Details z Parametrami

**src/screens/DetailsScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Details'>;

const DetailsScreen: React.FC<Props> = ({ route, navigation }) => {
  const { itemId, itemName } = route.params;

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Szczegóły Produktu</Text>
      <Text style={styles.info}>ID: {itemId}</Text>
      <Text style={styles.info}>Nazwa: {itemName}</Text>
      <Button title="Wróć" onPress={() => navigation.goBack()} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
  },
  info: {
    fontSize: 16,
    marginBottom: 10,
  },
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
    setTimeout(() => {
      setData('Dane załadowane!');
      setLoading(false);
    }, 2000);

    // Cleanup function (opcjonalnie)
    return () => {
      console.log('Component unmounted');
    };
  }, []); // [] = wykonaj raz na starcie

  if (loading) {
    return <ActivityIndicator size="large" />;
  }

  return <Text>{data}</Text>;
};
```

### 7.2. useEffect z Zależnościami

```tsx
const [count, setCount] = useState<number>(0);

useEffect(() => {
  console.log(`Count changed to: ${count}`);
}, [count]); // Wykonaj gdy count się zmieni
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

## ➡️ Następna Lekcja

**[Lekcja 3: .NET Backend - CQRS Setup](./lekcja-03-dotnet-cqrs.md)**

---

**Gratulacje! 🎉 Umiesz już React Native z TypeScript!**
