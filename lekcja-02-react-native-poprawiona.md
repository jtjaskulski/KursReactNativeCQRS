# Lekcja 2: React Native + TypeScript - Setup i Podstawy (4 godziny)

**Moduł:** React Native Podstawy  
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

**Zainstalowane:**
- ✅ Node.js 18+ (`node --version`)
- ✅ pnpm (`npm install -g pnpm`)
- ✅ Android Studio + Android SDK (dla Android)
- ✅ Xcode (dla iOS - tylko Mac)

**Weryfikacja:**
```bash
node --version        # v18.x.x lub nowszy
pnpm --version       # 9.x.x lub nowszy
java -version        # Java 11+
```

### 1.2. Instalacja React Native CLI

```bash
# Zainstaluj globalnie
npm install -g react-native-cli

# Sprawdź wersję
react-native --version    # Powinna być 0.82.1 lub nowsza
```

### 1.3. Utworzenie Projektu (Metoda 1 - Rekomendowana)

**React Native 0.82 ma TypeScript wbudowany, więc NIE używamy template'u!**

```bash
# Krok 1: Utwórz projekt (bez template'u!)
npx @react-native-community/cli init SolutionOrdersMobile

# Krok 2: Przejdź do folderu
cd SolutionOrdersMobile

# Krok 3: Zainstaluj zależności przez pnpm
pnpm install

# Krok 4: Utwórz plik .npmrc
echo "node-linker=hoisted" > .npmrc

# Krok 5: Ponownie zainstaluj (dla .npmrc)
pnpm install
```

**⚠️ WAŻNE:** Jeśli dostajesz błąd `template.config.js not found` - to normalne dla RN 0.82. Ignoruj go!

### 1.4. Struktura Projektu

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
├── AppRegistry.tsx       # Registry komponentów
├── tsconfig.json         # Konfiguracja TypeScript
├── package.json
├── .npmrc
├── babel.config.js
├── jest.config.js
└── metro.config.js
```

### 1.5. Weryfikacja Instalacji

```bash
# Sprawdź czy pliki .tsx istnieją
ls -la *.tsx             # Powinna być App.tsx

# Sprawdź tsconfig.json
cat tsconfig.json        # Powinna zawierać konfigurację TS
```

### 1.6. Konfiguracja .npmrc (Ważne!)

**Utwórz lub edytuj `.npmrc` w głównym katalogu:**

```ini
node-linker=hoisted
```

**Dlaczego?** React Native potrzebuje "płaskiej" struktury `node_modules`. pnpm domyślnie tworzy symlinki, ale `node-linker=hoisted` mówi aby działać jak npm.

---

## CZĘŚĆ 2: Uruchomienie na Emulatorze (40 minut)

### 2.1. Android - Uruchomienie Emulatora

**Opcja 1: Przez Android Studio (Rekomendowany)**

1. Otwórz **Android Studio**
2. Kliknij **Device Manager** (ikona telefonu)
3. Kliknij **+ Create Device**
4. Wybierz: **Pixel 7** lub **Pixel 6**
5. Wybierz system: **Android 13** lub wyżej
6. Kliknij **Next** → **Finish**
7. Kliknij **Play** aby uruchomić emulator

**Czekaj aż emulator się całkowicie załaduje** (może potrwać 1-2 minuty)

### 2.2. Uruchomienie React Native na Androidzie

**W nowym terminalu (zostaw Android Studio otwarte):**

```bash
cd SolutionOrdersMobile

# Uruchom aplikację
pnpm react-native run-android
```

**Pierwszy build potrwa 5-10 minut!** Metro Bundler kompiluje cały JavaScript.

**Jeśli się powiedzie:**
- Emulator wykaże aplikację
- Zobaczysz ekran powitalny React Native
- Licznik w lewym dolnym rogu (reload)

### 2.3. iOS - Uruchomienie (Tylko Mac)

```bash
cd SolutionOrdersMobile

# Zainstaluj CocoaPods
cd ios && pod install && cd ..

# Uruchom na symulatorze
pnpm react-native run-ios
```

### 2.4. Metro Bundler - Jeśli Manual Start

Jeśli aplikacja nie uruchomiła się automatycznie:

```bash
# W innym terminalu:
pnpm start

