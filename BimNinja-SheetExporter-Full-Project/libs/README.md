# Revit API DLLs

This folder should contain the Revit API DLLs required to build the project.

## How to populate this folder

### Option 1: Auto-download (Recommended for CI/GitHub Actions)

Run the PowerShell script:
```powershell
powershell -File scripts/download-revit-api.ps1
```

This script will:
1. Check for a local Revit installation and copy DLLs from there
2. If not found locally, try to download from a known source
3. If all fails, print instructions

### Option 2: Manual Copy (for local development)

Copy these two files from your Revit installation folder:

```
C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll      -> libs/RevitAPI.dll
C:\Program Files\Autodesk\Revit 2024\RevitAPIUI.dll    -> libs/RevitAPIUI.dll
```

Replace `2024` with your Revit version (2022, 2023, or 2024).

### Option 3: For GitHub Actions

The GitHub Actions workflow automatically runs `download-revit-api.ps1` before building.
If the script cannot download the DLLs, the build will fail with a clear error message.

## Note

These DLLs are redistributable by Autodesk for addin development purposes.
If you commit them to your repository, they are typically ~10-15 MB total.
