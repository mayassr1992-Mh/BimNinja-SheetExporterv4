# GitHub Verification Checklist for BIM Ninja Sheet Exporter

Use this checklist to verify everything is set up correctly on GitHub **before** your first build.

---

## Pre-Flight Checklist (Do These Before Pushing)

### 1. Revit API DLLs — CRITICAL

GitHub Actions runners do **not** have Revit installed. You **must** provide the DLLs.

**[ ] Option A: Copy DLLs to `libs/` folder (Recommended)**

```powershell
# On your machine with Revit installed, run:
Copy-Item "C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll"   "libs\RevitAPI.dll"
Copy-Item "C:\Program Files\Autodesk\Revit 2024\RevitAPIUI.dll" "libs\RevitAPIUI.dll"
```

Then commit them:
```bash
git add libs/*.dll
git commit -m "Add Revit API DLLs for CI build"
git push
```

**[ ] Option B: Use the download script**

The workflow runs `scripts/download-revit-api.ps1` automatically, but it may fail if:
- Your repo is private and the script can't download from public URLs
- The download URLs are unavailable

**Recommendation**: Use Option A. Commit the DLLs. They are ~10-15 MB total and Autodesk allows redistribution for addin development.

---

### 2. Verify Project Files Are in the Repo

**[ ] Check that these files exist in your GitHub repository:**

```
.github/workflows/build.yml              ← CI workflow
BimNinja.SheetExporter.sln               ← Solution file
BimNinja.SheetExporter.Addin/
  ├── BimNinja.SheetExporter.Addin.csproj
  ├── App.cs
  ├── Commands/SheetExportCommand.cs
  ├── Models/SheetInfo.cs
  ├── Models/ExportSettings.cs
  ├── Services/SheetExportService.cs
  ├── UI/SheetExporterWindow.xaml
  ├── UI/SheetExporterWindow.xaml.cs
  ├── UI/TrialLicenseWindow.xaml
  ├── UI/TrialLicenseWindow.xaml.cs
  ├── Properties/AssemblyInfo.cs
  ├── Resources/Icons/icon16.png
  ├── Resources/Icons/icon32.png
  └── BimNinja.SheetExporter.Addin.addin
BimNinja.Licensing/
  ├── BimNinja.Licensing.csproj
  ├── LicenseManager.cs
  ├── HardwareId.cs
  └── CryptoHelper.cs
BimNinja.Licensing.KeyGen/
  ├── BimNinja.Licensing.KeyGen.csproj
  └── Program.cs
BimNinja.Installer/
  ├── BimNinja.Installer.wixproj
  ├── Product.wxs
  ├── Installer.iss
  └── Icons/
      ├── appicon.ico
      ├── banner.bmp
      └── dialog.bmp
Directory.Build.props                    ← Revit path resolution
scripts/download-revit-api.ps1           ← DLL acquisition script
libs/
  ├── README.md
  ├── RevitAPI.dll        ← MUST be present for CI
  └── RevitAPIUI.dll      ← MUST be present for CI
```

**Quick check command** (run in your repo root):
```bash
# Check if all critical files exist
$files = @(
  ".github\workflows\build.yml",
  "BimNinja.SheetExporter.sln",
  "libs\RevitAPI.dll",
  "libs\RevitAPIUI.dll",
  "BimNinja.Installer\Icons\appicon.ico"
)
foreach ($f in $files) {
  if (Test-Path $f) { Write-Host "OK: $f" -ForegroundColor Green }
  else { Write-Host "MISSING: $f" -ForegroundColor Red }
}
```

---

### 3. Push to GitHub and Trigger the Build

**[ ] Push your code to GitHub**

```bash
git add .
git commit -m "Initial commit with full project"
git push origin main
```

**[ ] Go to GitHub and check Actions**

1. Open your repo on GitHub: `https://github.com/YOUR_USERNAME/YOUR_REPO`
2. Click the **Actions** tab
3. You should see a workflow run starting automatically

---

### 4. Watch the Build and Diagnose Failures

**[ ] If the build is green (all jobs passed):**

✅ Success! Go to step 5.

**[ ] If the build is red (any job failed):**

Click the failed job to see the error log. Common failures:

| Error | Cause | Fix |
|-------|-------|-----|
| `RevitAPI.dll not found` | DLLs not in `libs/` | Copy DLLs from your Revit install and commit them |
| `error MSB1009: Project file does not exist` | Wrong file paths | Check that all files are committed and paths match |
| `The WiX toolset build failed` | WiX SDK not restored | Check `nuget restore` step output |
| `Inno Setup is not installed` | Chocolatey install failed | Check the `choco install` step logs |
| `Upload artifact failed` | Nothing to upload | Check the build actually produced output files |

