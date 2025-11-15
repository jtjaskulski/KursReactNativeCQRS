# Lekcja 5: React Native - Integracja z API CQRS - ROZSZERZONA

**Moduł:** React Native + .NET Integration  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Stworzyć API Service z TypeScript
- ✅ Obsłużyć różne IP dla Android/iOS (10.0.2.2)
- ✅ Zaimplementować CRUD w React Native
- ✅ Stworzyć reużywalny komponent SimpleCrudList
- ✅ Zainicjalizować Items w App.tsx z Context API
- ✅ Wyświetlać i edytować dane z API

---

## CZĘŚĆ 1: Konfiguracja IP i Setup

### 1.1. Problem z localhost w React Native

**❌ NIE DZIAŁA:**
```typescript
const API_URL = 'http://localhost:5000/api';
```

**Dlaczego?**
- **localhost** w emulatorze = emulator sam, NIE komputer host!
- Android emulator ma specjalny IP dla hosta
- iOS simulator może używać localhost

### 1.2. Prawidłowe IP

**Android Emulator:**
```typescript
const API_URL = 'http://10.0.2.2:5000/api';
// 10.0.2.2 = specjalny alias dla localhost hosta
```

**iOS Simulator:**
```typescript
const API_URL = 'http://localhost:5000/api';
// lub IP komputera w sieci lokalnej:
// const API_URL = 'http://192.168.1.100:5000/api';
```

**Fizyczne urządzenie:**
```typescript
const API_URL = 'http://192.168.1.100:5000/api';
// IP komputera w tej samej sieci WiFi
```

### 1.3. Uniwersalne Rozwiązanie

**src/api/config.ts:**
```typescript
import { Platform } from 'react-native';

const getBaseUrl = (): string => {
  if (__DEV__) {
    // Development
    if (Platform.OS === 'android') {
      return 'http://10.0.2.2:5000/api';
    } else if (Platform.OS === 'ios') {
      return 'http://localhost:5000/api';
    }
  }
  
  // Production
  return 'https://your-production-api.com/api';
};

export const API_BASE_URL = getBaseUrl();
```

### 1.4. Konfiguracja dla Docker (WAŻNE!)

**Problem:** Gdy API działa w Dockerze, `10.0.2.2` i `localhost` NIE DZIAŁAJĄ dla Android Emulator!

**Rozwiązanie:** Użyj lokalnego IP swojego komputera.

#### Krok 1: Znajdź swoje IP

**Windows PowerShell:**
```powershell
ipconfig
```
Szukaj `IPv4 Address` dla `Ethernet adapter` lub `Wi-Fi` (np. `192.168.1.100`)

**macOS/Linux:**
```bash
ifconfig | grep "inet "
# lub
ip addr show
```

#### Krok 2: Zaktualizuj config.ts

**src/api/config.ts:**
```typescript
import { Platform } from 'react-native';

const getBaseUrl = (): string => {
  if (__DEV__) {
    // DOCKER: Użyj lokalnego IP komputera (nie localhost!)
    if (Platform.OS === 'android') {
      return 'http://192.168.1.100:5000/api';  // TWOJE IP!
    } else if (Platform.OS === 'ios') {
      return 'http://192.168.1.100:5000/api';  // TWOJE IP!
    }
  }
  
  // Production
  return 'https://your-production-api.com/api';
};

export const API_BASE_URL = getBaseUrl();

// Debug
console.log('API_BASE_URL:', API_BASE_URL);
console.log('Platform:', Platform.OS);
```

#### Krok 3: Sprawdź CORS w backendzie

Backend musi zezwalać na połączenia z aplikacji mobilnej.

**Program.cs:**
```csharp
// CORS - dla development zezwalaj na wszystkie połączenia
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ...

app.UseCors("AllowAll");
```

#### Krok 4: Test połączenia

1. **Sprawdź czy Docker działa:**
   ```bash
   docker ps
   ```

2. **Test API z przeglądarki:**
   ```
   http://localhost:5000/swagger
   http://192.168.1.100:5000/swagger
   ```

3. **Sprawdź logi w aplikacji:**
   - Otwórz React Native DevTools
   - Sprawdź `console.log('API_BASE_URL:', ...)`

#### Automatyczne znajdowanie IP

