# Lekcja 10: Natywne Moduły – Permissions, Camera, Geolokalizacja

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

**Moduł:** Integracja z funkcjami urządzenia  
**Czas trwania:** 3 godziny  
**Poziom:** Zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Używać AsyncStorage do przechowywania danych lokalnych
- ✅ Zarządzać uprawnieniami na Android i iOS
- ✅ Integrować kamerę do robienia zdjęć
- ✅ Pobierać lokalizację GPS użytkownika
- ✅ Rozumieć różnice między Android a iOS
- ✅ Tworzyć podstawowy natywny moduł (Android)

---

## CZĘŚĆ 1: Teoria Natywnych Modułów (15 minut)

### 1.1. Czym są natywne moduły?

**SCRIPT dla prowadzącego:**

> „React Native daje nam dostęp do JavaScript, ale czasami potrzebujemy funkcji dostępnych tylko w natywnym kodzie - kamera, GPS, Bluetooth, biometria. Natywne moduły to most (bridge) między JavaScript a kodem Java/Kotlin/Swift/Objective-C."

**Architektura:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    ARCHITEKTURA NATYWNYCH MODUŁÓW                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │                      JAVASCRIPT (React Native)                       │   │
│   │                                                                      │   │
│   │   import { launchCamera } from 'react-native-image-picker';         │   │
│   │   import Geolocation from '@react-native-community/geolocation';    │   │
│   │                                                                      │   │
│   └───────────────────────────────┬─────────────────────────────────────┘   │
│                                   │                                         │
│                                   │ BRIDGE (JSON serialization)             │
│                                   │                                         │
│   ┌───────────────────────────────┼─────────────────────────────────────┐   │
│   │                               ▼                                      │   │
│   │   ┌─────────────────┐    ┌─────────────────┐                        │   │
│   │   │    ANDROID      │    │      iOS        │                        │   │
│   │   │  (Java/Kotlin)  │    │ (Swift/Obj-C)   │                        │   │
│   │   ├─────────────────┤    ├─────────────────┤                        │   │
│   │   │ Camera API      │    │ AVFoundation    │                        │   │
│   │   │ Location API    │    │ CoreLocation    │                        │   │
│   │   │ Permissions     │    │ Privacy APIs    │                        │   │
│   │   └─────────────────┘    └─────────────────┘                        │   │
│   │                    NATIVE PLATFORM                                   │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Kiedy potrzebujesz natywnego modułu?

| Funkcja | Czy potrzebujesz natywnego modułu? | Biblioteka |
|---------|-----------------------------------|------------|
| Kamera | TAK | react-native-image-picker |
| GPS | TAK | @react-native-community/geolocation |
| Local Storage | TAK | @react-native-async-storage/async-storage |
| Bluetooth | TAK | react-native-ble-manager |
| Biometria | TAK | react-native-biometrics |
| HTTP | NIE | fetch (wbudowane) |
| UI Components | NIE | React Native core |

---

## CZĘŚĆ 2: AsyncStorage – Lokalne Dane (30 minut)

### 2.1. Instalacja

```bash
# Instalacja
pnpm add @react-native-async-storage/async-storage

# iOS - pod install
cd ios && pod install && cd ..

# WAŻNE: Przebuduj aplikację!
pnpm react-native run-android
```

### 2.2. Podstawowe operacje

**src/utils/storage.ts:**

```typescript
import AsyncStorage from '@react-native-async-storage/async-storage';

/**
 * Klasa pomocnicza do pracy z AsyncStorage
 * AsyncStorage przechowuje dane jako string, więc potrzebujemy JSON.stringify/parse
 */
class StorageService {
  // ========== PODSTAWOWE OPERACJE ==========

  /**
   * Zapisuje wartość pod kluczem
   */
  async set(key: string, value: string): Promise<void> {
    try {
      await AsyncStorage.setItem(key, value);
    } catch (error) {
      console.error(`Błąd zapisu klucza "${key}":`, error);
      throw error;
    }
  }

  /**
   * Pobiera wartość z klucza
   */
  async get(key: string): Promise<string | null> {
    try {
      return await AsyncStorage.getItem(key);
    } catch (error) {
      console.error(`Błąd odczytu klucza "${key}":`, error);
      throw error;
    }
  }

  /**
   * Usuwa wartość
   */
  async remove(key: string): Promise<void> {
    try {
      await AsyncStorage.removeItem(key);
    } catch (error) {
      console.error(`Błąd usuwania klucza "${key}":`, error);
      throw error;
    }
  }

  /**
   * Czyści całe storage (OSTROŻNIE!)
   */
  async clear(): Promise<void> {
    try {
      await AsyncStorage.clear();
    } catch (error) {
      console.error('Błąd czyszczenia storage:', error);
      throw error;
    }
  }

  // ========== OPERACJE NA OBIEKTACH ==========

  /**
   * Zapisuje obiekt jako JSON
   */
  async setObject<T>(key: string, value: T): Promise<void> {
    try {
      const jsonValue = JSON.stringify(value);
      await AsyncStorage.setItem(key, jsonValue);
    } catch (error) {
      console.error(`Błąd zapisu obiektu "${key}":`, error);
      throw error;
    }
  }

  /**
   * Pobiera obiekt z JSON
   */
  async getObject<T>(key: string): Promise<T | null> {
    try {
      const jsonValue = await AsyncStorage.getItem(key);
      if (jsonValue === null) return null;
      return JSON.parse(jsonValue) as T;
    } catch (error) {
      console.error(`Błąd odczytu obiektu "${key}":`, error);
      throw error;
    }
  }

  // ========== OPERACJE MASOWE ==========

  /**
   * Zapisuje wiele par klucz-wartość naraz
   */
  async multiSet(pairs: [string, string][]): Promise<void> {
    try {
      await AsyncStorage.multiSet(pairs);
    } catch (error) {
      console.error('Błąd zapisu wielu kluczy:', error);
      throw error;
    }
  }

  /**
   * Pobiera wiele wartości naraz
   */
  async multiGet(keys: string[]): Promise<readonly [string, string | null][]> {
    try {
      return await AsyncStorage.multiGet(keys);
    } catch (error) {
      console.error('Błąd odczytu wielu kluczy:', error);
      throw error;
    }
  }

  /**
   * Pobiera wszystkie klucze
   */
  async getAllKeys(): Promise<readonly string[]> {
    try {
      return await AsyncStorage.getAllKeys();
    } catch (error) {
      console.error('Błąd pobierania kluczy:', error);
      throw error;
    }
  }
}

export const storage = new StorageService();

// ========== KLUCZE STORAGE ==========
export const STORAGE_KEYS = {
  AUTH_TOKEN: '@auth_token',
  USER_PROFILE: '@user_profile',
  SETTINGS: '@settings',
  CART_ITEMS: '@cart_items',
  RECENT_SEARCHES: '@recent_searches',
  THEME: '@theme',
} as const;
```

