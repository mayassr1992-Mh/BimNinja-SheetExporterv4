@echo off
setlocal EnableDelayedExpansion
chcp 65001 >nul

echo ========================================
echo   BIM Ninja - Auto Setup ^& Git Push
echo ========================================
echo.

:: Step 1: Find project folder
set "PROJECT="
set /p PROJECT="Enter your project folder path (e.g. C:\BimNinja): "
if not exist "%PROJECT%" (
    echo ERROR: Folder not found: %PROJECT%
    pause
    exit /b 1
)

set "LIBS=%PROJECT%\libs"
if not exist "%LIBS%" mkdir "%LIBS%"

echo.
echo [Step 1/4] Looking for Revit installation...

set "REVIT_DLL="
set "REVTI_UI_DLL="

for %%V in (2024 2023 2022 2021 2020) do (
    if exist "C:\Program Files\Autodesk\Revit %%V\RevitAPI.dll" (
        echo Found Revit %%V
        set "REVIT_DLL=C:\Program Files\Autodesk\Revit %%V\RevitAPI.dll"
        set "REVIT_UI_DLL=C:\Program Files\Autodesk\Revit %%V\RevitAPIUI.dll"
        goto :found_revit
    )
)

if not exist "%REVIT_DLL%" (
    echo.
    echo WARNING: Revit not found in standard locations.
    echo Please copy these files manually:
    echo   RevitAPI.dll  -^> %LIBS%\RevitAPI.dll
    echo   RevitAPIUI.dll -^> %LIBS%\RevitAPIUI.dll
    echo.
    pause
    goto :check_dlls
)

:found_revit
echo Copying Revit DLLs to project...
copy /Y "%REVIT_DLL%" "%LIBS%\RevitAPI.dll" >nul
copy /Y "%REVIT_UI_DLL%" "%LIBS%\RevitAPIUI.dll" >nul
echo OK: DLLs copied.

:check_dlls
if not exist "%LIBS%\RevitAPI.dll" (
    echo ERROR: RevitAPI.dll still not found. Cannot continue.
    pause
    exit /b 1
)
if not exist "%LIBS%\RevitAPIUI.dll" (
    echo ERROR: RevitAPIUI.dll still not found. Cannot continue.
    pause
    exit /b 1
)

echo.
echo [Step 2/4] Checking Git installation...

git --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Git is not installed.
    echo Please download and install Git from:
    echo   https://git-scm.com/download/win
    echo.
    echo After installing Git, run this script again.
    pause
    exit /b 1
)

for /f "tokens=*" %%a in ('git --version') do set GITVER=%%a
echo OK: %GITVER%

echo.
echo [Step 3/4] Setting up Git repository...

cd /d "%PROJECT%"

if not exist "\.git" (
    git init
    echo Git repository initialized.
) else (
    echo Git repository already exists.
)

git add .
git commit -m "Add BIM Ninja Sheet Exporter with Revit API DLLs" --allow-empty

echo.
echo [Step 4/4] Pushing to GitHub...
echo.
set /p REPO="Enter your GitHub repo URL (https://github.com/USERNAME/REPO.git): "

git remote remove origin 2>nul
git remote add origin %REPO%

:: Determine branch
git show-ref --verify --quiet refs/heads/main
if errorlevel 1 (
    set BRANCH=master
) else (
    set BRANCH=main
)

echo.
echo Pushing to branch: %BRANCH%
echo You may need to enter your GitHub username and password/token.
echo.

git push -u origin %BRANCH% --force

if errorlevel 1 (
    echo.
    echo ========================================
    echo   PUSH FAILED
    echo ========================================
    echo.
    echo Common fixes:
    echo 1. Check your GitHub repo URL is correct
    echo 2. Use a GitHub Personal Access Token instead of password
    echo 3. Create token at: https://github.com/settings/tokens
    echo.
) else (
    echo.
    echo ========================================
    echo   SUCCESS! Files pushed to GitHub.
    echo ========================================
    echo.
    echo Go to GitHub Actions to see the build:
    echo https://github.com/mayassr1992-Mh/BimNinja-SheetExporter/actions
    echo.
    echo Wait 2-3 minutes for the build to complete.
    echo.
)

pause
endlocal
