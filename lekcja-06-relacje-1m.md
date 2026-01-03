# Lekcja 6: Relacje 1:M w CQRS — Produkty, Kategorie, Jednostki (2 godziny)

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

**Moduł:** Relacje 1:M, foreign keys w praktyce  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Modelować relacje jeden-do-wielu (1:M) w EF Core
- ✅ Obsłużyć pickery relacyjne w React Native
- ✅ Rozwijać zapytania CQRS do mapowania relacji
- ✅ Budować formularze i listy produktów z kategorią i jednostką miary

---

## CZĘŚĆ 1: Modele i relacje w bazie danych (20 minut)

### 1.1. Relacja Category → Item

- Każdy produkt (Item) należy do jednej kategorii
- Każda kategoria ma wiele produktów

**EF Core — Item.cs:**
```csharp
public int IdCategory { get; set; }
public virtual Category Category { get; set; } = null!;
```

**Category.cs:**
```csharp
public virtual ICollection<Item> Items { get; set; } = new List<Item>();
```

### 1.2. Relacja UnitOfMeasurement → Item

- Każdy produkt ma opcjonalnie jednostkę miary

**Item.cs:**
```csharp
public int? IdUnitOfMeasurement { get; set; }
public virtual UnitOfMeasurement? UnitOfMeasurement { get; set; }
```

**UnitOfMeasurement.cs:**
```csharp
public virtual ICollection<Item> Items { get; set; } = new List<Item>();
```

---

## CZĘŚĆ 2: PickerField w React Native (30 minut)

### 2.1. Instalacja Picker

```bash
pnpm add @react-native-picker/picker
```

**⚠️ WAŻNE: Po instalacji natywnej biblioteki musisz przebudować aplikację!**

```bash
# Android - pełny rebuild
cd android && ./gradlew clean && cd ..
pnpm react-native run-android

# iOS (tylko Mac)
cd ios && pod install && cd ..
pnpm react-native run-ios
```

> **Wskazówka:** Hot Reload NIE wystarczy dla natywnych modułów. Zawsze wykonaj pełny build po dodaniu biblioteki z natywnym kodem.

### 2.2. Komponent PickerField

Tworzymy reużywalny komponent do wyboru wartości z listy.

**src/components/PickerField.tsx:**
```tsx
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { Picker } from '@react-native-picker/picker';

interface PickerFieldProps<T> {
  label: string;
  value: string | number | null;
  items: T[];
  getValue: (item: T) => string | number;
  getLabel: (item: T) => string;
  onChange: (value: string | number | null) => void;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  error?: string;
}

export function PickerField<T>({
  label,
  value,
  items,
  getValue,
  getLabel,
  onChange,
  placeholder = 'Wybierz...',
  required = false,
  disabled = false,
  error,
}: PickerFieldProps<T>) {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>
        {label}
        {required && <Text style={styles.required}> *</Text>}
      </Text>
      <View style={[
        styles.pickerContainer,
        disabled && styles.disabled,
        error && styles.errorBorder,
      ]}>
        <Picker
          selectedValue={value}
          onValueChange={onChange}
          style={styles.picker}
          enabled={!disabled}
        >
          <Picker.Item label={placeholder} value={null} />
          {items.map((item) => (
            <Picker.Item
              key={getValue(item).toString()}
              label={getLabel(item)}
              value={getValue(item)}
            />
          ))}
        </Picker>
      </View>
      {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
  );
}

const styles = StyleSheet.create({
  container: { marginBottom: 16 },
  label: { fontWeight: '600', marginBottom: 8, color: '#333' },
  required: { color: '#F44336' },
  pickerContainer: {
    backgroundColor: '#fff',
    borderRadius: 4,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  picker: { height: 50 },
  disabled: { backgroundColor: '#f5f5f5', opacity: 0.7 },
  errorBorder: { borderColor: '#F44336' },
  errorText: { color: '#F44336', fontSize: 12, marginTop: 4 },
});
```

### 2.3. Kluczowe cechy PickerField

| Właściwość | Opis |
|------------|------|
| `label` | Etykieta nad pickerem |
| `value` | Obecnie wybrana wartość |
| `items` | Tablica elementów do wyboru |
| `getValue` | Funkcja zwracająca wartość elementu |
| `getLabel` | Funkcja zwracająca wyświetlany tekst |
| `onChange` | Callback wywoływany przy zmianie |
| `placeholder` | Tekst dla opcji "brak wyboru" |
| `required` | Wyświetla gwiazdkę przy etykiecie |
| `disabled` | Wyłącza picker |
| `error` | Tekst błędu walidacji |

