# How to Get Your Installer from GitHub Actions

Follow these steps exactly to download your compiled installer from GitHub.

---

## Step 1: Push Your Code to GitHub

Make sure you have:
1. Committed the Revit API DLLs (see `libs/README.md`)
2. Pushed all files to GitHub
3. Waited for the build to complete (check the orange dot next to the latest commit)

---

## Step 2: Open the Actions Tab

1. Go to your repository on GitHub:
   ```
   https://github.com/YOUR_USERNAME/YOUR_REPO_NAME
   ```

2. Click the **Actions** tab at the top of the page (next to "Pull requests", "Issues")

   ```
   Code   Issues   Pull requests   Actions   Projects   ...
                       ▲
                     CLICK HERE
   ```

---

## Step 3: Find the Workflow Run

You will see a list of workflow runs. Look for:
- **Workflow name**: "Build BIM Ninja Sheet Exporter"
- **Status**: A green checkmark ✅ (or orange dot if still running)

```
All workflows    Build BIM Ninja Sheet Exporter
                 ▼
    ✅ Build BIM Ninja Sheet Exporter — main
    "Initial commit with BIM Ninja Sheet Exporter"
    main · 2 minutes ago
```

Click on the **workflow run name** (the one with the green checkmark).

---

## Step 4: Click Into the Run Details

After clicking, you will see a page with three sections:

```
Build BIM Ninja Sheet Exporter
#1 — Initial commit with BIM Ninja Sheet Exporter

Jobs
─────────────────────────────────────────────────
    ✅ build-addin          — 1m 30s
    ✅ build-installer-inno — 45s
    ⚠️ build-installer-wix — 30s
    ✅ create-release       — 10s
─────────────────────────────────────────────────
    Artifacts  ▼
    (scroll down to see this)
```

Scroll down to the **Artifacts** section.

---

## Step 5: Download the Installer

Scroll down past the job list to find this section:

```
Artifacts
─────────────────────────────────────────────────
    Name                              Size
─────────────────────────────────────────────────
    BimNinja-SheetExporter-Build       50 KB
    BimNinjaSheetExporter-Setup-EXE  1.2 MB  ← DOWNLOAD THIS
    BimNinjaSheetExporter-Setup-MSI  (empty) ← Optional, may not exist
─────────────────────────────────────────────────
```

Click on the artifact name to download it:
- **Click `BimNinjaSheetExporter-Setup-EXE`** — this is your installer

The file will download as a **ZIP file** (e.g., `BimNinjaSheetExporter-Setup-EXE.zip`).

**Note:** GitHub always zips artifacts when you download them, even if the artifact is already a single file.

---

## Step 6: Extract and Use the Installer

1. Find the downloaded ZIP file (usually in your `Downloads` folder)
2. **Extract the ZIP** — you will get:
   ```
   BimNinjaSheetExporter_Setup.exe
   ```
3. Double-click the EXE to run the installer

---

## Troubleshooting

### "I don't see any artifacts"

This means the build failed. Scroll up and look for:
- A red ❌ next to a job name
- Click the failed job to see the error

Most common fix: **You forgot to commit `libs/RevitAPI.dll` and `libs/RevitAPIUI.dll`**

### "The artifact is empty / 0 bytes"

The installer build failed but the artifact step still ran. Check the `build-installer-inno` job logs for errors.

### "The Actions tab is empty / no workflows"

You need to push the `.github/workflows/build.yml` file. Check:
```bash
# Make sure the file exists in your repo
git ls-files .github/workflows/build.yml

# If not, add it
git add .github/workflows/build.yml
git commit -m "Add GitHub Actions workflow"
git push
```

### "I see the workflow but it says 'This workflow has no runs yet'"

Push a new commit to trigger it:
```bash
# Make a small change
echo "# Build triggered" >> README.md
git add README.md
git commit -m "Trigger build"
git push
```

Or click the **Run workflow** button on the Actions page (manual trigger).

---

## Visual Summary

```
GitHub Repo Page
    │
    ├──► Click "Actions" tab
    │         │
    │         ▼
    │    Actions Page
    │         │
    │         ├──► Find green ✅ run
    │         │         │
    │         │         ▼
    │         │    Click run name
    │         │         │
    │         │         ▼
    │         │    Run Details Page
    │         │         │
    │         │         ├──► Scroll down to "Artifacts"
    │         │         │         │
    │         │         │         ▼
    │         │         │    Click artifact name
    │         │         │         │
    │         │         │         ▼
    │         │         │    ZIP downloads
    │         │         │         │
    │         │         │         ▼
    │         │         │    Extract → Get EXE
    │         │         │         │
    │         │         │         ▼
    │         │         │    Run installer
    │         │         │
    │         │         ▼
    │         │    DONE!
```

---

## Alternative: Download from the Latest Release

If you have the `create-release` job enabled, you can also get the installer from the **Releases** tab:

1. Go to your repo → **Releases** tab (right side)
2. Find the latest release (e.g., "v1.0.0")
3. Click the release
4. Download `BimNinjaSheetExporter_Setup.exe` directly (no ZIP wrapper)

**Note:** This only works after the `create-release` job has run successfully on the `main` branch.
