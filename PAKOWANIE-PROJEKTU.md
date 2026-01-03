# Pakowanie Projektu - Czyszczenie i Przywracanie

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

---

## 🎯 Cel

Ten dokument opisuje jak przygotować projekt do spakowania (np. ZIP) oraz jak go przywrócić do działania po rozpakowaniu.

---

## CZĘŚĆ 1: Co można bezpiecznie usunąć?

### 1.1. React Native (folder `rn/SolutionOrdersMobile`)

| Folder/Plik | Rozmiar | Można usunąć? | Opis |
|-------------|---------|---------------|------|
| `node_modules/` | ~1 GB | ✅ TAK | Zależności npm/pnpm - odtwarzane przez `pnpm install` |
| `android/.gradle/` | ~200 MB | ✅ TAK | Cache Gradle - odtwarzane przy buildzie |
| `android/app/build/` | ~500 MB | ✅ TAK | Skompilowana aplikacja Android |
| `android/build/` | ~50 MB | ✅ TAK | Artefakty buildu Android |
| `android/app/.cxx/` | ~100 MB | ✅ TAK | Cache C++ (native modules) |
| `ios/Pods/` | ~500 MB | ✅ TAK | Zależności iOS (CocoaPods) |
| `ios/build/` | ~200 MB | ✅ TAK | Skompilowana aplikacja iOS |
| `.metro/` | ~50 MB | ✅ TAK | Cache Metro bundlera |
| `package-lock.json` | ~1 MB | ⚠️ ZOSTAW | Lock file - zapewnia spójność wersji |
| `pnpm-lock.yaml` | ~500 KB | ⚠️ ZOSTAW | Lock file dla pnpm |

### 1.2. .NET Backend (folder `dotnet/`)

| Folder/Plik | Rozmiar | Można usunąć? | Opis |
|-------------|---------|---------------|------|
| `bin/` | ~50 MB | ✅ TAK | Skompilowane pliki binarne |
| `obj/` | ~100 MB | ✅ TAK | Pliki pośrednie kompilacji |
| `*.csproj.user` | ~1 KB | ✅ TAK | Lokalne ustawienia użytkownika |
| `Migrations/` | ~50 KB | ❌ NIE | Migracje bazy - NIEZBĘDNE! |
| `appsettings.*.json` | ~2 KB | ❌ NIE | Konfiguracja - NIEZBĘDNE! |

### 1.3. Pliki globalne

| Folder/Plik | Można usunąć? | Opis |
|-------------|---------------|------|
| `.git/` | ⚠️ ZALEŻY | Historia git - usuń tylko dla "czystej" paczki |
| `.vs/` | ✅ TAK | Cache Visual Studio |
| `.idea/` | ✅ TAK | Cache IntelliJ/Rider |
| `.vscode/` | ⚠️ ZOSTAW | Ustawienia VS Code - przydatne |

---

## CZĘŚĆ 2: Skrypty czyszczące

### 2.1. Windows (PowerShell)

**clean-project.ps1:**
```powershell
# Czyszczenie projektu React Native + .NET przed pakowaniem
# Uruchom z głównego folderu projektu

Write-Host "🧹 Czyszczenie projektu..." -ForegroundColor Cyan

# React Native
$rnPath = "rn\SolutionOrdersMobile"

Write-Host "Usuwanie node_modules..." -ForegroundColor Yellow
cmd /c "rd /s /q $rnPath\node_modules" 2>$null

Write-Host "Usuwanie Android cache..." -ForegroundColor Yellow
Remove-Item -Path "$rnPath\android\.gradle" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$rnPath\android\build" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$rnPath\android\app\build" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$rnPath\android\app\.cxx" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Usuwanie iOS cache..." -ForegroundColor Yellow
Remove-Item -Path "$rnPath\ios\Pods" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$rnPath\ios\build" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Usuwanie Metro cache..." -ForegroundColor Yellow
Remove-Item -Path "$rnPath\.metro" -Recurse -Force -ErrorAction SilentlyContinue

# .NET
$dotnetPath = "dotnet\SolutionOrdersReact.Server"

Write-Host "Usuwanie .NET bin/obj..." -ForegroundColor Yellow
Remove-Item -Path "$dotnetPath\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$dotnetPath\obj" -Recurse -Force -ErrorAction SilentlyContinue

# Oblicz rozmiar
$size = (Get-ChildItem -Recurse -File -ErrorAction SilentlyContinue | 
    Measure-Object -Property Length -Sum).Sum / 1MB

Write-Host ""
Write-Host "✅ Gotowe! Rozmiar projektu: $([math]::Round($size, 2)) MB" -ForegroundColor Green
```

### 2.2. macOS/Linux (Bash)

