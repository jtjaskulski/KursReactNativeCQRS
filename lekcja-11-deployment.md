# Lekcja 11: Deployment – Budowanie i Publikacja Aplikacji

> **Opracowano dla WSB-NLU 2026 - mgr. Jakub Jaskulski**

**Moduł:** Wdrożenie do produkcji  
**Czas trwania:** 3 godziny  
**Poziom:** Zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Budować APK i AAB dla Android
- ✅ Budować IPA dla iOS
- ✅ Konfigurować podpis aplikacji (signing)
- ✅ Dockeryzować backend dla produkcji
- ✅ Konfigurować CI/CD z GitHub Actions
- ✅ Przygotować aplikację do publikacji w sklepach

---

## CZĘŚĆ 1: Teoria Deployment (15 minut)

### 1.1. Przegląd procesu

**SCRIPT dla prowadzącego:**

> „Deployment to nie tylko 'wrzucenie do sklepu'. To cały proces: budowanie, testowanie, podpisywanie, optymalizacja i publikacja. Każdy krok jest ważny."

**Diagram procesu:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        PROCES DEPLOYMENT                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐  │
│   │   DEVELOP   │───▶│    BUILD    │───▶│    TEST     │───▶│   DEPLOY    │  │
│   └─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘  │
│         │                  │                  │                  │          │
│         ▼                  ▼                  ▼                  ▼          │
│   • Kod źródłowy     • Release Build    • QA Testing       • Store Upload  │
│   • Feature Branch   • Signing          • Beta Testing     • Production    │
│   • Code Review      • ProGuard         • Regression       • Monitoring    │
│                      • Asset Bundle                                         │
│                                                                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   📱 MOBILE (React Native)           🖥️ BACKEND (.NET)                      │
│   ┌─────────────────────────┐        ┌─────────────────────────┐            │
│   │ Android:                │        │ Docker:                 │            │
│   │ • APK (debug/testing)   │        │ • Build Image           │            │
│   │ • AAB (Google Play)     │        │ • Push to Registry      │            │
│   ├─────────────────────────┤        │ • Deploy to Server      │            │
│   │ iOS:                    │        │                         │            │
│   │ • IPA (TestFlight)      │        │ Options:                │            │
│   │ • App Store             │        │ • Azure/AWS/GCP         │            │
│   └─────────────────────────┘        │ • VPS (DigitalOcean)    │            │
│                                      │ • Kubernetes            │            │
│                                      └─────────────────────────┘            │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. APK vs AAB

| Format | Opis | Użycie |
|--------|------|--------|
| **APK** | Tradycyjny plik instalacyjny | Testy, beta, sideloading |
| **AAB** | Android App Bundle | Google Play (wymagany!) |

**AAB zalety:**
- Mniejszy rozmiar (o 15-20%)
- Google optymalizuje dla każdego urządzenia
- Wymagany od 2021 dla nowych aplikacji

---

## CZĘŚĆ 2: Budowanie Android (45 minut)

### 2.1. Generowanie klucza do podpisu

**SCRIPT dla prowadzącego:**

> „Klucz do podpisu to NAJWAŻNIEJSZY plik w projekcie. BEZ NIEGO nie zaktualizujesz aplikacji w sklepie. NIGDY go nie zgub i NIGDY nie commituj do repozytorium!"

**Generowanie klucza:**

```bash
# Utwórz folder na klucze (dodaj do .gitignore!)
mkdir -p android/app/keystores

# Generuj klucz (zapamiętaj hasła!)
keytool -genkeypair -v \
  -storetype PKCS12 \
  -keystore android/app/keystores/release.keystore \
  -alias orders-app-key \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000

# Zostaniesz poproszony o:
# 1. Hasło do keystore (np: K3ystore@Pass)
# 2. Imię i nazwisko
# 3. Organizacja
# 4. Miasto, województwo, kraj
# 5. Hasło do klucza (użyj tego samego co keystore)
```

### 2.2. Konfiguracja gradle.properties

**android/gradle.properties:**

