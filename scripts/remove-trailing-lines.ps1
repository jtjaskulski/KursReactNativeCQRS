# Usuwa puste linie na koncu plikow
# Uruchom z glownego folderu projektu: .\scripts\remove-trailing-lines.ps1
# UWAGA: Pliki .md sa wykluczone - VS je automatycznie obsluzy dzieki .editorconfig

param(
    [string]$Path = ".",
    [string[]]$Extensions = @("*.cs", "*.json", "*.tsx", "*.ts", "*.kt", "*.xml", "*.gradle", "*.editorconfig", "*.yml", "*.yaml"),
    [string[]]$Exclude = @("node_modules", "bin", "obj", ".gradle", "build", ".git", "pnpm-lock.yaml", "package-lock.json")
)

Write-Host "Szukanie plikow..." -ForegroundColor Cyan

$excludePattern = ($Exclude | ForEach-Object { [regex]::Escape($_) }) -join "|"

$files = Get-ChildItem -Path $Path -Include $Extensions -Recurse -File -ErrorAction SilentlyContinue | Where-Object { $_.FullName -notmatch $excludePattern }

Write-Host "Znaleziono $($files.Count) plikow do sprawdzenia" -ForegroundColor Yellow

$count = 0
foreach ($file in $files) {
    try {
        # Czytaj jako bajty zeby wykryc BOM
        $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
        
        if ($bytes.Length -eq 0) { continue }
        
        # Wykryj BOM i encoding
        $hasBom = $false
        $encoding = New-Object System.Text.UTF8Encoding($false)
        
        if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            $hasBom = $true
            $encoding = New-Object System.Text.UTF8Encoding($true)
            $content = $encoding.GetString($bytes, 3, $bytes.Length - 3)
        } else {
            $content = $encoding.GetString($bytes)
        }
        
        $trimmed = $content.TrimEnd()
        
        if ($content -ne $trimmed) {
            [System.IO.File]::WriteAllText($file.FullName, $trimmed, $encoding)
            Write-Host "  OK $($file.Name)" -ForegroundColor Green
            $count++
        }
    }
    catch {
        Write-Host "  FAIL $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Poprawiono $count plikow" -ForegroundColor Green
Write-Host "UWAGA: Pliki .md nie sa modyfikowane - VS je obsluzy automatycznie" -ForegroundColor Yellow