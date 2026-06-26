using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using BimNinja.Licensing;
using System.Windows.Media.Imaging;
using System.IO;

namespace BimNinja.SheetExporter
{
    public class App : IExternalApplication
    {
        private static LicenseManager _licenseManager;
        public static LicenseManager LicenseManager => _licenseManager;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string licensePath = Path.Combine(appDataPath, "BIMNinja", "license.dat");
                _licenseManager = new LicenseManager(licensePath);

                string tabName = "BIM Ninja";
                try { application.CreateRibbonTab(tabName); } catch { }

                RibbonPanel panel = application.CreateRibbonPanel(tabName, "General");

                PushButtonData buttonData = new PushButtonData(
                    "SheetExporter",
                    "Sheet\nExporter",
                    typeof(SheetExportCommand).Assembly.Location,
                    typeof(SheetExportCommand).FullName)
                {
                    ToolTip = "Export sheets to PDF/DWG with custom naming and settings.",
                    LongDescription = "Select sheets, choose export format, set naming rules, and export in batch.",
                };

                PushButton button = panel.AddItem(buttonData) as PushButton;
                button.Image = LoadImage("icon16.png");
                button.LargeImage = LoadImage("icon32.png");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("BIM Ninja - Error", $"Failed to initialize: {ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private BitmapSource LoadImage(string resourceName)
        {
            try
            {
                var assembly = typeof(App).Assembly;
                string fullName = $"BimNinja.SheetExporter.Resources.Icons.{resourceName}";
                using (Stream stream = assembly.GetManifestResourceStream(fullName))
                {
                    if (stream == null) return null;
                    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return decoder.Frames[0];
                }
            }
            catch { return null; }
        }
    }
}
