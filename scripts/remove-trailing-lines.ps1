# Usuwa puste linie na końcu plików
# Uruchom z głównego folderu projektu: .\scripts\remove-trailing-lines.ps1

param(
    [string]$Path = ".",
    [string[]]$Extensions = @("*.cs", "*.md", "*.json", "*.tsx", "*.ts", "*.kt", "*.xml", "*.gradle"),
    [string[]]$Exclude = @("node_modules", "bin", "obj", ".gradle", "build", ".git")
)

Write-Host "🔍 Szukanie plików..." -ForegroundColor Cyan

$excludePattern = ($Exclude | ForEach-Object { [regex]::Escape($_) }) -join "|"

$files = Get-ChildItem -Path $Path -Include $Extensions -Recurse -File -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch $excludePattern }

Write-Host "Znaleziono $($files.Count) plików do sprawdzenia" -ForegroundColor Yellow

$count = 0
foreach ($file in $files) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content) {
            # Usuń trailing whitespace i zostaw dokładnie jedną nową linię na końcu
            $trimmed = $content.TrimEnd("`r", "`n", " ", "`t") + "`n"
            
            if ($content -ne $trimmed) {
                Set-Content -Path $file.FullName -Value $trimmed -NoNewline -Encoding UTF8
                Write-Host "  ✓ $($file.Name)" -ForegroundColor Green
                $count++
            }
        }
    }
    catch {
        Write-Host "  ✗ $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "✅ Poprawiono $count plików" -ForegroundColor Green