Możesz użyć skryptu PowerShell:

**find-host-ip.ps1:**
```powershell
$bestIP = (Get-NetIPAddress -AddressFamily IPv4 | 
    Where-Object { $_.IPAddress -notmatch '^(127\.|169\.254\.)' } | 
    Select-Object -First 1).IPAddress

Write-Host "Twoje IP: $bestIP" -ForegroundColor Green
Write-Host "Użyj: http://${bestIP}:5000/api" -ForegroundColor Yellow
```

**Uruchom:**
```powershell
cd rn/SolutionOrdersMobile
.\find-host-ip.ps1
```

### 1.5. adb reverse (Alternatywa dla Android bez Dockera)

```bash
# Przekieruj port 5000 z urządzenia na komputer
adb reverse tcp:5000 tcp:5000

# Teraz możesz używać localhost w Android!
const API_URL = 'http://localhost:5000/api';
```

**⚠️ Musisz to robić po każdym restarcie emulatora!**
**⚠️ NIE DZIAŁA z Dockerem - użyj lokalnego IP!**

---

## CZĘŚĆ 2: TypeScript Models

### 2.1. src/types/models.ts

```typescript
// Jednostka miary
export interface UnitOfMeasurement {
  idUnitOfMeasurement: number;
  name: string | null;
  description: string | null;
  isActive: boolean;
}

// Kategoria
export interface Category {
  idCategory: number;
  name: string | null;
  description: string | null;
  isActive: boolean;
}

// Klient
export interface Client {
  idClient: number;
  name: string | null;
  adress: string | null;  // Typo w bazie - zostawiamy
  phoneNumber: string | null;
  isActive: boolean;
}

// Pracownik
export interface Worker {
  idWorker: number;
  firstName: string | null;
  lastName: string | null;
  isActive: boolean;
  login: string;
  password?: string;
}

// Produkt (ItemDto z backendu)
export interface Item {
  idItem: number;
  name: string | null;
  description: string | null;
  idCategory: number;
  categoryName: string | null;
  price: number | null;
  quantity: number | null;
  idUnitOfMeasurement: number | null;
  unitName: string | null;
  code: string | null;
  isActive: boolean;
}

// Request types (dla Create/Update)
export interface CreateItemRequest {
  name: string;
  description?: string;
  idCategory: number;
  price?: number;
  quantity?: number;
  fotoUrl?: string;
  idUnitOfMeasurement?: number;
  code?: string;
}

export interface UpdateItemRequest extends CreateItemRequest {
  idItem: number;
  isActive: boolean;
}
```

---

## CZĘŚĆ 3: API Service z TypeScript

### 3.1. src/api/apiService.ts