### 2.3. Hook useStorage

**src/hooks/useStorage.ts:**

```typescript
import { useState, useEffect, useCallback } from 'react';
import { storage } from '../utils/storage';

interface UseStorageResult<T> {
  value: T | null;
  loading: boolean;
  error: string | null;
  setValue: (newValue: T) => Promise<void>;
  removeValue: () => Promise<void>;
  refresh: () => Promise<void>;
}

/**
 * Hook do pracy z AsyncStorage z automatyczną synchronizacją stanu
 */
export function useStorage<T>(key: string, defaultValue?: T): UseStorageResult<T> {
  const [value, setValue] = useState<T | null>(defaultValue ?? null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadValue = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const stored = await storage.getObject<T>(key);
      setValue(stored ?? defaultValue ?? null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd odczytu');
    } finally {
      setLoading(false);
    }
  }, [key, defaultValue]);

  const saveValue = useCallback(async (newValue: T) => {
    try {
      setError(null);
      await storage.setObject(key, newValue);
      setValue(newValue);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd zapisu');
      throw err;
    }
  }, [key]);

  const removeValue = useCallback(async () => {
    try {
      setError(null);
      await storage.remove(key);
      setValue(defaultValue ?? null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd usuwania');
      throw err;
    }
  }, [key, defaultValue]);

  useEffect(() => {
    loadValue();
  }, [loadValue]);

  return {
    value,
    loading,
    error,
    setValue: saveValue,
    removeValue,
    refresh: loadValue,
  };
}
```

### 2.4. Przykład: Koszyk zakupowy

**src/hooks/useCart.ts:**

```typescript
import { useState, useEffect, useCallback } from 'react';
import { storage, STORAGE_KEYS } from '../utils/storage';
import type { Item } from '../types/models';

interface CartItem {
  item: Item;
  quantity: number;
  addedAt: string;
}

interface UseCartResult {
  items: CartItem[];
  totalItems: number;
  totalAmount: number;
  loading: boolean;
  addItem: (item: Item, quantity?: number) => Promise<void>;
  removeItem: (itemId: number) => Promise<void>;
  updateQuantity: (itemId: number, quantity: number) => Promise<void>;
  clearCart: () => Promise<void>;
}

export function useCart(): UseCartResult {
  const [items, setItems] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(true);

  // Ładowanie koszyka przy starcie
  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    try {
      setLoading(true);
      const stored = await storage.getObject<CartItem[]>(STORAGE_KEYS.CART_ITEMS);
      setItems(stored || []);
    } catch (error) {
      console.error('Błąd ładowania koszyka:', error);
    } finally {
      setLoading(false);
    }
  };

  const saveCart = async (newItems: CartItem[]) => {
    await storage.setObject(STORAGE_KEYS.CART_ITEMS, newItems);
    setItems(newItems);
  };

  const addItem = useCallback(async (item: Item, quantity: number = 1) => {
    const existingIndex = items.findIndex(ci => ci.item.idItem === item.idItem);

    let newItems: CartItem[];

    if (existingIndex >= 0) {
      // Aktualizuj ilość
      newItems = [...items];
      newItems[existingIndex].quantity += quantity;
    } else {
      // Dodaj nowy
      newItems = [
        ...items,
        { item, quantity, addedAt: new Date().toISOString() }
      ];
    }

    await saveCart(newItems);
  }, [items]);

  const removeItem = useCallback(async (itemId: number) => {
    const newItems = items.filter(ci => ci.item.idItem !== itemId);
    await saveCart(newItems);
  }, [items]);

  const updateQuantity = useCallback(async (itemId: number, quantity: number) => {
    if (quantity <= 0) {
      await removeItem(itemId);
      return;
    }

    const newItems = items.map(ci =>
      ci.item.idItem === itemId
        ? { ...ci, quantity }
        : ci
    );
    await saveCart(newItems);
  }, [items, removeItem]);

  const clearCart = useCallback(async () => {
    await saveCart([]);
  }, []);

  // Obliczenia
  const totalItems = items.reduce((sum, ci) => sum + ci.quantity, 0);
  const totalAmount = items.reduce(
    (sum, ci) => sum + (ci.item.price || 0) * ci.quantity,
    0
  );

  return {
    items,
    totalItems,
    totalAmount,
    loading,
    addItem,
    removeItem,
    updateQuantity,
    clearCart,
  };
}
```

---

## CZĘŚĆ 3: Uprawnienia (Permissions) (35 minut)

### 3.1. Różnice Android vs iOS

**SCRIPT dla prowadzącego:**