**clean-project.sh:**
```bash
#!/bin/bash
set -e

echo "🧹 Czyszczenie projektu..."

# React Native
RN_PATH="rn/SolutionOrdersMobile"

echo "Usuwanie node_modules..."
rm -rf "$RN_PATH/node_modules"

echo "Usuwanie Android cache..."
rm -rf "$RN_PATH/android/.gradle"
rm -rf "$RN_PATH/android/build"
rm -rf "$RN_PATH/android/app/build"
rm -rf "$RN_PATH/android/app/.cxx"

echo "Usuwanie iOS cache..."
rm -rf "$RN_PATH/ios/Pods"
rm -rf "$RN_PATH/ios/build"

echo "Usuwanie Metro cache..."
rm -rf "$RN_PATH/.metro"

# .NET
DOTNET_PATH="dotnet/SolutionOrdersReact.Server"

echo "Usuwanie .NET bin/obj..."
rm -rf "$DOTNET_PATH/bin"
rm -rf "$DOTNET_PATH/obj"

echo ""
echo "✅ Gotowe!"
du -sh . | awk '{print "Rozmiar projektu: " $1}'
```

---

## CZĘŚĆ 3: Przywracanie projektu po rozpakowaniu

### 3.1. React Native

```bash
# 1. Przejdź do folderu React Native
cd rn/SolutionOrdersMobile

# 2. Zainstaluj zależności
pnpm install
# lub: npm install

# 3. (Tylko Mac) Zainstaluj Pods dla iOS
cd ios && pod install && cd ..

# 4. Uruchom aplikację
pnpm react-native run-android
# lub: pnpm react-native run-ios
```

**⚠️ Pierwszy build po instalacji potrwa 5-15 minut!**

### 3.2. .NET Backend

```bash
# 1. Przejdź do folderu .NET
cd dotnet/SolutionOrdersReact.Server

# 2. Przywróć pakiety NuGet
dotnet restore

# 3. Zbuduj projekt
dotnet build

# 4. Uruchom migracje (WAŻNE!)
dotnet ef database update

# 5. Uruchom seedery (jeśli baza jest pusta)
# Seedery uruchamiają się automatycznie przy starcie
# lub ręcznie przez endpoint /api/seed (jeśli zaimplementowany)

# 6. Uruchom serwer
dotnet run
```

### 3.3. Docker (alternatywa)

```bash
# 1. Przejdź do folderu dotnet
cd dotnet

# 2. Uruchom bazę danych
docker-compose -f docker-compose-db.yml up -d

# 3. Poczekaj 30s na inicjalizację SQL Server
Start-Sleep -Seconds 30  # PowerShell
# sleep 30  # Bash

# 4. Uruchom API
docker-compose up --build
```

---

## CZĘŚĆ 4: Rozwiązywanie problemów

### 4.1. React Native

**Problem: `Unable to resolve module`**
```bash
# Wyczyść cache Metro
pnpm start --reset-cache
```

**Problem: `SDK location not found`**
```bash
# Utwórz plik local.properties
echo "sdk.dir=C:\\Users\\<USER>\\AppData\\Local\\Android\\Sdk" > android/local.properties
```

**Problem: `Could not find node`**
```bash
# Sprawdź czy Node jest w PATH
node --version
```

### 4.2. .NET

**Problem: `Connection refused` do bazy**
```bash
# Sprawdź czy SQL Server działa
docker ps

# Sprawdź connection string w appsettings.json
```

**Problem: `Pending migrations`**
```bash
dotnet ef database update
```

---

## CZĘŚĆ 5: Checklist przed pakowaniem

- [ ] Usunięto `node_modules/`
- [ ] Usunięto `android/.gradle/`, `android/*/build/`
- [ ] Usunięto `ios/Pods/` (jeśli Mac)
- [ ] Usunięto `.NET bin/` i `obj/`
- [ ] Zachowano `package-lock.json` / `pnpm-lock.yaml`
- [ ] Zachowano `Migrations/`
- [ ] Zachowano `appsettings.*.json`
- [ ] Zachowano `local.properties` w `.gitignore` (nie pakuj!)
- [ ] Sprawdzono czy projekt zawiera seedery dla bazy

---

## CZĘŚĆ 6: Seedery bazy danych

**⚠️ WAŻNE:** Po rozpakowaniu i uruchomieniu projektu, baza danych będzie pusta!

Upewnij się, że projekt zawiera seedery (dane początkowe). 
Zobacz: **[SEEDERY-BAZY-DANYCH.md](./SEEDERY-BAZY-DANYCH.md)**

Seedery powinny dodać:
- Kategorie produktów
- Jednostki miary
- Przykładowe produkty
- Testowego użytkownika/pracownika

---

**Gotowe! Projekt jest przygotowany do spakowania. 📦**
