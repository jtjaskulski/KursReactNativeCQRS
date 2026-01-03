# Lekcja 6: Relacje 1:M w CQRS – Produkty, Kategorie, Jednostki (2 godziny)

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

**EF Core – Item.cs:**
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
  // Opcjonalne props (używane w lekcji 07)
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

### 2.3. Implementacja w formularzu produktu

```tsx
<PickerField
  label="Kategoria"
  value={formData.idCategory}
  items={categories || []}
  getValue={cat => cat.idCategory}
  getLabel={cat => cat.name || ''}
  onChange={val => setFormData(fd => ({ ...fd, idCategory: val }))}
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

**Backend – ItemDto:**
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

## CZĘŚĆ 4: Formularz produktu (40 minut)

- Uzupełnienie pól: name, description, idCategory, price, quantity, idUnitOfMeasurement, code
- Walidacja pól wymaganych
- PickerField dla kategorii i jednostki

---

## 📝 Zadania Praktyczne

### Zadanie 1: CRUD dla klientów (Client) z pickerem relacyjnym
### Zadanie 2: Przypisanie produktu do dostawcy (jeśli dodany)
### Zadanie 3: Obsługa „Brak kategorii” / „Brak jednostki” jako null

---

## ➡️ Następna Lekcja

**[Lekcja 7: Zamówienia – Relacje M:M (Order, OrderItem)](./lekcja-07-zamowienia.md)**

---

**Gratulacje! Teraz umiesz obsługiwać relacje 1:M!**