# Wybierz opcję:
# i - uruchom na iOS
# a - uruchom na Android
# r - reload
# d - otwórz DevTools
```

### 2.5. Najczęstsze Problemy

**Problem: Port 8081 zajęty**
```bash
# Wyczyść cache
pnpm start --reset-cache

# Lub zabij proces
lsof -i :8081
kill -9 <PID>
```

**Problem: Emulator się nie ładuje**
```bash
# Wyłącz i uruchom ponownie
adb devices           # Zobacz listę
adb emu kill          # Wyłącz emulator
# Uruchom ponownie przez Android Studio
```

**Problem: Gradle build failure**
```bash
cd android
./gradlew clean
cd ..
pnpm react-native run-android
```

---

## CZĘŚĆ 3: Pierwszy Komponent z TypeScript (30 minut)

### 3.1. Struktura App.tsx

**App.tsx** (główny plik - już istnieje):
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
- `justifyContent: 'center'` - wyrównanie pionowe

### 3.2. Modyfikacja i Hot Reload

1. Otwórz `App.tsx`
2. Zmień `"Hello React Native!"` na `"Cześć React Native!"`
3. **Zapisz plik** (Ctrl+S)
4. W emulatorze powinny zobaczyć zmiany automatycznie!

---

## CZĘŚĆ 4: Komponenty z Props (35 minut)

### 4.1. Tworzenie folderu Components

```bash
mkdir -p src/components
```

### 4.2. Komponent Greeting z Props

**src/components/Greeting.tsx:**

```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

// TypeScript Interface dla Props
interface GreetingProps {
  name: string;
  age?: number;  // Opcjonalne (?)
  isVip?: boolean;
}

// Komponent funkcyjny z typowanymi Props
const Greeting: React.FC<GreetingProps> = ({ 
  name, 
  age, 
  isVip = false 
}) => {
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
    marginHorizontal: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.2,
    shadowRadius: 2,
    elevation: 3,
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
    fontWeight: '600',
  },
});

export default Greeting;
```

### 4.3. Użycie Komponentu w App.tsx

**App.tsx** (zaktualizuj):
```tsx
import React from 'react';
import { View, ScrollView, StyleSheet } from 'react-native';
import Greeting from './src/components/Greeting';

function App(): JSX.Element {
  return (
    <ScrollView style={styles.container}>
      <Greeting name="Anna" age={25} />
      <Greeting name="Piotr" isVip={true} />
      <Greeting name="Kasia" age={30} isVip={true} />
      <Greeting name="Jan" />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
});

export default App;
```

**Wyjaśnienie:**
- `ScrollView` - pozwala scrollować zawartość
- Props można przekazywać z wartościami lub zmiennymi
- TypeScript automatycznie sprawdzi typy!

---

## CZĘŚĆ 5: State z useState Hook (35 minut)

### 5.1. Komponent Counter

**src/components/Counter.tsx:**

```tsx
import React, { useState } from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';

// Komponent bez Props - tylko State
const Counter: React.FC = () => {
  // useState z TypeScript - type inference
  const [count, setCount] = useState<number>(0);

  // Funkcje obsługi
  const handleIncrement = (): void => {
    setCount(count + 1);
  };

  const handleDecrement = (): void => {
    setCount(count - 1);
  };

  const handleReset = (): void => {
    setCount(0);
  };

  return (
    <View style={styles.container}>
      {/* Wyświetlanie state'u */}
      <Text style={styles.count}>{count}</Text>

      {/* Przyciski */}
      <View style={styles.buttons}>
        <Button title="+" onPress={handleIncrement} />
        <Button title="-" onPress={handleDecrement} />
        <Button title="Reset" onPress={handleReset} />
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
    borderRadius: 8,
    marginHorizontal: 16,
    marginVertical: 16,
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
    justifyContent: 'center',
  },
});

export default Counter;
```

### 5.2. Dodaj Counter do App.tsx

```tsx
import Counter from './src/components/Counter';

function App(): JSX.Element {
  return (
    <ScrollView style={styles.container}>
      <Greeting name="Anna" age={25} />
      <Counter />  {/* Dodaj tutaj */}
      <Greeting name="Piotr" isVip={true} />
    </ScrollView>
  );
}
```

---

## CZĘŚĆ 6: Stylowanie w React Native (30 minut)

### 6.1. StyleSheet vs Inline Styles

**StyleSheet (ZALECANE):**
```tsx
const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
  },
});

