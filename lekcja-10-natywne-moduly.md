# Lekcja 10: Natywne Moduły, Storage, Permissions w React Native (2 godziny)

**Moduł:** React Native Native Modules & Permissions  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji
Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Wykorzystać AsyncStorage do przechowywania danych lokalnie
- ✅ Użyć PermissionsAndroid i Permissions API
- ✅ Skorzystać z natywnych funkcji (np. kamera, lokalizacja)
- ✅ Zintegrować natywne dependency (android/ios) z TypeScript

---

## CZĘŚĆ 1: AsyncStorage – prosty local storage (25 minut)

### 1.1. Instalacja i podstawy

```bash
pnpm add @react-native-async-storage/async-storage
```

**Przykład zapisu/odczytu:**
```typescript
import AsyncStorage from '@react-native-async-storage/async-storage';

async function saveToken(token: string) {
  await AsyncStorage.setItem('token', token);
}
async function getToken() {
  return await AsyncStorage.getItem('token');
}
```

---

## CZĘŚĆ 2: PermissionsAPI (30 minut)

### 2.1. Android – PermissionsAndroid
```typescript
import { PermissionsAndroid } from 'react-native';

async function requestCameraPermission() {
  const granted = await PermissionsAndroid.request(
    PermissionsAndroid.PERMISSIONS.CAMERA,
    {
      title: "Potrzebny dostęp do kamery",
      message:
        "Aplikacja musi mieć dostęp do kamery, aby zrobić zdjęcia.",
      buttonNeutral: "Zapytaj później",
      buttonNegative: "Anuluj",
      buttonPositive: "OK"
    }
  );
  return granted === PermissionsAndroid.RESULTS.GRANTED;
}
```

### 2.2. iOS (Info.plist!)
- Dodaj klucze do Info.plist: NSCameraUsageDescription, NSLocationWhenInUseUsageDescription

---

## CZĘŚĆ 3: Dostęp do natywnego API (30 minut)

### 3.1. Kamera
```bash
pnpm add react-native-image-picker
```
```typescript
import { launchCamera } from 'react-native-image-picker';

launchCamera(
  { mediaType: 'photo' },
  (response) => {
    if (response.didCancel) return;
    if (response.errorCode) alert(response.errorMessage);
    // Dostęp do danych: response.assets[0].uri
  }
);
```

### 3.2. Lokalizacja
```bash
pnpm add @react-native-community/geolocation
```
```typescript
import Geolocation from '@react-native-community/geolocation';
Geolocation.getCurrentPosition(
  position => console.log(position.coords.longitude, position.coords.latitude),
  error => alert(error.message)
);
```

---

## CZĘŚĆ 4: Pisanie natywnego modułu Android (15 minut, bonus)

**android/app/src/main/java/com/solutionordersmobile/storage/StorageModule.java:**
```java
package com.solutionordersmobile.storage;
import android.content.Context;
import android.content.SharedPreferences;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;

public class StorageModule extends ReactContextBaseJavaModule {
  public StorageModule(ReactApplicationContext context) { super(context); }

  @Override
  public String getName() { return "StorageModule"; }

  @ReactMethod
  public void setValue(String key, String value) {
    SharedPreferences prefs = getReactApplicationContext().getSharedPreferences("MyPrefs", Context.MODE_PRIVATE);
    prefs.edit().putString(key, value).apply();
  }
}
```

**Podpięcie w React Native** – najczęściej nie musisz, ale możesz dobudować binding przez NativeModules.

---

## 📝 Zadania praktyczne
- Zrób przechowywanie tokena loginu w AsyncStorage
- Dodaj permissions dla kamery i test upload
- Spróbuj napisać własny prosty moduł natywny (np. licznik battery)

---

## ➡️ Następna Lekcja
**[Lekcja 11: Deployment, build APK/IPA, CI/CD](./lekcja-11-deployment.md)**

---

**Gratulacje! Umiesz korzystać z natywnych możliwości mobilnych!**