```properties
# ... istniejące ustawienia ...

# ========== RELEASE SIGNING ==========
# UWAGA: NIE COMMITUJ HASEŁ! Użyj zmiennych środowiskowych w CI/CD

# Dla development (lokalne hasła)
MYAPP_UPLOAD_STORE_FILE=keystores/release.keystore
MYAPP_UPLOAD_KEY_ALIAS=orders-app-key
MYAPP_UPLOAD_STORE_PASSWORD=K3ystore@Pass
MYAPP_UPLOAD_KEY_PASSWORD=K3ystore@Pass

# Dla CI/CD użyj:
# MYAPP_UPLOAD_STORE_PASSWORD=${KEYSTORE_PASSWORD}
# MYAPP_UPLOAD_KEY_PASSWORD=${KEY_PASSWORD}
```

### 2.3. Konfiguracja build.gradle

**android/app/build.gradle:**

```groovy
android {
    // ... istniejące ustawienia ...

    defaultConfig {
        applicationId "com.solutionordersmobile"
        minSdkVersion rootProject.ext.minSdkVersion
        targetSdkVersion rootProject.ext.targetSdkVersion
        versionCode 1
        versionName "1.0.0"
    }

    // ========== SIGNING CONFIGS ==========
    signingConfigs {
        debug {
            storeFile file('debug.keystore')
            storePassword 'android'
            keyAlias 'androiddebugkey'
            keyPassword 'android'
        }

        release {
            if (project.hasProperty('MYAPP_UPLOAD_STORE_FILE')) {
                storeFile file(MYAPP_UPLOAD_STORE_FILE)
                storePassword MYAPP_UPLOAD_STORE_PASSWORD
                keyAlias MYAPP_UPLOAD_KEY_ALIAS
                keyPassword MYAPP_UPLOAD_KEY_PASSWORD
            }
        }
    }

    // ========== BUILD TYPES ==========
    buildTypes {
        debug {
            signingConfig signingConfigs.debug
            debuggable true
        }

        release {
            signingConfig signingConfigs.release
            minifyEnabled true          // ProGuard - optymalizacja kodu
            shrinkResources true        // Usuwa nieużywane zasoby
            debuggable false

            proguardFiles getDefaultProguardFile('proguard-android-optimize.txt'),
                          'proguard-rules.pro'
        }
    }

    // ========== FLAVORS (opcjonalne) ==========
    flavorDimensions "environment"

    productFlavors {
        dev {
            dimension "environment"
            applicationIdSuffix ".dev"
            versionNameSuffix "-dev"
            buildConfigField "String", "API_URL", '"http://192.168.1.100:5000/api"'
        }

        staging {
            dimension "environment"
            applicationIdSuffix ".staging"
            versionNameSuffix "-staging"
            buildConfigField "String", "API_URL", '"https://staging-api.example.com/api"'
        }

        prod {
            dimension "environment"
            buildConfigField "String", "API_URL", '"https://api.example.com/api"'
        }
    }
}
```

### 2.4. ProGuard Rules

**android/app/proguard-rules.pro:**

```proguard
# React Native
-keep class com.facebook.react.** { *; }
-keep class com.facebook.hermes.** { *; }
-keep class com.facebook.jni.** { *; }

# Hermes engine
-keep class com.facebook.hermes.unicode.** { *; }
-keep class com.facebook.jni.** { *; }

# Networking
-keepclassmembers class * {
    @com.facebook.react.uimanager.annotations.ReactProp *;
}

# OkHttp
-dontwarn okhttp3.**
-dontwarn okio.**
-keep class okhttp3.** { *; }

# Gson (jeśli używasz)
-keep class com.google.gson.** { *; }
-keepattributes Signature

# Keep model classes
-keep class com.solutionordersmobile.models.** { *; }

# Native modules
-keep class com.solutionordersmobile.vibration.** { *; }
```

### 2.5. Budowanie APK

