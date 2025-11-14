# Lekcja 11: Deployment, APK/IPA Build, CI/CD (2 godziny)

**Moduł:** Wdrożenie mobilne i backendowe  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Zbudować APK/IPA (Android/iOS)
- ✅ Przygotować build production React Native (release)
- ✅ Wypchnąć backend .NET do produkcji (Docker)
- ✅ Zbudować prosty pipeline CI/CD

---

## CZĘŚĆ 1: Build i deploy backendu Docker (30 minut)

### 1.1. Produkcyjny Dockerfile
- Upewnij się że w Dockerfile masz:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj", "SolutionOrdersReact.Server/"]
RUN dotnet restore "SolutionOrdersReact.Server/SolutionOrdersReact.Server.csproj"
COPY . .
WORKDIR "/src/SolutionOrdersReact.Server"
RUN dotnet publish "SolutionOrdersReact.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SolutionOrdersReact.Server.dll"]
```


### 1.2. Produkcyjny docker-compose
```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=TestReactDb;User=sa;Password=SAFE_PASSWORD;TrustServerCertificate=True
    depends_on:
      sqlserver:
        condition: service_healthy
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=SAFE_PASSWORD
      - MSSQL_PID=Developer
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SAFE_PASSWORD -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s
volumes:
  sqlserver-data:
```

### 1.3. Build/Deploy
```bash
docker-compose -f docker-compose.yml --env-file .env up --build -d
```


---

## CZĘŚĆ 2: Build APK (Android) (30 minut)

### 2.1. Najpierw konfiguracja release

**android/app/build.gradle:**
- Ustaw versionCode, versionName
- Sekcja signingConfigs → release [wpisz jawną lub beda podpisywane przez CI]

### 2.2. Build release APK
```bash
cd android
./gradlew assembleRelease
```
Plik: **android/app/build/outputs/apk/release/app-release.apk** — możesz instalować!

### 2.3. Install na urządzeniu
```bash
adb install app-release.apk
```

---

## CZĘŚĆ 3: Build iOS (IPA) (tylko Mac) (20 minut)
```bash
cd ios
pod install
cd ..
npx react-native run-ios --configuration Release
# Archive i eksport do App Store przez Xcode Organizer
```

---

## CZĘŚĆ 4: CI/CD pipeline (20 minut)

### 4.1. Podstawowy pipeline (GitHub Actions)

**.github/workflows/deploy.yml**:
```yaml
name: Deploy Docker Backend
on:
  push:
    branches:
      - main
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Docker Image
        run: docker build -t myapi:latest .
      - name: Login to registry
        run: echo ${{ secrets.DOCKER_PASSWORD }} | docker login -u ${{ secrets.DOCKER_USER }} --password-stdin my.registry.example.com
      - name: Push image
        run: docker push my.registry.example.com/myapi:latest
```

### 4.2. Pipeliny mobilne
- Polecam [expo Application Services (EAS Build)](https://docs.expo.dev/eas/), [Bitrise.io], [App Center]


---

## 📝 Zadania praktyczne
- Wypchnij backend do prywatnego registry Docker
- Zbuduj i prześlij .APK na telefon (bez Google Play)
- Zautomatyzuj migracje EF Core w pipeline

---

**GRATULACJE! Twój kurs end-to-end jest kompletny i produkcyjny! 🚀**
