using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Interop;

namespace BimNinja.SheetExporter.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SheetExportCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var license = App.LicenseManager;
                if (!license.IsValid())
                {
                    var trialDlg = new UI.TrialLicenseWindow(license);
                    trialDlg.ShowDialog();
                    if (!license.IsValid())
                    {
                        TaskDialog.Show("BIM Ninja", "License is not valid or trial has expired. Please activate a license to continue.");
                        return Result.Cancelled;
                    }
                }

                var doc = commandData.Application.ActiveUIDocument.Document;
                var window = new UI.SheetExporterWindow(doc);
                new WindowInteropHelper(window) { Owner = commandData.Application.MainWindowHandle };
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