```bash
# Przejdź do folderu android
cd android

# Wyczyść poprzednie buildy
./gradlew clean

# Zbuduj APK release
./gradlew assembleRelease

# Albo dla konkretnego flavoru:
./gradlew assembleProdRelease

# APK znajdziesz w:
# android/app/build/outputs/apk/release/app-release.apk
# lub
# android/app/build/outputs/apk/prod/release/app-prod-release.apk
```

### 2.6. Budowanie AAB (dla Google Play)

```bash
cd android

# Zbuduj AAB
./gradlew bundleRelease

# Albo dla konkretnego flavoru:
./gradlew bundleProdRelease

# AAB znajdziesz w:
# android/app/build/outputs/bundle/release/app-release.aab
```

### 2.7. Testowanie APK na urządzeniu

```bash
# Lista podłączonych urządzeń
adb devices

# Instalacja APK
adb install android/app/build/outputs/apk/release/app-release.apk

# Jeśli już zainstalowane - reinstall
adb install -r android/app/build/outputs/apk/release/app-release.apk

# Odinstalowanie
adb uninstall com.solutionordersmobile
```

---

## CZĘŚĆ 3: Budowanie iOS (35 minut)

### 3.1. Wymagania

- macOS (Xcode działa tylko na Mac)
- Xcode 14+ z Command Line Tools
- Apple Developer Account ($99/rok)
- CocoaPods zainstalowane

### 3.2. Przygotowanie projektu

```bash
# Przejdź do folderu iOS
cd ios

# Zainstaluj dependencies
pod install --repo-update

# Otwórz w Xcode
open SolutionOrdersMobile.xcworkspace
```

### 3.3. Konfiguracja w Xcode

**W Xcode:**

1. Wybierz projekt w nawigatorze (lewy panel)
2. Wybierz target "SolutionOrdersMobile"
3. Zakładka "Signing & Capabilities":
   - Team: Wybierz swój Apple Developer Team
   - Bundle Identifier: `com.yourcompany.solutionordersmobile`
   - Zaznacz "Automatically manage signing"

4. Zakładka "General":
   - Version: 1.0.0
   - Build: 1

### 3.4. Info.plist dla produkcji

**ios/SolutionOrdersMobile/Info.plist (dodatkowe wpisy):**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- Podstawowe info -->
    <key>CFBundleDisplayName</key>
    <string>Orders Mobile</string>

    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>

    <key>CFBundleVersion</key>
    <string>1</string>

    <!-- Wyłącz HTTP dla produkcji -->
    <key>NSAppTransportSecurity</key>
    <dict>
        <key>NSAllowsArbitraryLoads</key>
        <false/>
        <key>NSExceptionDomains</key>
        <dict>
            <key>api.example.com</key>
            <dict>
                <key>NSExceptionAllowsInsecureHTTPLoads</key>
                <false/>
                <key>NSIncludesSubdomains</key>
                <true/>
            </dict>
        </dict>
    </dict>

    <!-- Privacy descriptions -->
    <key>NSCameraUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu do kamery, aby robić zdjęcia produktów</string>

    <key>NSPhotoLibraryUsageDescription</key>
    <string>Aplikacja potrzebuje dostępu do galerii, aby wybierać zdjęcia</string>

    <key>NSLocationWhenInUseUsageDescription</key>
    <string>Aplikacja potrzebuje lokalizacji, aby pokazać najbliższe sklepy</string>
</dict>
</plist>
```

### 3.5. Budowanie Archive

**W Xcode:**

1. Wybierz schemat "Release"
2. Wybierz urządzenie "Any iOS Device (arm64)"
3. Product → Archive
4. Poczekaj na zakończenie (kilka minut)
5. Organizer otworzy się automatycznie

### 3.6. Eksport IPA

**Z Organizera:**

1. Wybierz utworzone Archive
2. Kliknij "Distribute App"
3. Wybierz metodę dystrybucji:
   - **App Store Connect** - dla App Store/TestFlight
   - **Ad Hoc** - dla testerów (wymagane UDID urządzeń)
   - **Development** - dla development

4. Postępuj zgodnie z kreatorami
5. IPA zostanie zapisane w wybranej lokalizacji

### 3.7. Upload do App Store Connect

```bash
# Alternatywnie przez xcrun
xcrun altool --upload-app \
  --type ios \
  --file "path/to/app.ipa" \
  --username "your@apple.id" \
  --password "@keychain:AC_PASSWORD"