> „Na Androidzie uprawnienia pytamy w runtime. Na iOS większość uprawnień konfigurujemy w Info.plist. Ale ZAWSZE musimy pytać użytkownika!"

**Tabela uprawnień:**

| Funkcja | Android Manifest | iOS Info.plist |
|---------|------------------|----------------|
| Kamera | CAMERA | NSCameraUsageDescription |
| Galeria | READ_EXTERNAL_STORAGE | NSPhotoLibraryUsageDescription |
| Lokalizacja | ACCESS_FINE_LOCATION | NSLocationWhenInUseUsageDescription |
| Mikrofon | RECORD_AUDIO | NSMicrophoneUsageDescription |
| Kontakty | READ_CONTACTS | NSContactsUsageDescription |

### 3.2. Instalacja biblioteki uprawnień

```bash
# Instalacja
pnpm add react-native-permissions

# iOS - pod install
cd ios && pod install && cd ..
```

### 3.3. Konfiguracja Android

**android/app/src/main/AndroidManifest.xml:**

```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Uprawnienia -->
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.RECORD_AUDIO" />

    <!-- Funkcje opcjonalne -->
    <uses-feature android:name="android.hardware.camera" android:required="false" />
    <uses-feature android:name="android.hardware.camera.front" android:required="false" />

    <application
        android:name=".MainApplication"
        ...
    >
        ...
    </application>
</manifest>
```

### 3.4. Konfiguracja iOS

**ios/SolutionOrdersMobile/Info.plist:**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- ... inne wpisy ... -->

    <!-- Kamera -->
    <key>NSCameraUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu do kamery, aby robić zdjęcia produktów</string>

    <!-- Galeria - odczyt -->
    <key>NSPhotoLibraryUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu do galerii, aby wybierać zdjęcia</string>

    <!-- Galeria - zapis -->
    <key>NSPhotoLibraryAddUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu, aby zapisywać zdjęcia</string>

    <!-- Lokalizacja - podczas użycia -->
    <key>NSLocationWhenInUseUsageDescription</key>
    <string>Aplikacja potrzebuje lokalizacji, aby pokazać najbliższe sklepy</string>

    <!-- Lokalizacja - zawsze (jeśli potrzebne) -->
    <key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
    <string>Aplikacja potrzebuje lokalizacji w tle do śledzenia dostaw</string>

    <!-- Mikrofon -->
    <key>NSMicrophoneUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu do mikrofonu do nagrywania notatek głosowych</string>
</dict>
</plist>
```

### 3.5. Hook usePermissions

**src/hooks/usePermissions.ts:**

```typescript
import { useState, useCallback } from 'react';
import { Platform, Alert, Linking } from 'react-native';
import {
  check,
  request,
  PERMISSIONS,
  RESULTS,
  Permission,
  PermissionStatus,
} from 'react-native-permissions';

type PermissionType = 'camera' | 'photo' | 'location' | 'microphone';

interface UsePermissionsResult {
  checking: boolean;
  status: PermissionStatus | null;
  check: () => Promise<boolean>;
  request: () => Promise<boolean>;
  openSettings: () => void;
}

/**
 * Hook do zarządzania uprawnieniami
 */
export function usePermission(type: PermissionType): UsePermissionsResult {
  const [checking, setChecking] = useState(false);
  const [status, setStatus] = useState<PermissionStatus | null>(null);

  const getPermission = (): Permission => {
    const permissions: Record<PermissionType, { android: Permission; ios: Permission }> = {
      camera: {
        android: PERMISSIONS.ANDROID.CAMERA,
        ios: PERMISSIONS.IOS.CAMERA,
      },
      photo: {
        android: PERMISSIONS.ANDROID.READ_EXTERNAL_STORAGE,
        ios: PERMISSIONS.IOS.PHOTO_LIBRARY,
      },
      location: {
        android: PERMISSIONS.ANDROID.ACCESS_FINE_LOCATION,
        ios: PERMISSIONS.IOS.LOCATION_WHEN_IN_USE,
      },
      microphone: {
        android: PERMISSIONS.ANDROID.RECORD_AUDIO,
        ios: PERMISSIONS.IOS.MICROPHONE,
      },
    };

    return Platform.OS === 'ios'
      ? permissions[type].ios
      : permissions[type].android;
  };

  const checkPermission = useCallback(async (): Promise<boolean> => {
    try {
      setChecking(true);
      const permission = getPermission();
      const result = await check(permission);
      setStatus(result);
      return result === RESULTS.GRANTED;
    } catch (error) {
      console.error('Błąd sprawdzania uprawnienia:', error);
      return false;
    } finally {
      setChecking(false);
    }
  }, [type]);

  const requestPermission = useCallback(async (): Promise<boolean> => {
    try {
      setChecking(true);

      // Najpierw sprawdź
      const permission = getPermission();
      let result = await check(permission);

      // Jeśli jeszcze nie pytaliśmy - poproś
      if (result === RESULTS.DENIED) {
        result = await request(permission);
      }

      setStatus(result);

      // Jeśli zablokowane - pokaż dialog
      if (result === RESULTS.BLOCKED) {
        showBlockedAlert();
        return false;
      }

      return result === RESULTS.GRANTED;
    } catch (error) {
      console.error('Błąd żądania uprawnienia:', error);
      return false;
    } finally {
      setChecking(false);
    }
  }, [type]);

  const showBlockedAlert = () => {
    const messages: Record<PermissionType, string> = {
      camera: 'Dostęp do kamery został zablokowany',
      photo: 'Dostęp do galerii został zablokowany',
      location: 'Dostęp do lokalizacji został zablokowany',
      microphone: 'Dostęp do mikrofonu został zablokowany',
    };

    Alert.alert(
      'Uprawnienie zablokowane',
      `${messages[type]}. Czy chcesz otworzyć ustawienia?`,
      [
        { text: 'Anuluj', style: 'cancel' },
        { text: 'Otwórz ustawienia', onPress: openSettings },
      ]
    );
  };

  const openSettings = () => {
    Linking.openSettings();
  };

  return {
    checking,
    status,
    check: checkPermission,
    request: requestPermission,
    openSettings,
  };
}
```

---

## CZĘŚĆ 4: Kamera (30 minut)

### 4.1. Instalacja react-native-image-picker

```bash
# Instalacja
pnpm add react-native-image-picker

