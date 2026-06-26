# BIM Ninja Sheet Exporter — No-Build Deployment Guide

## Problem: You Can't Build Locally?

No Visual Studio? No MSBuild? No problem. You have **three options** to get a working installer without installing any build tools on your machine.

---

## Option 1: GitHub Actions (Recommended — Zero Local Tools)

**This is the easiest option.** GitHub will build everything for you in the cloud for free.

### Step 1: Create a GitHub Repository

1. Go to [github.com](https://github.com) and sign in (or create a free account)
2. Click **New Repository**
3. Name it: `BimNinja-SheetExporter` (or any name)
4. Make it **Public** or **Private** (both work with free GitHub Actions)
5. Click **Create Repository**

### Step 2: Upload This Project to GitHub

**Option A: Using GitHub Web Upload (No Git Needed)**

1. In your new GitHub repo, click **"uploading an existing file"**
2. Drag and drop **all** the files from this project folder (including subfolders)
3. Click **Commit changes**

**Option B: Using Git Command Line**

```bash
# Open Command Prompt in this project folder
cd "C:\path\to\BimNinja.SheetExporter"

git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/BimNinja-SheetExporter.git
git push -u origin main
```

### Step 3: Let GitHub Build It Automatically

1. Go to your repo on GitHub
2. Click **Actions** tab at the top
3. You will see the workflow running automatically
4. Wait ~5-10 minutes for it to complete
5. Click the completed workflow run
6. Scroll to **Artifacts** at the bottom
7. Download:
   - `BimNinjaSheetExporter-Setup-EXE` — the installer (recommended)
   - `BimNinjaSheetExporter-Setup-MSI` — the MSI alternative

### Step 4: Install on Your Machine

1. Extract the downloaded ZIP
2. Run `BimNinjaSheetExporter_Setup.exe` (or the `.msi`)
3. Follow the installer wizard
4. Restart Revit — the addin will appear in the **BIM Ninja** tab

---

## Option 2: Install Prerequisites & Build Locally

If you prefer to build on your own machine, install these free tools:

### Required Tools (All Free)

| Tool | Download Link | Purpose |
|------|---------------|---------|
| **Visual Studio 2022 Community** | [Download](https://visualstudio.microsoft.com/downloads/) | Build C# code |
| **.NET Framework 4.8 SDK** | Included with VS 2022 | Required runtime |
| **Inno Setup 6** | [Download](https://jrsoftware.org/isdl.php) | Build EXE installer |
| **WiX Toolset v4** (optional) | [Download](https://wixtoolset.org/) | Build MSI installer |

### Step-by-Step Local Build

1. **Install Visual Studio 2022 Community**
   - During installation, select **".NET desktop development"** workload
   - This includes MSBuild and .NET Framework 4.8

2. **Update Revit API References**
   - Open `BimNinja.SheetExporter.Addin\BimNinja.SheetExporter.Addin.csproj`
   - Update `HintPath` for `RevitAPI.dll` and `RevitAPIUI.dll` to match your Revit version:
     ```xml
     <HintPath>C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll</HintPath>
     ```

3. **Open Solution in Visual Studio**
   - Double-click `BimNinja.SheetExporter.sln`
   - Select **Release | x64** from the dropdown
   - Press **Ctrl+Shift+B** to build

4. **Build Installer**
   - Install Inno Setup
   - Open `BimNinja.Installer\Installer.iss` in Inno Setup Compiler
   - Press **F9** to compile
   - Output: `BimNinjaSheetExporter_Setup.exe`

---

## Option 3: Manual Installation (No Installer)

If you have the compiled DLLs (from GitHub Actions or a colleague), you can install manually:

### Step 1: Copy Files

Create this folder structure:

```
C:\Program Files\BIM Ninja\Sheet Exporter\
    ├── BimNinja.SheetExporter.Addin.dll
    ├── BimNinja.Licensing.dll
    └── BimNinja.SheetExporter.Addin.addin
```

### Step 2: Register with Revit

Create a text file at:

```
C:\ProgramData\Autodesk\Revit\Addins\2024\BimNinja.SheetExporter.addin
```

Paste this content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>BIM Ninja Sheet Exporter</Name>
    <Assembly>C:\Program Files\BIM Ninja\Sheet Exporter\BimNinja.SheetExporter.Addin.dll</Assembly>
    <FullClassName>BimNinja.SheetExporter.App</FullClassName>
    <VendorId>BIMNINJA</VendorId>
    <VendorDescription>BIM Ninja - Professional Revit Tools</VendorDescription>
  </AddIn>
</RevitAddIns>
```

Repeat for Revit 2023 and 2022 by changing the folder path.

### Step 3: Restart Revit

The **BIM Ninja** tab will appear with the **Sheet Exporter** button.

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "RevitAPI.dll not found" | Update the HintPath in `.csproj` to your Revit installation folder |
| "Build failed with MSB1009" | Make sure you selected **x64** platform, not Any CPU |
| "Addin not showing in Revit" | Check the `.addin` file path matches the actual DLL location |
| "License dialog keeps appearing" | Start the trial or enter a valid license key |
| "WiX build fails" | Use Inno Setup instead — it's simpler and more reliable |

---

## Need Help?

If you get stuck:
1. Check the **GitHub Actions** tab in your repo for build error logs
2. Share the error message and I can help fix it
3. The `README.md` in the project root has more detailed technical documentation
