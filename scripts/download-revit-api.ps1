#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads or copies Revit API DLLs for CI/local builds.
.DESCRIPTION
    This script attempts to obtain RevitAPI.dll and RevitAPIUI.dll
    by checking local Revit installations first, then falling back to download.
#>

param(
    [string]$OutputDir = "$PSScriptRoot\..\libs",
    [string]$RevitVersion = "2024"
)

$ErrorActionPreference = "Stop"
$OutputDir = Resolve-Path $OutputDir -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Path
if (-not $OutputDir) { $OutputDir = (Resolve-Path "$PSScriptRoot\..\libs").Path }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Revit API DLL Acquisition Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Output directory: $OutputDir"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$dlls = @("RevitAPI.dll", "RevitAPIUI.dll")
$foundAll = $true

# --- Step 1: Check if already present ---
$allPresent = $true
foreach ($dll in $dlls) {
    if (-not (Test-Path (Join-Path $OutputDir $dll))) { $allPresent = $false }
}
if ($allPresent) {
    Write-Host "All Revit API DLLs already present in $OutputDir." -ForegroundColor Green
    exit 0
}

# --- Step 2: Try local Revit installation ---
$revitPaths = @(
    "C:\Program Files\Autodesk\Revit $RevitVersion",
    "C:\Program Files\Autodesk\Revit 2024",
    "C:\Program Files\Autodesk\Revit 2023",
    "C:\Program Files\Autodesk\Revit 2022"
)

foreach ($path in $revitPaths) {
    if (Test-Path $path) {
        Write-Host "Found local Revit installation: $path" -ForegroundColor Green
        foreach ($dll in $dlls) {
            $src = Join-Path $path $dll
            $dest = Join-Path $OutputDir $dll
            if (Test-Path $src) {
                Copy-Item -Path $src -Destination $dest -Force
                Write-Host "  Copied: $dll" -ForegroundColor Green
            } else {
                Write-Host "  Missing: $dll in $path" -ForegroundColor Yellow
                $foundAll = $false
            }
        }
        if ($foundAll) { exit 0 }
        break
    }
}

# --- Step 3: Try downloading from known sources ---
Write-Host "Local Revit not found. Attempting download..." -ForegroundColor Yellow

# Known GitHub repos that host Revit API DLLs (these are community mirrors)
$downloadSources = @(
    "https://github.com/Autodesk-Forge/forge-api-dotnet-design.automation/raw/master/test/Forge.DesignAutomation.Revit/2024/RevitAPI.dll",
    "https://raw.githubusercontent.com/Nice3point/RevitToolkit/main/libs/RevitAPI.dll"
)

# Note: The above URLs are examples. In practice, many developers include
# the DLLs in their own repo or use a private artifact store.

Write-Host "Download URLs are not guaranteed. The recommended approach is:" -ForegroundColor Yellow
Write-Host "  1. Copy DLLs from your local Revit installation to libs/" -ForegroundColor White
Write-Host "  2. Commit them to your GitHub repository" -ForegroundColor White
Write-Host "  3. The CI build will then use them automatically" -ForegroundColor White
Write-Host ""

# --- Step 4: Try to download anyway (best effort) ---
try {
    foreach ($dll in $dlls) {
        $dest = Join-Path $OutputDir $dll
        # Try a known raw URL pattern (may fail)
        $url = "https://raw.githubusercontent.com/Autodesk/revit-api-samples/main/libs/2024/$dll"
        Write-Host "  Trying: $url" -ForegroundColor Gray
        try {
            Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing -ErrorAction Stop
            Write-Host "  Downloaded: $dll" -ForegroundColor Green
        } catch {
            Write-Host "  Failed to download $dll" -ForegroundColor Red
            $foundAll = $false
        }
    }
} catch {
    $foundAll = $false
}

if (-not $foundAll) {
    Write-Host ""
    Write-Host "ERROR: Could not obtain all Revit API DLLs." -ForegroundColor Red
    Write-Host ""
    Write-Host "Please manually copy these files from your Revit installation:" -ForegroundColor Yellow
    Write-Host "  C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll   -> $OutputDir\RevitAPI.dll" -ForegroundColor White
    Write-Host "  C:\Program Files\Autodesk\Revit 2024\RevitAPIUI.dll -> $OutputDir\RevitAPIUI.dll" -ForegroundColor White
    Write-Host ""
    Write-Host "Then commit them to your GitHub repository." -ForegroundColor Yellow
    exit 1
}

exit 0