# iOS - pod install
cd ios && pod install && cd ..

# WAŻNE: Przebuduj aplikację!
pnpm react-native run-android
```

### 4.2. Hook useCamera

**src/hooks/useCamera.ts:**

```typescript
import { useState, useCallback } from 'react';
import {
  launchCamera,
  launchImageLibrary,
  ImagePickerResponse,
  CameraOptions,
  ImageLibraryOptions,
  Asset,
} from 'react-native-image-picker';
import { usePermission } from './usePermissions';
import { Alert, Platform } from 'react-native';

interface UseCameraResult {
  image: Asset | null;
  loading: boolean;
  error: string | null;
  takePhoto: () => Promise<Asset | null>;
  pickFromGallery: () => Promise<Asset | null>;
  clearImage: () => void;
}

export function useCamera(): UseCameraResult {
  const [image, setImage] = useState<Asset | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const cameraPermission = usePermission('camera');
  const photoPermission = usePermission('photo');

  const defaultOptions: CameraOptions = {
    mediaType: 'photo',
    quality: 0.8,
    maxWidth: 1200,
    maxHeight: 1200,
    includeBase64: false,
    saveToPhotos: false,
  };

  const handleResponse = (response: ImagePickerResponse): Asset | null => {
    if (response.didCancel) {
      console.log('Użytkownik anulował');
      return null;
    }

    if (response.errorCode) {
      const errorMessages: Record<string, string> = {
        camera_unavailable: 'Kamera jest niedostępna',
        permission: 'Brak uprawnień',
        others: response.errorMessage || 'Nieznany błąd',
      };
      setError(errorMessages[response.errorCode] || response.errorMessage || 'Błąd');
      return null;
    }

    if (response.assets && response.assets.length > 0) {
      return response.assets[0];
    }

    return null;
  };

  const takePhoto = useCallback(async (): Promise<Asset | null> => {
    try {
      setLoading(true);
      setError(null);

      // Sprawdź uprawnienie do kamery
      const hasPermission = await cameraPermission.request();
      if (!hasPermission) {
        setError('Brak dostępu do kamery');
        return null;
      }

      return new Promise((resolve) => {
        launchCamera(defaultOptions, (response) => {
          const asset = handleResponse(response);
          setImage(asset);
          resolve(asset);
        });
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd kamery');
      return null;
    } finally {
      setLoading(false);
    }
  }, [cameraPermission]);

  const pickFromGallery = useCallback(async (): Promise<Asset | null> => {
    try {
      setLoading(true);
      setError(null);

      // Na Androidzie 13+ nie potrzebujemy uprawnienia do READ_EXTERNAL_STORAGE
      // Ale na starszych wersjach tak
      if (Platform.OS === 'android' && Platform.Version < 33) {
        const hasPermission = await photoPermission.request();
        if (!hasPermission) {
          setError('Brak dostępu do galerii');
          return null;
        }
      }

      const options: ImageLibraryOptions = {
        ...defaultOptions,
        selectionLimit: 1,
      };

      return new Promise((resolve) => {
        launchImageLibrary(options, (response) => {
          const asset = handleResponse(response);
          setImage(asset);
          resolve(asset);
        });
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd galerii');
      return null;
    } finally {
      setLoading(false);
    }
  }, [photoPermission]);

  const clearImage = useCallback(() => {
    setImage(null);
    setError(null);
  }, []);

  return {
    image,
    loading,
    error,
    takePhoto,
    pickFromGallery,
    clearImage,
  };
}
```

### 4.3. Komponent ImagePicker

**src/components/ImagePicker.tsx:**

```tsx
import React from 'react';
import {
  View,
  Text,
  Image,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { useCamera } from '../hooks/useCamera';
import type { Asset } from 'react-native-image-picker';

interface ImagePickerProps {
  label?: string;
  value?: Asset | null;
  onChange?: (asset: Asset | null) => void;
  maxSizeMB?: number;
}

export const ImagePicker: React.FC<ImagePickerProps> = ({
  label = 'Zdjęcie',
  value,
  onChange,
  maxSizeMB = 5,
}) => {
  const { image, loading, error, takePhoto, pickFromGallery, clearImage } = useCamera();

  const currentImage = value || image;

  const handleSelect = () => {
    Alert.alert(
      'Wybierz źródło',
      'Skąd chcesz wybrać zdjęcie?',
      [
        {
          text: 'Kamera',
          onPress: async () => {
            const asset = await takePhoto();
            if (asset) {
              validateAndSet(asset);
            }
          },
        },
        {
          text: 'Galeria',
          onPress: async () => {
            const asset = await pickFromGallery();
            if (asset) {
              validateAndSet(asset);
            }
          },
        },
        { text: 'Anuluj', style: 'cancel' },
      ]
    );
  };

  const validateAndSet = (asset: Asset) => {
    // Sprawdź rozmiar
    if (asset.fileSize && asset.fileSize > maxSizeMB * 1024 * 1024) {
      Alert.alert('Błąd', `Zdjęcie jest za duże. Maksymalny rozmiar: ${maxSizeMB} MB`);
      return;
    }

    onChange?.(asset);
  };

  const handleRemove = () => {
    Alert.alert(
      'Usuń zdjęcie',
      'Czy na pewno chcesz usunąć zdjęcie?',
      [
        { text: 'Anuluj', style: 'cancel' },
        {
          text: 'Usuń',
          style: 'destructive',
          onPress: () => {
            clearImage();
            onChange?.(null);
          },
        },
      ]
    );
  };

  return (
    <View style={styles.container}>
      <Text style={styles.label}>{label}</Text>

      {loading ? (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color="#007AFF" />
          <Text style={styles.loadingText}>Przetwarzanie...</Text>
        </View>
      ) : currentImage?.uri ? (
        <View style={styles.imageContainer}>
          <Image
            source={{ uri: currentImage.uri }}
            style={styles.image}
            resizeMode="cover"
          />
          <View style={styles.imageInfo}>
            <Text style={styles.imageInfoText}>
              {currentImage.width} × {currentImage.height}
            </Text>
            {currentImage.fileSize && (
              <Text style={styles.imageInfoText}>
                {(currentImage.fileSize / 1024 / 1024).toFixed(2)} MB
              </Text>
            )}
          </View>
          <View style={styles.imageActions}>
            <TouchableOpacity
              style={styles.changeButton}
              onPress={handleSelect}
            >
              <Text style={styles.changeButtonText}>Zmień</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.removeButton}
              onPress={handleRemove}
            >
              <Text style={styles.removeButtonText}>Usuń</Text>
            </TouchableOpacity>
          </View>
        </View>
      ) : (
        <TouchableOpacity style={styles.placeholder} onPress={handleSelect}>
          <Text style={styles.placeholderIcon}>📷</Text>
          <Text style={styles.placeholderText}>Dodaj zdjęcie</Text>
          <Text style={styles.placeholderHint}>
            Kliknij, aby wybrać z kamery lub galerii
          </Text>
        </TouchableOpacity>
      )}

      {error && <Text style={styles.error}>{error}</Text>}
    </View>
  );
};

const styles = StyleSheet.create({
  container: { marginBottom: 16 },
  label: { fontSize: 14, fontWeight: '600', color: '#333', marginBottom: 8 },
  loadingContainer: {
    height: 200, backgroundColor: '#f0f0f0', borderRadius: 8,
    justifyContent: 'center', alignItems: 'center',
  },
  loadingText: { marginTop: 8, color: '#666' },
  imageContainer: { borderRadius: 8, overflow: 'hidden' },
  image: { width: '100%', height: 200, backgroundColor: '#f0f0f0' },
  imageInfo: {
    flexDirection: 'row', justifyContent: 'space-between',
    padding: 8, backgroundColor: '#f0f0f0',
  },
  imageInfoText: { fontSize: 12, color: '#666' },
  imageActions: {
    flexDirection: 'row', justifyContent: 'space-around',
    padding: 8, backgroundColor: '#f0f0f0',
  },
  changeButton: {
    paddingVertical: 8, paddingHorizontal: 20,
    backgroundColor: '#007AFF', borderRadius: 6,
  },
  changeButtonText: { color: '#fff', fontWeight: '600' },
  removeButton: {
    paddingVertical: 8, paddingHorizontal: 20,
    backgroundColor: '#E53935', borderRadius: 6,
  },
  removeButtonText: { color: '#fff', fontWeight: '600' },
  placeholder: {
    height: 200, backgroundColor: '#f0f0f0', borderRadius: 8,
    borderWidth: 2, borderColor: '#ddd', borderStyle: 'dashed',
    justifyContent: 'center', alignItems: 'center',
  },
  placeholderIcon: { fontSize: 48, marginBottom: 8 },
  placeholderText: { fontSize: 16, fontWeight: '600', color: '#666' },
  placeholderHint: { fontSize: 12, color: '#999', marginTop: 4 },
  error: { color: '#E53935', fontSize: 12, marginTop: 4 },
});
```

---

## CZĘŚĆ 5: Geolokalizacja (30 minut)

### 5.1. Instalacja

```bash
# Instalacja
pnpm add @react-native-community/geolocation

# iOS - pod install
cd ios && pod install && cd ..
```

### 5.2. Hook useLocation

**src/hooks/useLocation.ts:**

```typescript
import { useState, useEffect, useCallback } from 'react';
import Geolocation, {
  GeolocationResponse,
  GeolocationError,
} from '@react-native-community/geolocation';
import { usePermission } from './usePermissions';
import { Platform } from 'react-native';

interface Location {
  latitude: number;
  longitude: number;
  accuracy: number;
  altitude: number | null;
  speed: number | null;
  timestamp: number;
}

interface UseLocationResult {
  location: Location | null;
  loading: boolean;
  error: string | null;
  getCurrentLocation: () => Promise<Location | null>;
  watchLocation: () => void;
  stopWatching: () => void;
  isWatching: boolean;
}

export function useLocation(): UseLocationResult {
  const [location, setLocation] = useState<Location | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [watchId, setWatchId] = useState<number | null>(null);

  const locationPermission = usePermission('location');

  // Konfiguracja geolokalizacji
  useEffect(() => {
    Geolocation.setRNConfiguration({
      skipPermissionRequests: false,
      authorizationLevel: 'whenInUse',
      locationProvider: 'auto',
    });
  }, []);

  // Cleanup przy odmontowaniu
  useEffect(() => {
    return () => {
      if (watchId !== null) {
        Geolocation.clearWatch(watchId);
      }
    };
  }, [watchId]);

  const parsePosition = (position: GeolocationResponse): Location => ({
    latitude: position.coords.latitude,
    longitude: position.coords.longitude,
    accuracy: position.coords.accuracy,
    altitude: position.coords.altitude,
    speed: position.coords.speed,
    timestamp: position.timestamp,
  });

  const handleError = (error: GeolocationError): string => {
    const errorMessages: Record<number, string> = {
      1: 'Brak uprawnień do lokalizacji',
      2: 'Nie można określić lokalizacji',
      3: 'Przekroczono limit czasu',
    };
    return errorMessages[error.code] || error.message || 'Nieznany błąd';
  };

  const getCurrentLocation = useCallback(async (): Promise<Location | null> => {
    try {
      setLoading(true);
      setError(null);

      // Sprawdź uprawnienie
      const hasPermission = await locationPermission.request();
      if (!hasPermission) {
        setError('Brak dostępu do lokalizacji');
        return null;
      }

      return new Promise((resolve) => {
        Geolocation.getCurrentPosition(
          (position) => {
            const loc = parsePosition(position);
            setLocation(loc);
            resolve(loc);
          },
          (error) => {
            const errorMsg = handleError(error);
            setError(errorMsg);
            resolve(null);
          },
          {
            enableHighAccuracy: true,
            timeout: 15000,
            maximumAge: 10000,
          }
        );
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd lokalizacji');
      return null;
    } finally {
      setLoading(false);
    }
  }, [locationPermission]);

  const watchLocation = useCallback(async () => {
    // Sprawdź uprawnienie
    const hasPermission = await locationPermission.request();
    if (!hasPermission) {
      setError('Brak dostępu do lokalizacji');
      return;
    }

    // Zatrzymaj poprzednie śledzenie
    if (watchId !== null) {
      Geolocation.clearWatch(watchId);
    }

    const id = Geolocation.watchPosition(
      (position) => {
        const loc = parsePosition(position);
        setLocation(loc);
        setError(null);
      },
      (error) => {
        const errorMsg = handleError(error);
        setError(errorMsg);
      },
      {
        enableHighAccuracy: true,
        distanceFilter: 10, // Minimum 10m różnicy
        interval: 5000, // Co 5 sekund (Android)
        fastestInterval: 2000, // Nie częściej niż co 2s (Android)
      }
    );

    setWatchId(id);
  }, [locationPermission, watchId]);

  const stopWatching = useCallback(() => {
    if (watchId !== null) {
      Geolocation.clearWatch(watchId);
      setWatchId(null);
    }
  }, [watchId]);

  return {
    location,
    loading,
    error,
    getCurrentLocation,
    watchLocation,
    stopWatching,
    isWatching: watchId !== null,
  };
}
```

### 5.3. Komponent LocationDisplay

**src/components/LocationDisplay.tsx:**

```tsx
import React, { useEffect } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Linking,
  Platform,
} from 'react-native';
import { useLocation } from '../hooks/useLocation';

interface LocationDisplayProps {
  onLocationChange?: (lat: number, lng: number) => void;
  autoStart?: boolean;
}

export const LocationDisplay: React.FC<LocationDisplayProps> = ({
  onLocationChange,
  autoStart = false,
}) => {
  const {
    location,
    loading,
    error,
    getCurrentLocation,
    watchLocation,
    stopWatching,
    isWatching,
  } = useLocation();

  useEffect(() => {
    if (autoStart) {
      getCurrentLocation();
    }
  }, [autoStart]);

  useEffect(() => {
    if (location && onLocationChange) {
      onLocationChange(location.latitude, location.longitude);
    }
  }, [location, onLocationChange]);

  const openInMaps = () => {
    if (!location) return;

    const { latitude, longitude } = location;
    const url = Platform.select({
      ios: `maps:${latitude},${longitude}`,
      android: `geo:${latitude},${longitude}?q=${latitude},${longitude}`,
    });

    if (url) {
      Linking.openURL(url).catch(() => {
        // Fallback do Google Maps web
        Linking.openURL(
          `https://www.google.com/maps?q=${latitude},${longitude}`
        );
      });
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>📍 Lokalizacja</Text>

      {loading && (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="small" color="#007AFF" />
          <Text style={styles.loadingText}>Pobieranie lokalizacji...</Text>
        </View>
      )}

      {error && (
        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>⚠️ {error}</Text>
        </View>
      )}

      {location && (
        <View style={styles.locationInfo}>
          <View style={styles.row}>
            <Text style={styles.label}>Szerokość:</Text>
            <Text style={styles.value}>{location.latitude.toFixed(6)}</Text>
          </View>
          <View style={styles.row}>
            <Text style={styles.label}>Długość:</Text>
            <Text style={styles.value}>{location.longitude.toFixed(6)}</Text>
          </View>
          <View style={styles.row}>
            <Text style={styles.label}>Dokładność:</Text>
            <Text style={styles.value}>±{location.accuracy.toFixed(0)}m</Text>
          </View>
          {location.altitude !== null && (
            <View style={styles.row}>
              <Text style={styles.label}>Wysokość:</Text>
              <Text style={styles.value}>{location.altitude.toFixed(0)}m n.p.m.</Text>
            </View>
          )}
          <View style={styles.row}>
            <Text style={styles.label}>Czas:</Text>
            <Text style={styles.value}>
              {new Date(location.timestamp).toLocaleTimeString('pl-PL')}
            </Text>
          </View>

          <TouchableOpacity style={styles.mapButton} onPress={openInMaps}>
            <Text style={styles.mapButtonText}>🗺️ Otwórz w mapach</Text>
          </TouchableOpacity>
        </View>
      )}

      <View style={styles.actions}>
        <TouchableOpacity
          style={styles.button}
          onPress={getCurrentLocation}
          disabled={loading}
        >
          <Text style={styles.buttonText}>🔄 Odśwież</Text>
        </TouchableOpacity>

        {isWatching ? (
          <TouchableOpacity
            style={[styles.button, styles.stopButton]}
            onPress={stopWatching}
          >
            <Text style={styles.buttonText}>⏹️ Zatrzymaj śledzenie</Text>
          </TouchableOpacity>
        ) : (
          <TouchableOpacity
            style={[styles.button, styles.watchButton]}
            onPress={watchLocation}
          >
            <Text style={styles.buttonText}>▶️ Śledź lokalizację</Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: { backgroundColor: '#fff', borderRadius: 8, padding: 16, marginBottom: 16 },
  title: { fontSize: 18, fontWeight: 'bold', color: '#333', marginBottom: 12 },
  loadingContainer: { flexDirection: 'row', alignItems: 'center', marginBottom: 12 },
  loadingText: { marginLeft: 8, color: '#666' },
  errorContainer: { backgroundColor: '#FFEBEE', padding: 8, borderRadius: 4, marginBottom: 12 },
  errorText: { color: '#C62828', fontSize: 14 },
  locationInfo: { backgroundColor: '#f5f5f5', padding: 12, borderRadius: 8, marginBottom: 12 },
  row: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 4 },
  label: { color: '#666', fontSize: 14 },
  value: { color: '#333', fontSize: 14, fontWeight: '500' },
  mapButton: {
    marginTop: 12, backgroundColor: '#4CAF50', padding: 10, borderRadius: 6, alignItems: 'center',
  },
  mapButtonText: { color: '#fff', fontWeight: '600' },
  actions: { flexDirection: 'row', justifyContent: 'space-between' },
  button: { flex: 1, padding: 12, backgroundColor: '#007AFF', borderRadius: 6, alignItems: 'center', marginHorizontal: 4 },
  buttonText: { color: '#fff', fontWeight: '600', fontSize: 14 },
  stopButton: { backgroundColor: '#FF9800' },
  watchButton: { backgroundColor: '#4CAF50' },
});
```

---

## CZĘŚĆ 6: Tworzenie Natywnego Modułu (Android) (25 minut)

### 6.1. Struktura natywnego modułu

**SCRIPT dla prowadzącego:**

> „Czasami gotowa biblioteka nie wystarczy i musimy napisać własny natywny moduł. Pokażę prosty przykład dla Androida - moduł do wibracji."

> **⚠️ UWAGA: Kotlin vs Java**
> 
> Nowsze projekty React Native (od 0.73+) używają **Kotlin** jako domyślny język dla Androida. 
> Poniższy przykład pokazany jest w **Java** dla kompatybilności z dokumentacją, ale jeśli Twój 
> projekt używa Kotlin (sprawdź czy masz `MainActivity.kt`), możesz:
> 
> 1. Tworzyć moduły w Kotlin (zalecane) - składnia jest bardzo podobna
> 2. Mieszać Java i Kotlin w tym samym projekcie (działa bez problemu)
> 
> Kotlin jest w pełni kompatybilny z Java i możesz używać obu języków w jednym projekcie.

```
android/app/src/main/java/com/solutionordersmobile/
├── MainApplication.kt            ← Kotlin (nowsze projekty)
├── MainActivity.kt               ← Kotlin (nowsze projekty)
└── vibration/                    ← Nasz moduł
    ├── VibrationModule.java      ← Java (lub .kt dla Kotlin)
    └── VibrationPackage.java     ← Java (lub .kt dla Kotlin)
```

### 6.2. VibrationModule.java

**android/app/src/main/java/com/solutionordersmobile/vibration/VibrationModule.java:**

```java
package com.solutionordersmobile.vibration;

import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.VibratorManager;

import androidx.annotation.NonNull;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableArray;

public class VibrationModule extends ReactContextBaseJavaModule {

    private final Vibrator vibrator;

    public VibrationModule(ReactApplicationContext context) {
        super(context);

        // Pobierz vibrator w zależności od wersji Android
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            VibratorManager vm = (VibratorManager) context.getSystemService(Context.VIBRATOR_MANAGER_SERVICE);
            vibrator = vm.getDefaultVibrator();
        } else {
            vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        }
    }

    @NonNull
    @Override
    public String getName() {
        // Ta nazwa będzie dostępna w JS jako NativeModules.CustomVibration
        return "CustomVibration";
    }

    /**
     * Wibruje przez określony czas (ms)
     */
    @ReactMethod
    public void vibrate(int duration, Promise promise) {
        try {
            if (vibrator == null || !vibrator.hasVibrator()) {
                promise.reject("NO_VIBRATOR", "Urządzenie nie obsługuje wibracji");
                return;
            }

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                vibrator.vibrate(
                    VibrationEffect.createOneShot(duration, VibrationEffect.DEFAULT_AMPLITUDE)
                );
            } else {
                vibrator.vibrate(duration);
            }

            promise.resolve(true);
        } catch (Exception e) {
            promise.reject("VIBRATION_ERROR", e.getMessage());
        }
    }

    /**
     * Wibruje według wzorca [przerwa, wibracja, przerwa, wibracja, ...]
     */
    @ReactMethod
    public void vibratePattern(ReadableArray pattern, int repeat, Promise promise) {
        try {
            if (vibrator == null || !vibrator.hasVibrator()) {
                promise.reject("NO_VIBRATOR", "Urządzenie nie obsługuje wibracji");
                return;
            }

            long[] patternArray = new long[pattern.size()];
            for (int i = 0; i < pattern.size(); i++) {
                patternArray[i] = (long) pattern.getDouble(i);
            }

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                vibrator.vibrate(
                    VibrationEffect.createWaveform(patternArray, repeat)
                );
            } else {
                vibrator.vibrate(patternArray, repeat);
            }

            promise.resolve(true);
        } catch (Exception e) {
            promise.reject("VIBRATION_ERROR", e.getMessage());
        }
    }

    /**
     * Zatrzymuje wibrację
     */
    @ReactMethod
    public void cancel() {
        if (vibrator != null) {
            vibrator.cancel();
        }
    }

    /**
     * Sprawdza czy urządzenie obsługuje wibrację
     */
    @ReactMethod
    public void hasVibrator(Promise promise) {
        boolean hasVibrator = vibrator != null && vibrator.hasVibrator();
        promise.resolve(hasVibrator);
    }
}
```

### 6.3. VibrationPackage.java

**android/app/src/main/java/com/solutionordersmobile/vibration/VibrationPackage.java:**

```java
package com.solutionordersmobile.vibration;

import androidx.annotation.NonNull;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class VibrationPackage implements ReactPackage {

    @NonNull
    @Override
    public List<NativeModule> createNativeModules(@NonNull ReactApplicationContext reactContext) {
        List<NativeModule> modules = new ArrayList<>();
        modules.add(new VibrationModule(reactContext));
        return modules;
    }

    @NonNull
    @Override
    public List<ViewManager> createViewManagers(@NonNull ReactApplicationContext reactContext) {
        return Collections.emptyList();
    }
}
```

### 6.4. Rejestracja w MainApplication

**android/app/src/main/java/com/solutionordersmobile/MainApplication.java:**

```java
package com.solutionordersmobile;

import android.app.Application;
import com.facebook.react.PackageList;
import com.facebook.react.ReactApplication;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.react.defaults.DefaultNewArchitectureEntryPoint;
import com.facebook.react.defaults.DefaultReactNativeHost;
import com.facebook.soloader.SoLoader;

// Import naszego pakietu
import com.solutionordersmobile.vibration.VibrationPackage;

import java.util.List;

public class MainApplication extends Application implements ReactApplication {

    private final ReactNativeHost mReactNativeHost =
        new DefaultReactNativeHost(this) {
            @Override
            protected List<ReactPackage> getPackages() {
                List<ReactPackage> packages = new PackageList(this).getPackages();

                // Dodaj nasz pakiet!
                packages.add(new VibrationPackage());

                return packages;
            }

            // ... reszta kodu ...
        };

    // ... reszta kodu ...
}
```

### 6.5. Użycie w JavaScript

**src/utils/vibration.ts:**

```typescript
import { NativeModules, Platform } from 'react-native';

const { CustomVibration } = NativeModules;

/**
 * Wrapper dla natywnego modułu wibracji
 */
export const Vibration = {
  /**
   * Wibruje przez określony czas
   */
  async vibrate(duration: number = 100): Promise<boolean> {
    if (Platform.OS !== 'android') {
      console.warn('CustomVibration działa tylko na Android');
      return false;
    }

    try {
      return await CustomVibration.vibrate(duration);
    } catch (error) {
      console.error('Błąd wibracji:', error);
      return false;
    }
  },

  /**
   * Wibruje według wzorca
   * @param pattern [przerwa, wibracja, przerwa, wibracja, ...]
   * @param repeat -1 = bez powtórzeń, 0+ = indeks od którego powtarzać
   */
  async vibratePattern(
    pattern: number[],
    repeat: number = -1
  ): Promise<boolean> {
    if (Platform.OS !== 'android') {
      console.warn('CustomVibration działa tylko na Android');
      return false;
    }

    try {
      return await CustomVibration.vibratePattern(pattern, repeat);
    } catch (error) {
      console.error('Błąd wibracji:', error);
      return false;
    }
  },

  /**
   * Zatrzymuje wibrację
   */
  cancel(): void {
    if (Platform.OS === 'android') {
      CustomVibration.cancel();
    }
  },

  /**
   * Sprawdza czy urządzenie obsługuje wibrację
   */
  async hasVibrator(): Promise<boolean> {
    if (Platform.OS !== 'android') {
      return false;
    }

    try {
      return await CustomVibration.hasVibrator();
    } catch {
      return false;
    }
  },

  // Predefiniowane wzorce
  patterns: {
    success: [0, 100],
    error: [0, 100, 100, 100, 100, 100],
    warning: [0, 200, 100, 200],
    notification: [0, 50, 50, 50],
  },
};
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Offline mode z AsyncStorage
Zaimplementuj cache'owanie produktów w AsyncStorage gdy brak internetu.

### Zadanie 2: Zdjęcie produktu
Dodaj możliwość dodawania zdjęcia przy tworzeniu produktu, z uploadem do API.

### Zadanie 3: Zapisywanie lokalizacji zamówienia
Zapisz lokalizację GPS przy tworzeniu zamówienia.

### Zadanie 4: Natywny moduł iOS
Stwórz odpowiednik VibrationModule dla iOS używając Swift.

### Zadanie 5: Skanowanie kodów kreskowych
Zintegruj bibliotekę do skanowania kodów kreskowych produktów.

---

## 🔍 Pytania Kontrolne

1. Czym różni się AsyncStorage od localStorage w przeglądarce?
2. Dlaczego uprawnienia na iOS konfigurujemy w Info.plist?
3. Co to jest bridge w React Native?
4. Kiedy używamy `@ReactMethod` vs `@ReactMethod(isBlockingSynchronousMethod = true)`?
5. Jak działa `launchCamera` vs `launchImageLibrary`?
6. Jakie są różnice w GPS między Android a iOS?

---

## ➡️ Następna Lekcja

**[Lekcja 11: Deployment – Budowanie i Publikacja Aplikacji](./lekcja-11-deployment.md)**

W następnej lekcji:
- Budowanie APK/AAB dla Android
- Budowanie IPA dla iOS
- Docker dla produkcji
- CI/CD z GitHub Actions
- Publikacja w Google Play i App Store

---

**Gratulacje! 🎉 Twoja aplikacja teraz korzysta z natywnych funkcji urządzenia!**