```

---

## CZĘŚĆ 4: Dockeryzacja Backend dla Produkcji (40 minut)

### 4.1. Multi-stage Dockerfile

**Dockerfile:**

```dockerfile
# =========================================
# STAGE 1: BUILD
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopiuj tylko pliki projektów (dla cache'owania warstw)
COPY ["SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj", "SolutionOrdersReact.Server/"]
RUN dotnet restore "SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj"

# Kopiuj resztę kodu
COPY . .

# Buduj w Release
WORKDIR "/src/SolutionOrdersReact.Server"
RUN dotnet build "SolutionOrdersReact.Server.csproj" -c Release -o /app/build

# =========================================
# STAGE 2: PUBLISH
# =========================================
FROM build AS publish
RUN dotnet publish "SolutionOrdersReact.Server.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishSingleFile=false

# =========================================
# STAGE 3: RUNTIME (mały obraz!)
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

# Utwórz non-root użytkownika dla bezpieczeństwa
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

WORKDIR /app

# Kopiuj opublikowaną aplikację
COPY --from=publish /app/publish .

# Zmień właściciela plików
RUN chown -R appuser:appgroup /app

# Przełącz na non-root użytkownika
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/health || exit 1

# Uruchom aplikację
ENTRYPOINT ["dotnet", "SolutionOrdersReact.Server.dll"]
```

### 4.2. docker-compose.production.yml

**docker-compose.production.yml:**

```yaml
version: '3.8'

services:
  # =========================================
  # API Backend
  # =========================================
  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: orders-api
    restart: unless-stopped
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
      - Serilog__MinimumLevel__Default=Information
    depends_on:
      sqlserver:
        condition: service_healthy
    networks:
      - orders-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  # =========================================
  # SQL Server Database
  # =========================================
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: orders-db
    restart: unless-stopped
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=${SA_PASSWORD}
      - MSSQL_PID=Express  # Express dla oszczędności (free)
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - orders-network
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -Q "SELECT 1" -C || exit 1
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 30s
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  # =========================================
  # Nginx Reverse Proxy (opcjonalnie)
  # =========================================
  nginx:
    image: nginx:alpine
    container_name: orders-nginx
    restart: unless-stopped
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl:/etc/nginx/ssl:ro
    depends_on:
      - api
    networks:
      - orders-network

networks:
  orders-network:
    driver: bridge

volumes:
  sqlserver-data:
    driver: local
```

### 4.3. Nginx Configuration

**nginx/nginx.conf:**

```nginx
events {
    worker_connections 1024;
}

http {
    upstream api {
        server api:8080;
    }

    # Rate limiting
    limit_req_zone $binary_remote_addr zone=api:10m rate=10r/s;

    server {
        listen 80;
        server_name api.example.com;

        # Redirect HTTP to HTTPS
        return 301 https://$server_name$request_uri;
    }

    server {
        listen 443 ssl http2;
        server_name api.example.com;

        # SSL Configuration
        ssl_certificate /etc/nginx/ssl/fullchain.pem;
        ssl_certificate_key /etc/nginx/ssl/privkey.pem;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256;
        ssl_prefer_server_ciphers off;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Strict-Transport-Security "max-age=31536000" always;

        # API Proxy
        location /api {
            limit_req zone=api burst=20 nodelay;

            proxy_pass http://api;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;

            # Timeouts
            proxy_connect_timeout 60s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
        }

        # Health check
        location /health {
            proxy_pass http://api;
            access_log off;
        }
    }
}
```

### 4.4. Environment file

**.env.production (NIE COMMITUJ!):**

```env
# Database
SA_PASSWORD=VeryStrong@Password123!
DB_CONNECTION_STRING=Server=sqlserver;Database=OrdersDb;User=sa;Password=VeryStrong@Password123!;TrustServerCertificate=True;

# API
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Production

# Logging
LOG_LEVEL=Information
```

### 4.5. Deployment script

**deploy.sh:**

```bash
#!/bin/bash
set -e

echo "🚀 Starting deployment..."

# Zmienne
COMPOSE_FILE="docker-compose.production.yml"
ENV_FILE=".env.production"

# Sprawdź czy .env istnieje
if [ ! -f "$ENV_FILE" ]; then
    echo "❌ Error: $ENV_FILE not found!"
    exit 1
fi

# Załaduj zmienne środowiskowe
export $(cat $ENV_FILE | xargs)

echo "📦 Pulling latest images..."
docker-compose -f $COMPOSE_FILE pull

echo "🔨 Building API..."
docker-compose -f $COMPOSE_FILE build --no-cache api

echo "⏹️ Stopping old containers..."
docker-compose -f $COMPOSE_FILE down

echo "▶️ Starting new containers..."
docker-compose -f $COMPOSE_FILE up -d

echo "⏳ Waiting for services to start..."
sleep 10

echo "🔍 Checking health..."
curl -f http://localhost:5000/health || {
    echo "❌ Health check failed!"
    docker-compose -f $COMPOSE_FILE logs api
    exit 1
}

