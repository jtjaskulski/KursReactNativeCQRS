# Lekcja 6: Relacje 1:M w CQRS — Produkty, Kategorie, Jednostki (2 godziny)

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

**Moduł:** Relacje 1:M, foreign keys w praktyce  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Modelować relacje jeden-do-wielu (1:M) w EF Core
- ✅ Obsłużyć selectory relacyjne w React Native (Chip UI oraz Picker)
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

## CZĘŚĆ 2: Selectory relacyjne w React Native (30 minut)

W React Native mamy dwa popularne podejścia do wyboru wartości z listy:

| Podejście | Zalety | Wady |
|-----------|--------|------|
| **Chip UI** | Wizualne, touch-friendly, widoczne wszystkie opcje | Zajmuje więcej miejsca |
| **Native Picker** | Kompaktowy, natywny wygląd | Wymaga dodatkowej biblioteki |

### 2.1. Podejście A: Chip UI (TouchableOpacity) — ZALECANE

To podejście używamy w naszym projekcie. Nie wymaga dodatkowych bibliotek natywnych.

**Implementacja w formularzu:**
```tsx
import { TouchableOpacity, Text, View, StyleSheet } from 'react-native';

// W komponencie formularza:
const [idCategory, setIdCategory] = useState('');
const [categories, setCategories] = useState<Category[]>([]);

// JSX:
<Text style={styles.label}>Kategoria *</Text>
<View style={styles.pickerContainer}>
  {categories.map(cat => {
    const isSelected = idCategory === cat.idCategory.toString();
    return (
      <TouchableOpacity
        key={cat.idCategory}
        style={[
          styles.chip,
          isSelected && styles.chipSelected,
        ]}
        onPress={() => setIdCategory(cat.idCategory.toString())}
        activeOpacity={0.7}
      >
        <Text style={[
          styles.chipText,
          isSelected && styles.chipTextSelected,
        ]}>
          {cat.name || 'Brak'}
        </Text>
      </TouchableOpacity>
    );
  })}
</View>
```

**Style dla Chip UI:**
```tsx
const styles = StyleSheet.create({
  pickerContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 8,
    marginBottom: 8,
  },
  chip: {
    paddingHorizontal: 16,
    paddingVertical: 10,
    borderRadius: 8,
    backgroundColor: '#e0e0e0',
    borderWidth: 2,
    borderColor: '#e0e0e0',
  },
  chipSelected: {
    backgroundColor: '#007AFF',
    borderColor: '#007AFF',
  },
  chipText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  chipTextSelected: {
    color: '#fff',
  },
});
```

**Obsługa opcjonalnych pól (np. jednostka miary):**
```tsx
<Text style={styles.label}>Jednostka miary</Text>
<View style={styles.pickerContainer}>
  {/* Opcja "Brak" dla pól opcjonalnych */}
  <TouchableOpacity
    style={[
      styles.chip,
      idUnitOfMeasurement === '' && styles.chipSelected,
    ]}
    onPress={() => setIdUnitOfMeasurement('')}
    activeOpacity={0.7}
  >
    <Text style={[
      styles.chipText,
      idUnitOfMeasurement === '' && styles.chipTextSelected,
    ]}>
      Brak
    </Text>
  </TouchableOpacity>
  {units.map(unit => {
    const isSelected = idUnitOfMeasurement === unit.idUnitOfMeasurement.toString();
    return (
      <TouchableOpacity
        key={unit.idUnitOfMeasurement}
        style={[
          styles.chip,
          isSelected && styles.chipSelected,
        ]}
        onPress={() => setIdUnitOfMeasurement(unit.idUnitOfMeasurement.toString())}
        activeOpacity={0.7}
      >
        <Text style={[
          styles.chipText,
          isSelected && styles.chipTextSelected,
        ]}>
          {unit.name || 'Brak'}
        </Text>
      </TouchableOpacity>
    );
  })}
</View>
```

### 2.2. Podejście B: PickerField (natywny Picker)

Alternatywnie możemy użyć natywnego Picker z biblioteki `@react-native-picker/picker`.

**Instalacja:**
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

**Komponent PickerField (src/components/PickerField.tsx):**
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

**Użycie PickerField w formularzu:**
```tsx
import { PickerField } from '../components/PickerField';

// W formularzu:
<PickerField
  label="Kategoria"
  value={formData.idCategory}
  items={categories || []}
  getValue={cat => cat.idCategory}
  getLabel={cat => cat.name || ''}
  onChange={val => setFormData(fd => ({ ...fd, idCategory: val }))}
  required
/>

<PickerField
  label="Jednostka"
  value={formData.idUnitOfMeasurement}
  items={units || []}
  getValue={u => u.idUnitOfMeasurement}
  getLabel={u => u.name || ''}
  onChange={val => setFormData(fd => ({ ...fd, idUnitOfMeasurement: val }))}
/>
```

---

## CZĘŚĆ 3: Rozszerzony CRUD dla Item (30 minut)

### 3.1. Queries (połączone dane)

**Backend — ItemDto:**
```csharp
public string? CategoryName { get; set; }
public string? UnitName { get; set; }
// ...w handlerze .Include().Select()
```

### 3.2. Ekran Listy produktów

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

---

## CZĘŚĆ 4: Formularz produktu — pełna implementacja (40 minut)

### 4.1. Pobieranie danych słownikowych

```tsx
const [categories, setCategories] = useState<Category[]>([]);
const [units, setUnits] = useState<UnitOfMeasurement[]>([]);
const [loading, setLoading] = useState(true);

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

### 4.2. Walidacja formularza

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
```

### 4.3. Pola formularza

| Pole | Typ | Wymagane | Uwagi |
|------|-----|----------|-------|
| name | TextInput | ✅ | Nazwa produktu |
| description | TextInput (multiline) | ❌ | Opis |
| price | TextInput (decimal-pad) | ❌ | Cena |
| quantity | TextInput (decimal-pad) | ❌ | Ilość |
| code | TextInput | ❌ | Kod produktu |
| idCategory | Chip/Picker | ✅ | Kategoria |
| idUnitOfMeasurement | Chip/Picker | ❌ | Jednostka miary |

---

## 📝 Zadania Praktyczne

### Zadanie 1: CRUD dla klientów (Client) z selectorem relacyjnym
Zaimplementuj ekrany tworzenia i edycji klienta z wyborem kategorii klienta (jeśli istnieje).

### Zadanie 2: Refaktoryzacja do ChipSelector
Wyodrębnij logikę Chip UI do reużywalnego komponentu `ChipSelector`:
```tsx
interface ChipSelectorProps<T> {
  items: T[];
  selectedValue: string;
  onSelect: (value: string) => void;
  getValue: (item: T) => string;
  getLabel: (item: T) => string;
  allowEmpty?: boolean;
  emptyLabel?: string;
  disabled?: boolean;
}
```

### Zadanie 3: Obsługa „Brak kategorii" / „Brak jednostki" jako null
Upewnij się, że opcjonalne pola (jak `idUnitOfMeasurement`) poprawnie obsługują wartość pustą i wysyłają `null` do API.

---

## ➡️ Następna Lekcja

**[Lekcja 7: Zamówienia — Relacje M:M (Order, OrderItem)](./lekcja-07-zamowienia.md)**

---

**Gratulacje! Teraz umiesz obsługiwać relacje 1:M w React Native z CQRS!**