<View style={styles.container} />
```

**Inline styles (NIE ZALECANE):**
```tsx
<View style={{ padding: 20, backgroundColor: '#fff' }} />
```

**Dlaczego StyleSheet?**
- ✅ Performance - style są zoptymalizowane
- ✅ Validation - TypeScript sprawdza typy
- ✅ Czytelność - oddzielone od kodu

### 6.2. Flexbox - Layout System

React Native używa **Flexbox** (jak CSS Flexbox):

```tsx
const styles = StyleSheet.create({
  // Kolumna (domyślnie)
  column: {
    flex: 1,
    flexDirection: 'column',       // góra ↓ dół
    justifyContent: 'center',      // wyrównanie głównej osi (pionowo)
    alignItems: 'center',          // wyrównanie boczne (poziomo)
  },

  // Rząd
  row: {
    flex: 1,
    flexDirection: 'row',          // lewo → prawo
    justifyContent: 'space-between', // rozłożone
    alignItems: 'center',
  },
});
```

### 6.3. Rozmiary i Spacing

```tsx
const styles = StyleSheet.create({
  // Stałe wymiary
  box: {
    width: 200,
    height: 200,
    backgroundColor: '#007AFF',
  },

  // Procenty
  halfWidth: {
    width: '50%',
    height: '100%',
  },

  // Spacing
  spacious: {
    padding: 20,           // wewnętrzny spacing
    margin: 10,            // zewnętrzny spacing
    marginTop: 5,          // specificzny
    paddingHorizontal: 15, // lewo + prawo
    paddingVertical: 10,   // góra + dół
  },

  // Granice i promienie
  rounded: {
    borderRadius: 8,       // zaokrąglone rogi
    borderWidth: 1,
    borderColor: '#ddd',
  },
});
```

### 6.4. Responsywne Wymiary

```tsx
import { Dimensions } from 'react-native';

const { width, height } = Dimensions.get('window');

const styles = StyleSheet.create({
  container: {
    width: width * 0.9,      // 90% szerokości ekranu
    height: height / 2,      // połowa wysokości
    backgroundColor: '#fff',
  },
});
```

---

## CZĘŚĆ 7: FlatList - Wydajne Listy (30 minut)

### 7.1. Prosty Map (NIE ZALECANE)

```tsx
const items = ['Jabłko', 'Banan', 'Pomarańcza'];

{items.map((item, index) => (
  <Text key={index}>{item}</Text>
))}
```

**❌ Problemy:**
- Bardzo wolne dla dużych list
- Przy usunięciu elementu - renderuje wszystko od nowa

### 7.2. FlatList (ZALECANE)

**src/components/ItemList.tsx:**

```tsx
import React from 'react';
import { View, Text, FlatList, StyleSheet } from 'react-native';

// TypeScript interface dlaItem
interface Item {
  id: string;
  name: string;
  price: number;
}

const ItemList: React.FC = () => {
  // Dane testowe
  const items: Item[] = [
    { id: '1', name: 'Laptop', price: 3000 },
    { id: '2', name: 'Monitor', price: 800 },
    { id: '3', name: 'Mysz', price: 50 },
    { id: '4', name: 'Klawiatura', price: 150 },
  ];

  // Render pojedynczego wiersza
  const renderItem = ({ item }: { item: Item }) => (
    <View style={styles.item}>
      <View style={styles.itemContent}>
        <Text style={styles.name}>{item.name}</Text>
        <Text style={styles.price}>{item.price} zł</Text>
      </View>
    </View>
  );

  return (
    <FlatList
      data={items}
      renderItem={renderItem}
      keyExtractor={(item) => item.id}  // Unikalny klucz
      style={styles.list}
      ItemSeparatorComponent={() => <View style={styles.separator} />}
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
    backgroundColor: '#fff',
  },
  itemContent: {
    flex: 1,
  },
  name: {
    fontSize: 16,
    fontWeight: '500',
    color: '#333',
  },
  price: {
    fontSize: 14,
    color: '#007AFF',
    marginTop: 4,
  },
  separator: {
    height: 1,
    backgroundColor: '#eee',
  },
});