echo "✅ Deployment successful!"
docker-compose -f $COMPOSE_FILE ps
```

---

## CZĘŚĆ 5: CI/CD z GitHub Actions (35 minut)

### 5.1. Workflow dla Android

**.github/workflows/android.yml:**

```yaml
name: Android Build

on:
  push:
    branches: [main, develop]
    paths:
      - 'rn/SolutionOrdersMobile/**'
  pull_request:
    branches: [main]
  workflow_dispatch:  # Manual trigger

env:
  WORKING_DIRECTORY: rn/SolutionOrdersMobile

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: '${{ env.WORKING_DIRECTORY }}/package-lock.json'

      - name: Setup Java
        uses: actions/setup-java@v4
        with:
          distribution: 'temurin'
          java-version: '17'

      - name: Setup Android SDK
        uses: android-actions/setup-android@v3

      - name: Install dependencies
        working-directory: ${{ env.WORKING_DIRECTORY }}
        run: npm ci

      - name: Decode Keystore
        working-directory: ${{ env.WORKING_DIRECTORY }}
        run: |
          echo "${{ secrets.ANDROID_KEYSTORE_BASE64 }}" | base64 -d > android/app/keystores/release.keystore

      - name: Build APK
        working-directory: ${{ env.WORKING_DIRECTORY }}/android
        env:
          MYAPP_UPLOAD_STORE_FILE: keystores/release.keystore
          MYAPP_UPLOAD_KEY_ALIAS: ${{ secrets.ANDROID_KEY_ALIAS }}
          MYAPP_UPLOAD_STORE_PASSWORD: ${{ secrets.ANDROID_KEYSTORE_PASSWORD }}
          MYAPP_UPLOAD_KEY_PASSWORD: ${{ secrets.ANDROID_KEY_PASSWORD }}
        run: |
          chmod +x ./gradlew
          ./gradlew assembleRelease --no-daemon

      - name: Build AAB (for Play Store)
        working-directory: ${{ env.WORKING_DIRECTORY }}/android
        env:
          MYAPP_UPLOAD_STORE_FILE: keystores/release.keystore
          MYAPP_UPLOAD_KEY_ALIAS: ${{ secrets.ANDROID_KEY_ALIAS }}
          MYAPP_UPLOAD_STORE_PASSWORD: ${{ secrets.ANDROID_KEYSTORE_PASSWORD }}
          MYAPP_UPLOAD_KEY_PASSWORD: ${{ secrets.ANDROID_KEY_PASSWORD }}
        run: ./gradlew bundleRelease --no-daemon

      - name: Upload APK
        uses: actions/upload-artifact@v4
        with:
          name: app-release.apk
          path: ${{ env.WORKING_DIRECTORY }}/android/app/build/outputs/apk/release/app-release.apk

      - name: Upload AAB
        uses: actions/upload-artifact@v4
        with:
          name: app-release.aab
          path: ${{ env.WORKING_DIRECTORY }}/android/app/build/outputs/bundle/release/app-release.aab

  # =========================================
  # Upload to Google Play (only on main)
  # =========================================
  deploy:
    needs: build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - name: Download AAB
        uses: actions/download-artifact@v4
        with:
          name: app-release.aab

      - name: Upload to Google Play
        uses: r0adkll/upload-google-play@v1
        with:
          serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
          packageName: com.solutionordersmobile
          releaseFiles: app-release.aab
          track: internal  # internal, alpha, beta, production
          status: completed
