# BIM Ninja - Auto Setup Script
# This script copies Revit DLLs and pushes to GitHub
# Run this in PowerShell as Administrator

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  BIM Ninja - GitHub Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Find the project folder
$projectPath = Read-Host "Enter the path to your extracted project folder (e.g., C:\Projects\BimNinja-SheetExporter)"
if (-not (Test-Path $projectPath)) {
    Write-Host "ERROR: Folder not found: $projectPath" -ForegroundColor Red
    exit 1
}

$libsPath = Join-Path $projectPath "libs"
New-Item -ItemType Directory -Force -Path $libsPath | Out-Null

# Step 2: Find Revit installation
Write-Host "Searching for Revit installation..." -ForegroundColor Yellow
$revitPaths = @(
    "C:\Program Files\Autodesk\Revit 2024",
    "C:\Program Files\Autodesk\Revit 2023",
    "C:\Program Files\Autodesk\Revit 2022",
    "C:\Program Files\Autodesk\Revit 2021",
    "C:\Program Files\Autodesk\Revit 2020"
)

$foundRevit = $null
foreach ($path in $revitPaths) {
    if (Test-Path "$path\RevitAPI.dll") {
        $foundRevit = $path
        Write-Host "Found Revit: $path" -ForegroundColor Green
        break
    }
}

if (-not $foundRevit) {
    Write-Host "ERROR: Revit not found in standard locations." -ForegroundColor Red
    Write-Host "Please manually copy RevitAPI.dll and RevitAPIUI.dll to: $libsPath" -ForegroundColor Yellow
    Read-Host "Press Enter after copying the files..."
    
    if (-not (Test-Path "$libsPath\RevitAPI.dll")) {
        Write-Host "ERROR: RevitAPI.dll still not found. Exiting." -ForegroundColor Red
        exit 1
    }
} else {
    # Copy DLLs
    Write-Host "Copying Revit DLLs to project..." -ForegroundColor Yellow
    Copy-Item "$foundRevit\RevitAPI.dll" "$libsPath\RevitAPI.dll" -Force
    Copy-Item "$foundRevit\RevitAPIUI.dll" "$libsPath\RevitAPIUI.dll" -Force
    Write-Host "DLLs copied successfully!" -ForegroundColor Green
}

# Step 3: Check if Git is installed
try {
    $gitVersion = git --version 2>$null
    Write-Host "Git found: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Git is not installed." -ForegroundColor Red
    Write-Host "Please download and install Git from: https://git-scm.com/download/win" -ForegroundColor Yellow
    Write-Host "After installing, run this script again." -ForegroundColor Yellow
    exit 1
}

# Step 4: Initialize Git and push to GitHub
Write-Host ""
Write-Host "Setting up Git repository..." -ForegroundColor Yellow
Set-Location $projectPath

# Initialize git if not already
try {
    git status 2>$null | Out-Null
} catch {
    git init
}

# Add all files
git add .

# Commit
git commit -m "Add BIM Ninja Sheet Exporter with Revit API DLLs" --allow-empty

# Set remote
$repoUrl = Read-Host "Enter your GitHub repo URL (e.g., https://github.com/mayassr1992-Mh/BimNinja-SheetExporter.git)"
git remote remove origin 2>$null
git remote add origin $repoUrl

# Determine branch name
$branch = "main"
try {
    git show-ref --verify --quiet refs/heads/main
} catch {
    $branch = "master"
}

# Push to GitHub
Write-Host ""
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
Write-Host "You may need to enter your GitHub username and password/token." -ForegroundColor Cyan
git push -u origin $branch --force

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  SUCCESS! Push completed." -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Go to GitHub Actions to see the build:"
    Write-Host "https://github.com/mayassr1992-Mh/BimNinja-SheetExporter/actions"
    Write-Host ""
    Write-Host "Wait 2-3 minutes for the build to complete." -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "Push failed. You may need to:" -ForegroundColor Red
    Write-Host "1. Check your GitHub credentials" -ForegroundColor Yellow
    Write-Host "2. Create a personal access token (Settings > Developer settings > Personal access tokens)" -ForegroundColor Yellow
    Write-Host "3. Use the token as your password when prompted" -ForegroundColor Yellow
}

Read-Host "Press Enter to exit"
