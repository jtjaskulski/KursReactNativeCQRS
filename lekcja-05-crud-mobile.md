# Lekcja 5: React Native - Integracja z API CQRS (3 godziny)

**Moduł:** React Native + .NET Integration  
**Czas trwania:** 3 godziny  
**Poziom:** Średnio-zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Stworzyć API Service z TypeScript
- ✅ Obsłużyć różne IP dla Android/iOS
- ✅ Zaimplementować CRUD w React Native
- ✅ Stworzyć reużywalny komponent SimpleCrudList
- ✅ Wyświetlać i edytować dane z API

---

## CZĘŚĆ 1: Konfiguracja IP i Setup (20 minut)

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

// Automatycznie wybiera prawidłowy host
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

### 1.4. adb reverse (Alternatywa dla Android)

```bash
# Przekieruj port 5000 z urządzenia na komputer
adb reverse tcp:5000 tcp:5000

# Teraz możesz używać localhost w Android!
const API_URL = 'http://localhost:5000/api';
```

**⚠️ Musisz to robić po każdym restarcie emulatora!**

---

## CZĘŚĆ 2: TypeScript Models (15 minut)

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

## CZĘŚĆ 3: API Service z TypeScript (30 minut)

### 3.1. src/api/apiService.ts

```typescript
import { API_BASE_URL } from './config';
import type {
  UnitOfMeasurement,
  Category,
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

    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...options.headers,
    };

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

  // ========== PRODUKTY ==========

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
}

// Singleton
export default new ApiService();
```

---

## CZĘŚĆ 4: Custom Hook - useFetch (20 minut)

### 4.1. src/hooks/useFetch.ts

```typescript
import { useState, useEffect, useCallback } from 'react';

interface UseFetchResult<T> {
  data: T | null;
  loading: boolean;
  error: Error | null;
  refetch: () => void;
}

export function useFetch<T>(
  fetchFn: () => Promise<T>
): UseFetchResult<T> {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<Error | null>(null);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await fetchFn();
      setData(result);
    } catch (err) {
      setError(err as Error);
    } finally {
      setLoading(false);
    }
  }, [fetchFn]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  return { data, loading, error, refetch: loadData };
}
```

---

## CZĘŚĆ 5: Ekran Listy - UnitOfMeasurementScreen (40 minut)

### 5.1. src/screens/UnitOfMeasurementScreen.tsx