**[ ] To re-run a failed build:**

1. Go to the Actions tab
2. Click the failed workflow run
3. Click **Re-run jobs** (top right) → **Re-run all jobs**

Or make a small commit (e.g., edit README.md) and push again.

---

### 5. Download and Verify the Installer

**[ ] Download the artifacts**

1. Go to the **Actions** tab
2. Click the **successful** workflow run
3. Scroll to **Artifacts** at the bottom
4. Download:
   - `BimNinjaSheetExporter-Setup-EXE` ← This is your installer
   - `BimNinjaSheetExporter-Setup-MSI` ← Alternative (may be empty if WiX failed)

**[ ] Test the installer locally**

1. Extract the ZIP artifact
2. Run `BimNinjaSheetExporter_Setup.exe` on a machine with Revit
3. Verify:
   - The installer shows your branded dialog/banner
   - It installs to `C:\Program Files\BIM Ninja\Sheet Exporter\`
   - The `.addin` file is registered in Revit's addins folder
   - The **BIM Ninja** tab appears in Revit

---

## Quick Diagnostic Commands

Run these locally if the build fails and you want to debug before pushing again:

```powershell
# Test 1: Verify all source files compile
msbuild BimNinja.SheetExporter.sln /p:Configuration=Release /p:Platform=x64

# Test 2: Check if RevitAPI.dll is found
Test-Path "libs\RevitAPI.dll"
Test-Path "libs\RevitAPIUI.dll"

# Test 3: Test the download script
powershell -File scripts\download-revit-api.ps1

# Test 4: Verify Inno Setup syntax (if installed)
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /Q "BimNinja.Installer\Installer.iss"
```

---

## What to Do If the Build Keeps Failing

### Scenario A: RevitAPI.dll cannot be found in CI

**Solution 1**: Commit the DLLs to your repo (recommended)

```bash
git add libs/RevitAPI.dll libs/RevitAPIUI.dll
git commit -m "Add Revit API DLLs for CI"
git push
```

**Solution 2**: Use a private NuGet feed or Azure Artifacts

1. Upload RevitAPI.dll as a NuGet package to Azure Artifacts or GitHub Packages
2. Add a `nuget.config` to your repo pointing to the private feed
3. Use `<PackageReference>` instead of `<Reference>` in the `.csproj`

### Scenario B: WiX MSI build fails

WiX v4 is optional. If it keeps failing, just disable it:

Edit `.github/workflows/build.yml` and comment out the `build-installer-wix` job:
```yaml
  # build-installer-wix:
  #   runs-on: windows-latest
  #   needs: build-addin
  #   ...
```

The Inno Setup EXE installer is more reliable and easier to build.

### Scenario C: Inno Setup download fails

Replace the download URL in the workflow with a more reliable one:
```yaml
- name: Install Inno Setup
  run: |
    choco install innosetup --yes --no-progress
```

This uses Chocolatey (already installed on `windows-latest` runners) which is more reliable than direct downloads.

---

## Expected Build Timeline

| Step | Time | Status Indicator |
|------|------|-----------------|
| Checkout code | ~5s | ✅ |
| Setup MSBuild | ~5s | ✅ |
| Restore NuGet | ~15s | ✅ |
| Build solution | ~30-60s | ✅ |
| Install Inno Setup | ~30s | ✅ |
| Build installer | ~15s | ✅ |
| Upload artifacts | ~10s | ✅ |
| **Total** | **~2-3 minutes** | ✅ |

If it takes longer than 5 minutes, something is likely wrong. Check the logs.

---

## Final Checklist Before Publishing

**[ ] Build passes on GitHub Actions**
**[ ] Installer EXE is generated and downloadable**
**[ ] Installer installs successfully on a test machine**
**[ ] Revit shows the "BIM Ninja" tab**
**[ ] Sheet Exporter button opens the dialog**
**[ ] Trial dialog appears on first run**
**[ ] License key activation works**
**[ ] You can generate license keys with the KeyGen tool**

Once all of these are checked, you are ready to distribute your addin!

---

## Support

If the build still fails after following this checklist:

1. Go to the **Actions** tab on GitHub
2. Click the failed run
3. Expand the failed job (e.g., `build-addin`)
4. Expand the failed step (e.g., `Build Solution`)
5. Copy the error message
6. Share it with me and I will help you fix it