---

## CZĘŚĆ 3: Formularz produktu z PickerField (40 minut)

### 3.1. Importy i stan formularza

```tsx
import { PickerField } from '../components/PickerField';
import type { Category, UnitOfMeasurement } from '../types/models';

// Stan formularza - używamy number | null dla ID
const [idCategory, setIdCategory] = useState<number | null>(null);
const [idUnitOfMeasurement, setIdUnitOfMeasurement] = useState<number | null>(null);

// Dane słownikowe
const [categories, setCategories] = useState<Category[]>([]);
const [units, setUnits] = useState<UnitOfMeasurement[]>([]);
```

### 3.2. Pobieranie danych słownikowych

```tsx
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
```

### 3.3. Użycie PickerField w formularzu

```tsx
<PickerField
  label="Kategoria"
  value={idCategory}
  items={categories}
  getValue={cat => cat.idCategory}
  getLabel={cat => cat.name || 'Brak nazwy'}
  onChange={val => setIdCategory(val as number | null)}
  placeholder="Wybierz kategorię..."
  required
  disabled={submitting}
/>

<PickerField
  label="Jednostka miary"
  value={idUnitOfMeasurement}
  items={units}
  getValue={u => u.idUnitOfMeasurement}
  getLabel={u => u.name || 'Brak nazwy'}
  onChange={val => setIdUnitOfMeasurement(val as number | null)}
  placeholder="Brak jednostki"
  disabled={submitting}
/>
```

### 3.4. Walidacja i wysłanie formularza

```tsx
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
      idCategory: idCategory,
      idUnitOfMeasurement: idUnitOfMeasurement ?? undefined,
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
```

### 3.5. Pola formularza — podsumowanie

| Pole | Typ | Wymagane | Komponent |
|------|-----|----------|-----------|
| name | string | ✅ | TextInput |
| description | string | ❌ | TextInput (multiline) |
| price | number | ❌ | TextInput (decimal-pad) |
| quantity | number | ❌ | TextInput (decimal-pad) |
| code | string | ❌ | TextInput |
| idCategory | number | ✅ | PickerField |
| idUnitOfMeasurement | number | ❌ | PickerField |

---

## CZĘŚĆ 4: Rozszerzony CRUD dla Item (20 minut)

### 4.1. Queries (połączone dane)

**Backend — ItemDto:**
```csharp
public string? CategoryName { get; set; }
public string? UnitName { get; set; }
// ...w handlerze .Include().Select()
```

### 4.2. Ekran listy produktów

```tsx
<FlatList
  data={items}
  renderItem={({ item }) => (
    <ItemCard
      name={item.name}
      price={item.price}
      categoryName={item.categoryName}
      unitName={item.unitName}
    />
  )}
  // ...
/>
```

### 4.3. Edycja produktu — inicjalizacja stanu

```tsx
// Form state - inicjalizacja z przekazanego itemu
const [idCategory, setIdCategory] = useState<number | null>(item.idCategory);
const [idUnitOfMeasurement, setIdUnitOfMeasurement] = useState<number | null>(
  item.idUnitOfMeasurement ?? null
);
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: CRUD dla klientów (Client) z PickerField
Zaimplementuj ekrany tworzenia i edycji klienta. Jeśli klient ma kategorię, użyj PickerField do jej wyboru.

### Zadanie 2: Dodaj walidację błędów do PickerField
Rozszerz formularz o obsługę błędów walidacji dla PickerField:
```tsx
const [errors, setErrors] = useState<{category?: string}>({});

// W walidacji:
if (!idCategory) {
  setErrors(e => ({ ...e, category: 'Wybierz kategorię' }));
  return;
}

// W PickerField:
<PickerField
  label="Kategoria"
  // ...
  error={errors.category}
/>
```

### Zadanie 3: Obsługa „Brak jednostki" jako null
Upewnij się, że opcjonalne pola (jak `idUnitOfMeasurement`) poprawnie obsługują wartość pustą i wysyłają `undefined` do API zamiast `null`.

---

## ➡️ Następna Lekcja

**[Lekcja 7: Zamówienia — Relacje M:M (Order, OrderItem)](./lekcja-07-zamowienia.md)**

---

**Gratulacje! Teraz umiesz obsługiwać relacje 1:M w React Native z CQRS!**
