@echo off
REM Build script for BIM Ninja Sheet Exporter
REM Requires: Visual Studio 2022, .NET Framework 4.8, WiX Toolset v4 (optional), Inno Setup (optional)

echo ======================================
echo  BIM Ninja Sheet Exporter - Build
echo ======================================

set "SLN=%~dp0BimNinja.SheetExporter.sln"

REM Build C# projects
MSBuild.exe "%SLN%" /p:Configuration=Release /p:Platform="x64" /restore
if errorlevel 1 (
    echo MSBuild failed. Trying dotnet build...
    dotnet build "%SLN%" -c Release
)

echo.
echo Build complete. Check bin\Release folders.
pause