export default ItemList;
```

**Wyjaśnienie:**
- `keyExtractor` - musi być unikalny!
- `renderItem` - renderuje jeden element
- `ItemSeparatorComponent` - linia separator między elementami
- `FlatList` automatycznie virtualizuje (renderuje tylko widoczne)

---

## CZĘŚĆ 8: Formularze i TextInput (35 minut)

### 8.1. Prosty Formularz

**src/components/SimpleForm.tsx:**

```tsx
import React, { useState } from 'react';
import {
  View,
  TextInput,
  Button,
  Text,
  StyleSheet,
  Alert,
} from 'react-native';

// TypeScript interface dla danych formularza
interface FormData {
  name: string;
  email: string;
}

const SimpleForm: React.FC = () => {
  // State dla formularza
  const [formData, setFormData] = useState<FormData>({
    name: '',
    email: '',
  });

  // State dla statusu
  const [submitted, setSubmitted] = useState(false);

  // Handler do zmiany pola
  const handleChangeName = (text: string): void => {
    setFormData({ ...formData, name: text });
  };

  const handleChangeEmail = (text: string): void => {
    setFormData({ ...formData, email: text });
  };

  // Handler do wysłania
  const handleSubmit = (): void => {
    if (!formData.name || !formData.email) {
      Alert.alert('Błąd', 'Wypełnij wszystkie pola!');
      return;
    }

    console.log('Submitted:', formData);
    setSubmitted(true);

    // Reset po 2 sekundach
    setTimeout(() => {
      setFormData({ name: '', email: '' });
      setSubmitted(false);
    }, 2000);
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Formularz Kontaktowy</Text>

      {/* Pole Imię */}
      <TextInput
        style={styles.input}
        placeholder="Wpisz imię"
        value={formData.name}
        onChangeText={handleChangeName}
        placeholderTextColor="#999"
      />

      {/* Pole Email */}
      <TextInput
        style={styles.input}
        placeholder="Wpisz email"
        value={formData.email}
        onChangeText={handleChangeEmail}
        keyboardType="email-address"
        autoCapitalize="none"
        placeholderTextColor="#999"
      />

      {/* Przycisk */}
      <Button title="Wyślij" onPress={handleSubmit} />

      {/* Komunikat sukcesu */}
      {submitted && (
        <Text style={styles.success}>
          ✓ Wysłano: {formData.name} ({formData.email})
        </Text>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
    backgroundColor: '#fff',
    marginHorizontal: 16,
    marginVertical: 16,
    borderRadius: 8,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 16,
    color: '#333',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 12,
    borderRadius: 8,
    marginBottom: 16,
    fontSize: 16,
    color: '#333',
  },
  success: {
    marginTop: 16,
    color: 'green',
    fontSize: 14,
    fontWeight: '600',
  },
});

export default SimpleForm;
```

### 8.2. Różne typy Input

```tsx
// Email
<TextInput
  keyboardType="email-address"
  autoCapitalize="none"
  placeholder="email@example.com"
/>

// Telefon
<TextInput
  keyboardType="phone-pad"
  placeholder="123456789"
/>

// Liczby
<TextInput
  keyboardType="numeric"
  placeholder="Wpisz liczbę"
/>

// Hasło
<TextInput
  secureTextEntry={true}
  placeholder="Wpisz hasło"
/>

// Wielowierszowy
<TextInput
  multiline={true}
  numberOfLines={4}
  placeholder="Wpisz wiadomość..."
/>
```

---

## CZĘŚĆ 9: React Navigation (45 minut)

### 9.1. Instalacja

```bash
pnpm add @react-navigation/native
pnpm add @react-navigation/native-stack
pnpm add react-native-screens react-native-safe-area-context
```

### 9.2. TypeScript Types

**src/navigation/types.ts:**

```typescript
export type RootStackParamList = {
  Home: undefined;
  Details: { 
    itemId: number; 
    itemName: string;
  };
  Profile: { 
    userId: number;
  };
};
```

### 9.3. Navigator - RootNavigator.tsx

**src/navigation/RootNavigator.tsx:**

```tsx
import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import {
  createNativeStackNavigator,
  NativeStackScreenProps,
} from '@react-navigation/native-stack';

import HomeScreen from '../screens/HomeScreen';
import DetailsScreen from '../screens/DetailsScreen';
import { RootStackParamList } from './types';

// Tworzenie Stack Navigator
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
        {/* Screen Home */}
        <Stack.Screen 
          name="Home" 
          component={HomeScreen}
          options={{ title: 'Strona Główna' }}
        />

        {/* Screen Details */}
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

### 9.4. Ekran Home z Nawigacją

**src/screens/HomeScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Home'>;

const HomeScreen: React.FC<Props> = ({ navigation }) => {
  const goToDetails = (): void => {
    // Nawigacja z parametrami
    navigation.navigate('Details', {
      itemId: 123,
      itemName: 'Laptop Dell',
    });
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Ekran Główny</Text>
      <Text style={styles.subtitle}>Witaj w aplikacji!</Text>

      <Button 
        title="Przejdź do szczegółów" 
        onPress={goToDetails}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
    backgroundColor: '#f5f5f5',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 10,
    color: '#333',
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    marginBottom: 20,
  },
});

