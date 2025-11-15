# Lekcja 6: Relacje 1:M w CQRS – Produkty, Kategorie, Jednostki (2 godziny)

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
  onChange: (value: string | number) => void;
}

export function PickerField<T>({
  label,
  value,
  items,
  getValue,
  getLabel,
  onChange,
}: PickerFieldProps<T>) {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>{label}</Text>
      <Picker
        selectedValue={value}
        onValueChange={onChange}
        style={styles.picker}
      >
        <Picker.Item label="Wybierz..." value={null} />
        {items.map((item) => (
          <Picker.Item
            key={getValue(item).toString()}
            label={getLabel(item)}
            value={getValue(item)}
          />
        ))}
      </Picker>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { marginBottom: 16 },
  label: { fontWeight: '600', marginBottom: 8 },
  picker: { backgroundColor: '#fff', borderRadius: 4 }
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