```tsx
import React, { useState } from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
  TextInput,
  Button,
} from 'react-native';
import { useFetch } from '../hooks/useFetch';
import apiService from '../api/apiService';
import type { UnitOfMeasurement } from '../types/models';

const UnitOfMeasurementScreen: React.FC = () => {
  const { data: units, loading, error, refetch } = useFetch(() =>
    apiService.getUnitOfMeasurements()
  );

  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<UnitOfMeasurement | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
  });

  // ===== HANDLERS =====

  const handleCreate = async () => {
    try {
      await apiService.createUnitOfMeasurement({
        name: formData.name,
        description: formData.description,
        isActive: true,
      });
      Alert.alert('Sukces', 'Jednostka została utworzona');
      setFormData({ name: '', description: '' });
      setShowForm(false);
      refetch();
    } catch (err) {
      Alert.alert('Błąd', (err as Error).message);
    }
  };

  const handleUpdate = async () => {
    if (!editingItem) return;

    try {
      await apiService.updateUnitOfMeasurement(editingItem.idUnitOfMeasurement, {
        name: formData.name,
        description: formData.description,
        isActive: editingItem.isActive,
      });
      Alert.alert('Sukces', 'Jednostka została zaktualizowana');
      setFormData({ name: '', description: '' });
      setEditingItem(null);
      setShowForm(false);
      refetch();
    } catch (err) {
      Alert.alert('Błąd', (err as Error).message);
    }
  };

  const handleDelete = (item: UnitOfMeasurement) => {
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
              await apiService.deleteUnitOfMeasurement(item.idUnitOfMeasurement);
              Alert.alert('Sukces', 'Jednostka została usunięta');
              refetch();
            } catch (err) {
              Alert.alert('Błąd', (err as Error).message);
            }
          },
        },
      ]
    );
  };

  const handleEdit = (item: UnitOfMeasurement) => {
    setEditingItem(item);
    setFormData({
      name: item.name || '',
      description: item.description || '',
    });
    setShowForm(true);
  };

  const handleCancel = () => {
    setFormData({ name: '', description: '' });
    setEditingItem(null);
    setShowForm(false);
  };

  // ===== RENDER =====

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Ładowanie...</Text>
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.centerContainer}>
        <Text style={styles.errorText}>Błąd: {error.message}</Text>
        <Button title="Spróbuj ponownie" onPress={refetch} />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      {/* NAGŁÓWEK */}
      <View style={styles.header}>
        <Text style={styles.title}>Jednostki Miary</Text>
        {!showForm && (
          <TouchableOpacity
            style={styles.addButton}
            onPress={() => setShowForm(true)}
          >
            <Text style={styles.addButtonText}>+ Dodaj</Text>
          </TouchableOpacity>
        )}
      </View>

      {/* FORMULARZ */}
      {showForm && (
        <View style={styles.form}>
          <Text style={styles.formTitle}>
            {editingItem ? 'Edytuj jednostkę' : 'Nowa jednostka'}
          </Text>

          <TextInput
            style={styles.input}
            placeholder="Nazwa (np. szt, kg, l)"
            value={formData.name}
            onChangeText={(text) => setFormData({ ...formData, name: text })}
          />

          <TextInput
            style={styles.input}
            placeholder="Opis"
            value={formData.description}
            onChangeText={(text) => setFormData({ ...formData, description: text })}
            multiline
          />

          <View style={styles.formButtons}>
            <Button
              title="Anuluj"
              onPress={handleCancel}
              color="#999"
            />
            <Button
              title={editingItem ? 'Zapisz' : 'Utwórz'}
              onPress={editingItem ? handleUpdate : handleCreate}
            />
          </View>
        </View>
      )}

      {/* LISTA */}
      <FlatList
        data={units}
        keyExtractor={(item) => item.idUnitOfMeasurement.toString()}
        renderItem={({ item }) => (
          <View style={styles.item}>
            <View style={styles.itemContent}>
              <Text style={styles.itemName}>{item.name}</Text>
              <Text style={styles.itemDescription}>{item.description}</Text>
            </View>
            <View style={styles.itemActions}>
              <TouchableOpacity
                style={styles.editButton}
                onPress={() => handleEdit(item)}
              >
                <Text style={styles.buttonText}>✏️ Edytuj</Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={styles.deleteButton}
                onPress={() => handleDelete(item)}
              >
                <Text style={styles.buttonText}>🗑️ Usuń</Text>
              </TouchableOpacity>
            </View>
          </View>
        )}
        ListEmptyComponent={
          <Text style={styles.emptyText}>Brak jednostek miary</Text>
        }
      />
    </View>
  );
};

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
    marginBottom: 20,
    textAlign: 'center',
    paddingHorizontal: 20,
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
  form: {
    backgroundColor: '#fff',
    padding: 16,
    margin: 16,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  formTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 16,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 12,
    borderRadius: 8,
    marginBottom: 12,
    fontSize: 16,
  },
  formButtons: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 8,
  },
  item: {
    backgroundColor: '#fff',
    padding: 16,
    marginHorizontal: 16,
    marginVertical: 8,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  itemContent: {
    marginBottom: 12,
  },
  itemName: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
  },
  itemDescription: {
    fontSize: 14,
    color: '#666',
    marginTop: 4,
  },
  itemActions: {
    flexDirection: 'row',
    columnGap: 8,  // gap wspierany od RN 0.71+
  },
  editButton: {
    backgroundColor: '#4CAF50',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 6,
  },
  deleteButton: {
    backgroundColor: '#F44336',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 6,
  },
  buttonText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '500',
  },
  emptyText: {
    textAlign: 'center',
    fontSize: 16,
    color: '#999',
    marginTop: 40,
  },
});

export default UnitOfMeasurementScreen;
```

---

## CZĘŚĆ 6: Reużywalny Komponent SimpleCrudList (30 minut)

### 6.1. src/components/SimpleCrudList.tsx