```typescript

import { API_BASE_URL } from './config';
import type {
  UnitOfMeasurement,
  Category,
  Client,
  Item,
  CreateItemRequest,
  UpdateItemRequest,
} from '../types/models';

class ApiService {
  private baseUrl: string;

  constructor() {
    this.baseUrl = API_BASE_URL;
  }

  /**
   * Generyczny request handler
   */
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    // Create a Headers instance when available (covers Headers | string[][] | Record).
    // If a Headers constructor isn't present in the runtime/types, fall back to
    // a plain object merge so `fetch` still receives headers in an acceptable shape.
    const HeadersCtor = (globalThis as any).Headers;
    let headers: any;
    if (HeadersCtor) {
      headers = new HeadersCtor(options.headers as any);
      if (!headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
      }
    } else {
      headers = {
        'Content-Type': 'application/json',
        ...(options.headers as any),
      };
    }

    try {
      console.log(`API Request: ${options.method || 'GET'} ${url}`);

      const response = await fetch(url, {
        ...options,
        headers,
      });

      // Sprawdzenie statusu
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          `HTTP ${response.status}: ${errorText || response.statusText}`
        );
      }

      // Jeśli 204 No Content - nie parsuj JSON
      if (response.status === 204) {
        return {} as T;
      }

      const data = await response.json();
      console.log(`API Response:`, data);
      return data;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

  // ========== JEDNOSTKI MIARY ==========

  async getUnitOfMeasurements(): Promise<UnitOfMeasurement[]> {
    return this.request<UnitOfMeasurement[]>('/UnitOfMeasurement');
  }

  async getUnitOfMeasurement(id: number): Promise<UnitOfMeasurement> {
    return this.request<UnitOfMeasurement>(`/UnitOfMeasurement/${id}`);
  }

  async createUnitOfMeasurement(
    data: Omit<UnitOfMeasurement, 'idUnitOfMeasurement'>
  ): Promise<{ id: number }> {
    return this.request<{ id: number }>('/UnitOfMeasurement', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateUnitOfMeasurement(
    id: number,
    data: Partial<UnitOfMeasurement>
  ): Promise<void> {
    return this.request<void>(`/UnitOfMeasurement/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...data, idUnitOfMeasurement: id }),
    });
  }

  async deleteUnitOfMeasurement(id: number): Promise<void> {
    return this.request<void>(`/UnitOfMeasurement/${id}`, {
      method: 'DELETE',
    });
  }

  // ========== KATEGORIE ==========

  async getCategories(): Promise<Category[]> {
    return this.request<Category[]>('/Category');
  }

  async createCategory(
    data: Omit<Category, 'idCategory'>
  ): Promise<{ id: number }> {
    return this.request<{ id: number }>('/Category', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateCategory(
    id: number,
    data: Partial<Category>
  ): Promise<void> {
    return this.request<void>(`/Category/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...data, idCategory: id }),
    });
  }

  async deleteCategory(id: number): Promise<void> {
    return this.request<void>(`/Category/${id}`, {
      method: 'DELETE',
    });
  }

  // ========== PRODUKTY (ITEMS) ==========

  async getItems(): Promise<Item[]> {
    return this.request<Item[]>('/Items');
  }

  async getItem(id: number): Promise<Item> {
    return this.request<Item>(`/Items/${id}`);
  }

  async createItem(data: CreateItemRequest): Promise<{ id: number }> {
    return this.request<{ id: number }>('/Items', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateItem(id: number, data: UpdateItemRequest): Promise<void> {
    return this.request<void>(`/Items/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deleteItem(id: number): Promise<void> {
    return this.request<void>(`/Items/${id}`, {
      method: 'DELETE',
    });
  }

  // ========== KLIENCI ==========

  async getClients(): Promise<Client[]> {
    return this.request<Client[]>('/Client');
  }

  async createClient(data: Omit<Client, 'idClient'>): Promise<{ id: number }> {
    return this.request<{ id: number }>('/Client', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }
}


// Singleton
export default new ApiService(); 

```

---

## CZĘŚĆ 4: Context API dla Items

### 4.1. src/context/ItemsContext.tsx

```typescript
import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import apiService from '../api/apiService';
import type { Item, CreateItemRequest, UpdateItemRequest } from '../types/models';

interface ItemsContextType {
  items: Item[];
  loading: boolean;
  error: string | null;
  
  // Actions
  refreshItems: () => Promise<void>;
  createItem: (data: CreateItemRequest) => Promise<void>;
  updateItem: (id: number, data: UpdateItemRequest) => Promise<void>;
  deleteItem: (id: number) => Promise<void>;
}

const ItemsContext = createContext<ItemsContextType | undefined>(undefined);

export function ItemsProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Pobranie wszystkich produktów
  const refreshItems = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await apiService.getItems();
      setItems(data);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unknown error';
      setError(message);
      console.error('Failed to load items:', err);
    } finally {
      setLoading(false);
    }
  };

  // Tworzenie produktu
  const createItem = async (data: CreateItemRequest) => {
    try {
      setError(null);
      const result = await apiService.createItem(data);
      
      // Dodaj nowy item do listy (lokalnie bez refresh)
      // Mapujemy pola z CreateItemRequest na pełny Item, ustawiając
      // wartości domyślne (null/empty) tam gdzie request ma pola opcjonalne.
      const newItem: Item = {
        idItem: result.id,
        name: data.name,
        description: data.description ?? null,
        idCategory: data.idCategory,
        categoryName: '',
        price: data.price ?? null,
        quantity: data.quantity ?? null,
        idUnitOfMeasurement: data.idUnitOfMeasurement ?? null,
        unitName: '',
        code: data.code ?? null,
        isActive: true,
      };
      setItems(prev => [...prev, newItem]);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unknown error';
      setError(message);
      throw err;
    }
  };

  // Aktualizacja produktu
  const updateItem = async (id: number, data: UpdateItemRequest) => {
    try {
      setError(null);
      await apiService.updateItem(id, data);
      
      // Zaktualizuj lokalnie
      setItems(prev =>
        prev.map(item =>
          item.idItem === id
            ? { ...item, ...data }
            : item
        )
      );
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unknown error';
      setError(message);
      throw err;
    }
  };

  // Usunięcie produktu
  const deleteItem = async (id: number) => {
    try {
      setError(null);
      await apiService.deleteItem(id);
      
      // Usuń lokalnie
      setItems(prev => prev.filter(item => item.idItem !== id));
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unknown error';
      setError(message);
      throw err;
    }
  };

  // Załaduj produkty przy montowaniu
  useEffect(() => {
    refreshItems();
  }, []);

  return (
    <ItemsContext.Provider
      value={{
        items,
        loading,
        error,
        refreshItems,
        createItem,
        updateItem,
        deleteItem,
      }}
    >
      {children}
    </ItemsContext.Provider>
  );
}

export function useItems() {
  const context = useContext(ItemsContext);
  if (!context) {
    throw new Error('useItems must be used within ItemsProvider');
  }
  return context;
}
```

---

## CZĘŚĆ 5: App.tsx - Inicjalizacja

### 5.1. Struktura App.tsx (NOWA - 2025)

**App.tsx:**

```tsx
import React from 'react';
import { View, StatusBar, useColorScheme, StyleSheet } from 'react-native';
import { SafeAreaProvider, useSafeAreaInsets } from 'react-native-safe-area-context';
import { ItemsProvider } from './src/context/ItemsContext';
import RootNavigator from './src/navigation/RootNavigator';

function App(): React.JSX.Element {
  const isDarkMode = useColorScheme() === 'dark';

  return (
    <SafeAreaProvider>
      <StatusBar barStyle={isDarkMode ? 'light-content' : 'dark-content'} />
      <ItemsProvider>
        <AppContent />
      </ItemsProvider>
    </SafeAreaProvider>
  );
}

function AppContent(): React.JSX.Element {
  const insets = useSafeAreaInsets();

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      <RootNavigator />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
});