```

### 5.2. Workflow dla Backend

**.github/workflows/backend.yml:**

```yaml
name: Backend Build & Deploy

on:
  push:
    branches: [main]
    paths:
      - 'CQRSReactNative/**'
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}/orders-api

jobs:
  # =========================================
  # BUILD & TEST
  # =========================================
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        working-directory: CQRSReactNative
        run: dotnet restore

      - name: Build
        working-directory: CQRSReactNative
        run: dotnet build --no-restore -c Release

      - name: Test
        working-directory: CQRSReactNative
        run: dotnet test --no-build -c Release --verbosity normal

  # =========================================
  # DOCKER BUILD & PUSH
  # =========================================
  docker:
    needs: build
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=sha,prefix=
            type=ref,event=branch
            type=raw,value=latest,enable=${{ github.ref == 'refs/heads/main' }}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: ./CQRSReactNative
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}

  # =========================================
  # DEPLOY TO SERVER
  # =========================================
  deploy:
    needs: docker
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - name: Deploy to Server
        uses: appleboy/ssh-action@v1.0.0
        with:
          host: ${{ secrets.SERVER_HOST }}
          username: ${{ secrets.SERVER_USER }}
          key: ${{ secrets.SERVER_SSH_KEY }}
          script: |
            cd /opt/orders-app
            docker-compose pull api
            docker-compose up -d api
            docker image prune -f
            echo "Deployment completed at $(date)"