```tsx
import React, { useState } from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
  TextInput,
  Button,
} from 'react-native';

interface Field {
  key: string;
  label: string;
  placeholder?: string;
  multiline?: boolean;
}

interface SimpleCrudListProps<T> {
  title: string;
  data: T[] | null;
  loading: boolean;
  error: Error | null;
  fields: Field[];
  getItemKey: (item: T) => string;
  getItemDisplay: (item: T) => { title: string; subtitle?: string };
  onCreate: (data: Record<string, string>) => Promise<void>;
  onUpdate: (item: T, data: Record<string, string>) => Promise<void>;
  onDelete: (item: T) => Promise<void>;
  onRefresh: () => void;
  getFormDataFromItem: (item: T) => Record<string, string>;
}

export function SimpleCrudList<T>({
  title,
  data,
  loading,
  error,
  fields,
  getItemKey,
  getItemDisplay,
  onCreate,
  onUpdate,
  onDelete,
  onRefresh,
  getFormDataFromItem,
}: SimpleCrudListProps<T>): JSX.Element {
  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<T | null>(null);
  const [formData, setFormData] = useState<Record<string, string>>({});

  const resetForm = () => {
    setFormData({});
    setEditingItem(null);
    setShowForm(false);
  };

  const handleCreate = async () => {
    try {
      await onCreate(formData);
      Alert.alert('Sukces', 'Rekord został utworzony');
      resetForm();
      onRefresh();
    } catch (err) {
      Alert.alert('Błąd', (err as Error).message);
    }
  };

  const handleUpdate = async () => {
    if (!editingItem) return;
    try {
      await onUpdate(editingItem, formData);
      Alert.alert('Sukces', 'Rekord został zaktualizowany');
      resetForm();
      onRefresh();
    } catch (err) {
      Alert.alert('Błąd', (err as Error).message);
    }
  };

  const handleDelete = (item: T) => {
    const display = getItemDisplay(item);
    Alert.alert(
      'Potwierdzenie',
      `Czy na pewno usunąć "${display.title}"?`,
      [
        { text: 'Anuluj', style: 'cancel' },
        {
          text: 'Usuń',
          style: 'destructive',
          onPress: async () => {
            try {
              await onDelete(item);
              Alert.alert('Sukces', 'Rekord został usunięty');
              onRefresh();
            } catch (err) {
              Alert.alert('Błąd', (err as Error).message);
            }
          },
        },
      ]
    );
  };

  const handleEdit = (item: T) => {
    setEditingItem(item);
    setFormData(getFormDataFromItem(item));
    setShowForm(true);
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  if (error) {
    return (
      <View style={styles.centerContainer}>
        <Text style={styles.errorText}>Błąd: {error.message}</Text>
        <Button title="Odśwież" onPress={onRefresh} />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>{title}</Text>
        {!showForm && (
          <TouchableOpacity
            style={styles.addButton}
            onPress={() => setShowForm(true)}
          >
            <Text style={styles.addButtonText}>+ Dodaj</Text>
          </TouchableOpacity>
        )}
      </View>

      {showForm && (
        <View style={styles.form}>
          {fields.map((field) => (
            <TextInput
              key={field.key}
              style={[styles.input, field.multiline && styles.multiline]}
              placeholder={field.placeholder || field.label}
              value={formData[field.key] || ''}
              onChangeText={(text) =>
                setFormData({ ...formData, [field.key]: text })
              }
              multiline={field.multiline}
            />
          ))}
          <View style={styles.formButtons}>
            <Button title="Anuluj" onPress={resetForm} color="#999" />
            <Button
              title={editingItem ? 'Zapisz' : 'Utwórz'}
              onPress={editingItem ? handleUpdate : handleCreate}
            />
          </View>
        </View>
      )}

      <FlatList
        data={data}
        keyExtractor={getItemKey}
        renderItem={({ item }) => {
          const display = getItemDisplay(item);
          return (
            <View style={styles.item}>
              <View style={styles.itemContent}>
                <Text style={styles.itemTitle}>{display.title}</Text>
                {display.subtitle && (
                  <Text style={styles.itemSubtitle}>{display.subtitle}</Text>
                )}
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
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  centerContainer: { flex: 1, justifyContent: 'center', alignItems: 'center' },
  errorText: { color: 'red', marginBottom: 20 },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    padding: 16,
    backgroundColor: '#fff',
  },
  title: { fontSize: 24, fontWeight: 'bold' },
  addButton: { backgroundColor: '#007AFF', padding: 10, borderRadius: 8 },
  addButtonText: { color: '#fff', fontWeight: '600' },
  form: { backgroundColor: '#fff', padding: 16, margin: 16, borderRadius: 8 },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 12,
    borderRadius: 8,
    marginBottom: 12,
  },
  multiline: { height: 80 },
  formButtons: { flexDirection: 'row', justifyContent: 'space-between' },
  item: {
    backgroundColor: '#fff',
    padding: 16,
    marginHorizontal: 16,
    marginVertical: 8,
    borderRadius: 8,
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  itemContent: { flex: 1 },
  itemTitle: { fontSize: 16, fontWeight: 'bold' },
  itemSubtitle: { fontSize: 14, color: '#666', marginTop: 4 },
  itemActions: { 
    flexDirection: 'row',
    columnGap: 8,  // gap jest wspierany od RN 0.71+
  },
  editButton: { backgroundColor: '#4CAF50', padding: 8, borderRadius: 6 },
  deleteButton: { backgroundColor: '#F44336', padding: 8, borderRadius: 6 },
  buttonText: { color: '#fff' },
});
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Ekran Categories
Użyj `SimpleCrudList` do stworzenia ekranu dla kategorii.

### Zadanie 2: Pull to Refresh
Dodaj pull-to-refresh do listy.

### Zadanie 3: Search
Dodaj wyszukiwanie po nazwie.

---

## ➡️ Następna Lekcja

**[Lekcja 8: FluentValidation + Pipeline Behaviors](./lekcja-08-walidacja.md)**

---

**Gratulacje! 🎉 Połączyłeś React Native z CQRS API!**