export default App;
```

**Wyjaśnienie:**
- `SafeAreaProvider` - obsługa notcha/dynamic island
- `ItemsProvider` - opakowuje całą aplikację (context dostępny wszędzie)
- `NavigationContainer` - znajduje się w RootNavigator.tsx
- `AppContent` - oddzielony komponent do obsługi safe areas
- **WAŻNE:** ItemsProvider MUSI opakowywać komponenty używające useItems()

---

## CZĘŚĆ 6: Ekran Listy Produktów

### 6.1. src/screens/ItemsScreen.tsx

```typescript
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { useItems } from '../context/ItemsContext';
import type { Item } from '../types/models';

import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Items'>;

function ItemsScreen({ navigation }: Props) {
  const { items, loading, error, deleteItem } = useItems();

  const handleDelete = (item: Item) => {
    Alert.alert(
      'Potwierdzenie',
      `Czy na pewno usunąć "${item.name}"?`,
      [
        { text: 'Anuluj', style: 'cancel' },
        {
          text: 'Usuń',
          style: 'destructive',
          onPress: async () => {
            try {
              await deleteItem(item.idItem);
              Alert.alert('Sukces', 'Produkt usunięty');
            } catch (err) {
              Alert.alert('Błąd', (err as Error).message);
            }
          },
        },
      ]
    );
  };

  const handleEdit = (item: Item) => {
    navigation.navigate('EditItem', { item });
  };

  const renderItem = ({ item }: { item: Item }) => (
    <View style={styles.itemCard}>
      <View style={styles.itemContent}>
        <Text style={styles.itemName}>{item.name || 'N/A'}</Text>
        <Text style={styles.itemPrice}>
          Cena: {item.price?.toFixed(2) || '0.00'} zł
        </Text>
        <Text style={styles.itemCategory}>
          Kategoria: {item.categoryName || 'Brak'}
        </Text>
        <Text style={styles.itemUnit}>
          Ilość: {item.quantity || 0} {item.unitName || 'szt'}
        </Text>
      </View>

      <View style={styles.itemActions}>
        <TouchableOpacity
          style={styles.editButton}
          onPress={() => handleEdit(item)}
        >
          <Text style={styles.buttonText}>✏️</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.deleteButton}
          onPress={() => handleDelete(item)}
        >
          <Text style={styles.buttonText}>🗑️</Text>
        </TouchableOpacity>
      </View>
    </View>
  );

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Ładowanie produktów...</Text>
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.centerContainer}>
        <Text style={styles.errorText}>❌ Błąd: {error}</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Produkty ({items.length})</Text>
        <TouchableOpacity
          style={styles.addButton}
          onPress={() => navigation.navigate('CreateItem')}
        >
          <Text style={styles.addButtonText}>+ Dodaj</Text>
        </TouchableOpacity>
      </View>

      <FlatList
        data={items}
        renderItem={renderItem}
        keyExtractor={(item) => item.idItem.toString()}
        contentContainerStyle={styles.listContent}
        ListEmptyComponent={
          <Text style={styles.emptyText}>Brak produktów</Text>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  centerContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loadingText: {
    marginTop: 10,
    fontSize: 16,
    color: '#666',
  },
  errorText: {
    fontSize: 16,
    color: 'red',
    textAlign: 'center',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 16,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#ddd',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
  },
  addButton: {
    backgroundColor: '#007AFF',
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
  },
  addButtonText: {
    color: '#fff',
    fontWeight: '600',
  },
  listContent: {
    padding: 16,
  },
  itemCard: {
    flexDirection: 'row',
    backgroundColor: '#fff',
    padding: 12,
    marginBottom: 12,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  itemContent: {
    flex: 1,
  },
  itemName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
  },
  itemPrice: {
    fontSize: 14,
    color: '#007AFF',
    marginTop: 4,
    fontWeight: '500',
  },
  itemCategory: {
    fontSize: 12,
    color: '#666',
    marginTop: 2,
  },
  itemUnit: {
    fontSize: 12,
    color: '#666',
    marginTop: 2,
  },
  itemActions: {
    flexDirection: 'row',
    columnGap: 8,  // gap wspierany od RN 0.71+
  },
  editButton: {
    backgroundColor: '#4CAF50',
    padding: 10,
    borderRadius: 6,
    justifyContent: 'center',
  },
  deleteButton: {
    backgroundColor: '#F44336',
    padding: 10,
    borderRadius: 6,
    justifyContent: 'center',
  },
  buttonText: {
    fontSize: 18,
  },
  emptyText: {
    textAlign: 'center',
    fontSize: 16,
    color: '#999',
    marginTop: 40,
  },
});