```

### 5.3. Secrets do skonfigurowania

**W GitHub → Settings → Secrets and variables → Actions:**

| Secret | Opis |
|--------|------|
| `ANDROID_KEYSTORE_BASE64` | `base64 -w 0 release.keystore` |
| `ANDROID_KEYSTORE_PASSWORD` | Hasło do keystore |
| `ANDROID_KEY_PASSWORD` | Hasło do klucza |
| `ANDROID_KEY_ALIAS` | Alias klucza |
| `GOOGLE_PLAY_SERVICE_ACCOUNT` | JSON z Google Play Console |
| `SERVER_HOST` | IP serwera produkcyjnego |
| `SERVER_USER` | Użytkownik SSH |
| `SERVER_SSH_KEY` | Klucz prywatny SSH |

---

## CZĘŚĆ 6: Publikacja w Sklepach (25 minut)

### 6.1. Google Play Console

**Kroki:**

1. **Utwórz konto deweloperskie** ($25 jednorazowo)
   - https://play.google.com/console

2. **Utwórz aplikację**
   - Nazwa
   - Język domyślny
   - Typ (aplikacja/gra)
   - Bezpłatna/płatna

3. **Wypełnij Store Listing**
   - Tytuł (max 50 znaków)
   - Krótki opis (max 80 znaków)
   - Pełny opis (max 4000 znaków)
   - Ikona (512x512 PNG)
   - Feature graphic (1024x500)
   - Screenshots (min 2)

4. **Content Rating**
   - Wypełnij kwestionariusz IARC

5. **App Privacy**
   - Data safety form
   - Polityka prywatności URL

6. **Release Management**
   - Wgraj AAB
   - Internal → Closed → Open → Production

### 6.2. App Store Connect

**Kroki:**

1. **Utwórz konto deweloperskie** ($99/rok)
   - https://developer.apple.com

2. **App Store Connect**
   - Utwórz nową aplikację
   - Bundle ID musi pasować

3. **App Information**
   - Nazwa
   - Kategoria
   - Opis (max 4000 znaków)
   - Słowa kluczowe (max 100 znaków)
   - Support URL, Privacy URL

4. **Screenshots**
   - 6.5" (iPhone 14 Pro Max)
   - 5.5" (iPhone 8 Plus)
   - 12.9" iPad (jeśli wspierasz)

5. **TestFlight** (wewnętrzne testy)
   - Wgraj IPA przez Xcode
   - Dodaj testerów
   - Testuj przed App Review

6. **App Review**
   - Wypełnij informacje o recenzji
   - Podaj dane testowe jeśli potrzebne
   - Oczekuj 24-48h na recenzję

### 6.3. Checklist przed publikacją

**Android:**
- [ ] Wersja i build number zaktualizowane
- [ ] ProGuard włączony
- [ ] Klucz release podpisany
- [ ] AAB zbudowane i przetestowane
- [ ] Wszystkie permissions opisane
- [ ] Screenshots dla różnych urządzeń
- [ ] Polityka prywatności

**iOS:**
- [ ] Wersja i build number zaktualizowane
- [ ] Archive zbudowany w Release
- [ ] Signing z właściwym Team
- [ ] Info.plist kompletny
- [ ] Screenshots dla wymaganych urządzeń
- [ ] Privacy descriptions wypełnione

**Backend:**
- [ ] Docker image zbudowany
- [ ] Zmienne środowiskowe ustawione
- [ ] HTTPS skonfigurowane
- [ ] CORS poprawnie ustawione
- [ ] Health check endpoint działa
- [ ] Logi działają

---

## 📝 Zadania Praktyczne

### Zadanie 1: Build warianty
Skonfiguruj 3 build flavors: dev, staging, prod z różnymi API URLs.

### Zadanie 2: Automatyzacja wersji
Zaimplementuj automatyczne zwiększanie versionCode w CI/CD.

### Zadanie 3: Code Push
Zintegruj Microsoft CodePush do szybkich aktualizacji bez publikacji w sklepie.

### Zadanie 4: Monitoring
Dodaj Firebase Crashlytics do śledzenia crashów w produkcji.

### Zadanie 5: Beta testing
Skonfiguruj TestFlight dla iOS i Internal Testing dla Android.

---

## 🔍 Pytania Kontrolne

1. Czym różni się APK od AAB?
2. Dlaczego nie commitujemy keystore i haseł do repozytorium?
3. Co to jest ProGuard i do czego służy?
4. Jak działa multi-stage Docker build?
5. Po co używamy GitHub Secrets?
6. Jakie są różnice między Internal, Alpha, Beta i Production track w Google Play?

---

## ➡️ Zakończenie Kursu

**Gratulacje! 🎉 Ukończyłeś kurs React Native + CQRS!**

### Czego się nauczyłeś:

1. ✅ TypeScript - typowanie statyczne
2. ✅ React Native - komponenty, nawigacja, hooki
3. ✅ CQRS - Vertical Slice Architecture
4. ✅ Docker - konteneryzacja bazy i API
5. ✅ CRUD Mobile - integracja frontend-backend
6. ✅ Relacje 1:M - Entity Framework
7. ✅ Zamówienia - Master-Detail
8. ✅ Walidacja - FluentValidation + Pipeline
9. ✅ Paginacja - optymalizacja zapytań
10. ✅ Natywne Moduły - kamera, GPS, permissions
11. ✅ Deployment - CI/CD, publikacja

### Następne kroki:

- 📱 Zbuduj swoją aplikację
- 🔒 Dodaj autoryzację (JWT)
- 📊 Zintegruj analitykę
- 🧪 Napisz testy E2E
- 🌐 Wdróż na produkcję

**Powodzenia w dalszej nauce! 🚀**
