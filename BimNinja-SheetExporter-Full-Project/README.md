# BIM Ninja - Sheet Exporter

Professional Revit addin for batch exporting sheets to PDF, DWG, and DWF with custom naming conventions, licensing, and trial support.

## Features

- **Export Formats**: PDF, DWG, DWF
- **Batch Export**: Select multiple sheets with checkboxes
- **Custom Naming**: Use patterns like `{SheetNumber} - {SheetName}`
- **Combine Mode**: Export all sheets to a single PDF
- **Licensing**: Built-in 14-day trial and hardware-locked license key activation
- **Installer**: MSI (WiX) and EXE (Inno Setup) options

## Requirements

- Autodesk Revit 2022, 2023, or 2024
- .NET Framework 4.8
- Visual Studio 2022 (for building)

## Project Structure

```
BimNinja.SheetExporter.sln
├── BimNinja.SheetExporter.Addin/    Main Revit addin (C#, WPF UI)
├── BimNinja.Licensing/                Licensing & trial library
├── BimNinja.Installer/                Installer projects
│   ├── BimNinja.Installer.wixproj     WiX v4 MSI project
│   └── Installer.iss                  Inno Setup EXE project
└── Build.bat                          Build script
```

## Building from Source

### 1. Update Revit References

Open `BimNinja.SheetExporter.Addin.csproj` and update the `HintPath` for `RevitAPI.dll` and `RevitAPIUI.dll` to match your Revit installation path:

```xml
<Reference Include="RevitAPI">
  <HintPath>C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll</HintPath>
</Reference>
```

### 2. Build Solution

Open the solution in Visual Studio 2022 and build in **Release | x64**.

Or use the command line:
```cmd
MSBuild.exe BimNinja.SheetExporter.sln /p:Configuration=Release /p:Platform="x64" /restore
```

### 3. Generate Icons (Optional)

Run the Python script to generate placeholder icons:
```cmd
python generate_icons.py
```
Replace generated icons with your branded icons in `BimNinja.Installer/Icons/`.

### 4. Build Installer

#### Option A: Inno Setup (Recommended — Easiest)
1. Install [Inno Setup](https://jrsoftware.org/isinfo.php)
2. Open `BimNinja.Installer/Installer.iss` in Inno Setup Compiler
3. Compile (F9)
4. Output: `BimNinjaSheetExporter_Setup.exe`

#### Option B: WiX v4 MSI
1. Install [WiX Toolset v4](https://wixtoolset.org/)
2. Run:
```cmd
cd BimNinja.Installer
wix build -p:Configuration=Release -p:Platform=x64
```
3. Output: `BimNinjaSheetExporter.msi`

## Licensing & Trial System

### How It Works

1. **First Run**: User sees trial/license dialog
2. **Trial**: Click "Start Trial" → 14-day evaluation begins
3. **Activation**: User provides Hardware ID, you generate a license key, they enter it
4. **Hardware Lock**: License is tied to CPU + Motherboard + Disk serial numbers

### Generating License Keys

Use the included `LicenseKeyGenerator` console app or script to generate keys for your customers.

```bash
dotnet run --project BimNinja.Licensing.KeyGen
```

Enter the customer's Hardware ID to get a valid license key.

### License Key Format

Keys are 16+ character strings with a checksum. Example generation logic:
- The `LicenseManager.ValidateLicenseKey()` method uses a simple checksum.
- **For production**: Replace with server-side validation or a stronger algorithm.

## Customizing the Export Logic

The core export logic is in `Services/SheetExportService.cs`. Modify the `ExportToPdf`, `ExportToDwg`, or `ExportToDwf` methods to match your PyRevit script's behavior.

Common customizations:
- Custom paper sizes: Set `PDFExportOptions.PaperFormat`
- Custom print settings: Use `PrintManager` API
- Naming patterns: Add more placeholders in `ApplyNamingPattern()`
- Additional formats: Implement `ExportToIfc()` or others

## Revit Version Support

The addin supports Revit 2022, 2023, and 2024. To add more versions, update the registry entries in the installer and add new `RevitAPI` reference paths in the `.csproj` file (or use a multi-targeting build).

## Distribution

1. Build the installer (EXE or MSI)
2. Sign the installer with your code signing certificate (recommended)
3. Distribute to customers
4. Collect Hardware IDs and issue license keys

## Security Notes

- The built-in license validation is **client-side only** and uses a simple checksum.
- For stronger protection, implement:
  - Online license server with API calls
  - Obfuscation (e.g., Dotfuscator, ConfuserEx)
  - Signed license files with RSA public/private key pairs

## Support

For issues or customization requests, contact BIM Ninja support.