export default ItemsScreen;
```

---

## CZĘŚĆ 7: Formularz Tworzenia Produktu

### 7.1. src/screens/CreateItemScreen.tsx

```typescript
import {
  View,
  TextInput,
  Button,
  StyleSheet,
  Alert,
  ScrollView,
  Text,
  ActivityIndicator,
} from 'react-native';
import { useState, useEffect } from 'react';
import { useItems } from '../context/ItemsContext';
import apiService from '../api/apiService';
import type { Category, UnitOfMeasurement } from '../types/models';

import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'CreateItem'>;

function CreateItemScreen({ navigation }: Props) {
  const { createItem } = useItems();

  // Form state
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [quantity, setQuantity] = useState('');
  const [code, setCode] = useState('');
  const [idCategory, setIdCategory] = useState('');
  const [idUnitOfMeasurement, setIdUnitOfMeasurement] = useState('');

  // Dropdowns state
  const [categories, setCategories] = useState<Category[]>([]);
  const [units, setUnits] = useState<UnitOfMeasurement[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // Pobranie kategorii i jednostek
  useEffect(() => {
    const loadData = async () => {
      try {
        const [cats, unts] = await Promise.all([
          apiService.getCategories(),
          apiService.getUnitOfMeasurements(),
        ]);
        setCategories(cats);
        setUnits(unts);
      } catch (error) {
        Alert.alert('Błąd', 'Nie udało się załadować danych');
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, []);

  const handleSubmit = async () => {
    // Walidacja
    if (!name.trim()) {
      Alert.alert('Błąd', 'Wpisz nazwę produktu');
      return;
    }
    if (!idCategory) {
      Alert.alert('Błąd', 'Wybierz kategorię');
      return;
    }

    try {
      setSubmitting(true);
      await createItem({
        name: name.trim(),
        description: description.trim() || undefined,
        price: price ? parseFloat(price) : undefined,
        quantity: quantity ? parseFloat(quantity) : undefined,
        idCategory: parseInt(idCategory),
        idUnitOfMeasurement: idUnitOfMeasurement
          ? parseInt(idUnitOfMeasurement)
          : undefined,
        code: code.trim() || undefined,
      });

      Alert.alert('Sukces', 'Produkt został utworzony', [
        { text: 'OK', onPress: () => navigation.goBack() },
      ]);
    } catch (error) {
      Alert.alert('Błąd', (error as Error).message);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <View style={styles.form}>
        <Text style={styles.label}>Nazwa produktu *</Text>
        <TextInput
          style={styles.input}
          placeholder="Wpisz nazwę"
          value={name}
          onChangeText={setName}
          editable={!submitting}
        />

        <Text style={styles.label}>Opis</Text>
        <TextInput
          style={[styles.input, styles.multiline]}
          placeholder="Wpisz opis"
          value={description}
          onChangeText={setDescription}
          multiline
          numberOfLines={3}
          editable={!submitting}
        />

        <Text style={styles.label}>Cena</Text>
        <TextInput
          style={styles.input}
          placeholder="Wpisz cenę"
          value={price}
          onChangeText={setPrice}
          keyboardType="decimal-pad"
          editable={!submitting}
        />

        <Text style={styles.label}>Ilość</Text>
        <TextInput
          style={styles.input}
          placeholder="Wpisz ilość"
          value={quantity}
          onChangeText={setQuantity}
          keyboardType="decimal-pad"
          editable={!submitting}
        />

        <Text style={styles.label}>Kod produktu</Text>
        <TextInput
          style={styles.input}
          placeholder="Wpisz kod"
          value={code}
          onChangeText={setCode}
          editable={!submitting}
        />

        <Text style={styles.label}>Kategoria *</Text>
        <View style={styles.pickerContainer}>
          {categories.map(cat => (
            <Button
              key={cat.idCategory}
              title={cat.name || 'Brak'}
              onPress={() => setIdCategory(cat.idCategory.toString())}
              color={
                idCategory === cat.idCategory.toString()
                  ? '#007AFF'
                  : '#999'
              }
            />
          ))}
        </View>

        <Text style={styles.label}>Jednostka miary</Text>
        <View style={styles.pickerContainer}>
          <Button
            title="Brak"
            onPress={() => setIdUnitOfMeasurement('')}
            color={idUnitOfMeasurement === '' ? '#007AFF' : '#999'}
          />
          {units.map(unit => (
            <Button
              key={unit.idUnitOfMeasurement}
              title={unit.name || 'Brak'}
              onPress={() =>
                setIdUnitOfMeasurement(unit.idUnitOfMeasurement.toString())
              }
              color={
                idUnitOfMeasurement === unit.idUnitOfMeasurement.toString()
                  ? '#007AFF'
                  : '#999'
              }
            />
          ))}
        </View>

        <View style={styles.buttons}>
          <Button
            title="Anuluj"
            onPress={() => navigation.goBack()}
            color="#999"
            disabled={submitting}
          />
          <Button
            title={submitting ? 'Wysyłanie...' : 'Utwórz'}
            onPress={handleSubmit}
            disabled={submitting}
          />
        </View>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  centerContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  form: {
    padding: 16,
  },
  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
    marginBottom: 8,
    marginTop: 16,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  multiline: {
    height: 80,
    textAlignVertical: 'top',
  },
  pickerContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    columnGap: 8,  // gap wspierany od RN 0.71+
    marginBottom: 8,
  },
  buttons: {
    flexDirection: 'row',
    columnGap: 10,  // gap wspierany od RN 0.71+
    marginTop: 20,
    marginBottom: 30,
  },
});

export default CreateItemScreen;
```

---

## CZĘŚĆ 8: Nawigacja z Items

### 8.1. src/navigation/RootNavigator.tsx

```tsx
import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import ItemsScreen from '../screens/ItemsScreen';
import CreateItemScreen from '../screens/CreateItemScreen';
import EditItemScreen from '../screens/EditItemScreen';

const Stack = createNativeStackNavigator();

function RootNavigator(): React.JSX.Element {
  return (
    <NavigationContainer>
      <Stack.Navigator
        screenOptions={{
          headerStyle: { backgroundColor: '#007AFF' },
          headerTintColor: '#fff',
          headerTitleStyle: { fontWeight: 'bold' },
        }}
      >
        <Stack.Screen
          name="Items"
          component={ItemsScreen}
          options={{ title: 'Produkty' }}
        />
        <Stack.Screen
          name="CreateItem"
          component={CreateItemScreen}
          options={{ title: 'Nowy Produkt' }}
        />
        <Stack.Screen
          name="EditItem"
          component={EditItemScreen}
          options={{ title: 'Edytuj Produkt' }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

export default RootNavigator;
```

---

## CZĘŚĆ 9: Pełny Workflow - Checklista

### Setup Flow:

1. ✅ **Backend ASP.NET:**
   ```bash
   dotnet new webapi -n SolutionOrdersReact.Server
   dotnet add package MediatR
   dotnet add package EntityFrameworkCore.SqlServer
   # Utwórz API endpoints dla /Items, /Category, /UnitOfMeasurement
   ```

2. ✅ **React Native projekt:**
   ```bash
   # Utwórz projekt
   npx @react-native-community/cli init SolutionOrdersMobile
   cd SolutionOrdersMobile
   
   # Zainstaluj zależności
   pnpm install
   
   # React Navigation
   pnpm add @react-navigation/native
   pnpm add @react-navigation/native-stack
   pnpm add react-native-screens react-native-safe-area-context
   
   # iOS (tylko Mac)
   cd ios && pod install && cd ..
   
   # Rebuild aplikacji
   pnpm react-native run-android  # lub run-ios
   ```
   
   **⚠️ WAŻNE:** Po instalacji natywnych zależności zawsze trzeba przebudować aplikację!

3. ✅ **Twoja struktura folderów:**
   ```
   src/
   ├── api/
   │   ├── config.ts
   │   └── apiService.ts
   ├── types/
   │   └── models.ts
   ├── context/
   │   └── ItemsContext.tsx
   ├── screens/
   │   ├── ItemsScreen.tsx
   │   ├── CreateItemScreen.tsx
   │   └── EditItemScreen.tsx
   ├── navigation/
   │   └── RootNavigator.tsx
   └── App.tsx
   ```

4. ✅ **App.tsx:**
   ```
   SafeAreaProvider
   └─ StatusBar
   └─ ItemsProvider ← Context z Items
      └─ NavigationContainer
         └─ RootNavigator
            ├─ ItemsScreen
            ├─ CreateItemScreen
            └─ EditItemScreen
   ```

5. ✅ **Testowanie:**
   ```bash
   pnpm react-native run-android
   # Powinieneś zobaczyć listę produktów z API!
   ```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Kategorie CRUD
Powtórz ten sam pattern dla Categories (context, screens, navigation).

### Zadanie 2: Edit Screen
Utwórz `EditItemScreen.tsx` - pobierz item z `route.params`, pozwól edit, wyślij PUT.

### Zadanie 3: Wyszukiwanie
Dodaj search bar do `ItemsScreen.tsx` - filtruj Items po nazwie.

### Zadanie 4: Pull to Refresh
Dodaj `RefreshControl` do FlatList w ItemsScreen.

### Zadanie 5: Error Handling
Dodaj retry logic jeśli API jest offline.

---

## 🔍 Debugging Tips

**Jeśli API nie odpowiada:**
```bash
# Sprawdź IP:
adb shell getprop ro.kernel.qemu.host.ip  # Android

# Test bezpośrednio:
curl http://10.0.2.2:5000/api/Items

# Logi React Native:
pnpm react-native log-android
```

**Jeśli Context nie działa:**
```typescript
// Zawsze test czy provider wraps ekran
console.log('Items Context:', useItems());
```

---

## ➡️ Następna Lekcja

**[Lekcja 6: Relacje 1:M – Produkty, Kategorie, Jednostki](./lekcja-06-relacje-1m.md)**

---

**Gratulacje! 🎉 Masz pełny workflow Items z API!**