export default HomeScreen;
```

### 9.5. Ekran Details z Parametrami

**src/screens/DetailsScreen.tsx:**

```tsx
import React from 'react';
import { View, Text, Button, StyleSheet } from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Details'>;

const DetailsScreen: React.FC<Props> = ({ route, navigation }) => {
  // Pobierz parametry z route
  const { itemId, itemName } = route.params;

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Szczegóły Produktu</Text>
      
      <Text style={styles.info}>ID: {itemId}</Text>
      <Text style={styles.info}>Nazwa: {itemName}</Text>

      <Button 
        title="Wróć" 
        onPress={() => navigation.goBack()}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    backgroundColor: '#f5f5f5',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
    color: '#333',
  },
  info: {
    fontSize: 16,
    color: '#666',
    marginBottom: 10,
  },
});

export default DetailsScreen;
```

### 9.6. Główny App.tsx z Nawigacją

**App.tsx** (zaktualizuj):

```tsx
import React from 'react';
import RootNavigator from './src/navigation/RootNavigator';

function App(): JSX.Element {
  return <RootNavigator />;
}

export default App;
```

---

## CZĘŚĆ 10: useEffect i Lifecycle (30 minut)

### 10.1. Podstawy useEffect

```tsx
import React, { useEffect } from 'react';
import { View, Text } from 'react-native';

const DataLoader: React.FC = () => {
  useEffect(() => {
    // Wykonuje się po montowaniu komponentu
    console.log('Component mounted');

    // Opcjonalnie: cleanup function
    return () => {
      console.log('Component unmounted');
    };
  }, []);  // [] = wykonaj raz na starcie

  return <Text>Hello</Text>;
};
```

### 10.2. useEffect z Zależnościami

```tsx
const [count, setCount] = useState<number>(0);

useEffect(() => {
  console.log(`Count changed to: ${count}`);
  
  // Cleanup (opcjonalnie)
  return () => {
    console.log('Cleanup before count changes');
  };
}, [count]);  // Wykonaj gdy count się zmieni
```

### 10.3. Asynchroniczny useEffect

```tsx
useEffect(() => {
  const loadData = async () => {
    try {
      const response = await fetch('https://api.example.com/data');
      const data = await response.json();
      setData(data);
    } catch (error) {
      console.error(error);
    }
  };

  loadData();
}, []);
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Rozszerz Greeting
Dodaj pole `email` do interfejsu `GreetingProps` i wyświetl je w komponencie.

### Zadanie 2: Counter z Historią
Rozszerz komponent `Counter` o tablicę historii poprzednich wartości. Wyświetl ostatnie 5 wartości.

### Zadanie 3: Formularz Rejestracji
Utwórz komponent `RegistrationForm` z polami: imię, email, hasło. Dodaj walidację.

### Zadanie 4: Lista z FlatList
Zbuduj komponent wyświetlający listę studentów pobraną z API (wykorzystaj fetch + useEffect).

### Zadanie 5: Nawigacja z listą
Stwórz ekran z listą produktów. Po kliknięciu → przejście do szczegółów produktu.

---

## ➡️ Następna Lekcja

**[Lekcja 3: .NET Backend - CQRS Setup](./lekcja-03-dotnet-cqrs.md)**

W następnej lekcji:
- Utworzymy projekt ASP.NET Core 8
- Zainstalujemy Entity Framework
- Stworzymy modele i DbContext
- Zaimplementujemy pierwszy Query CQRS

---

**Gratulacje! 🎉 Umiesz już React Native z TypeScript!**

Przesyłaj problemy jeśli coś nie zadziała! 💪
