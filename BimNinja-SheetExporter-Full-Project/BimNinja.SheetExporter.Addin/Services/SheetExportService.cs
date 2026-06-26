using Autodesk.Revit.DB;
using BimNinja.SheetExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BimNinja.SheetExporter.Services
{
    public class SheetExportService
    {
        private readonly Document _doc;

        public SheetExportService(Document doc)
        {
            _doc = doc;
        }

        public List<SheetInfo> GetAllSheets()
        {
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Where(s => !s.IsPlaceholder)
                .Select(s => SheetInfo.FromViewSheet(s))
                .OrderBy(s => s.SheetNumber)
                .ToList();
            return collector;
        }

        public void ExportSheets(List<SheetInfo> sheets, ExportSettings settings, IProgress<string> progress)
        {
            if (string.IsNullOrWhiteSpace(settings.OutputFolder))
                throw new InvalidOperationException("Output folder is not set.");
            if (!Directory.Exists(settings.OutputFolder))
                Directory.CreateDirectory(settings.OutputFolder);

            switch (settings.Format)
            {
                case ExportFormat.PDF:
                    ExportToPdf(sheets, settings, progress);
                    break;
                case ExportFormat.DWG:
                    ExportToDwg(sheets, settings, progress);
                    break;
                case ExportFormat.DWF:
                    ExportToDwf(sheets, settings, progress);
                    break;
                default:
                    throw new NotSupportedException($"Format {settings.Format} is not yet supported.");
            }
        }

        private void ExportToPdf(List<SheetInfo> sheets, ExportSettings settings, IProgress<string> progress)
        {
            if (sheets.Count == 0) return;

            var pdfSettings = new PDFExportOptions
            {
                HideCropBoundaries = settings.HideCropBoundaries,
                HideReferencePlanes = settings.HideReferencePlanes,
                HideScopeBoxes = settings.HideScopeBoxes,
                ColorDepth = ColorDepthType.Color,
                RasterQuality = settings.ForceRaster ? RasterQualityType.High : RasterQualityType.Medium,
                PaperFormat = ExportPaperFormat.Default,
                Orientation = PageOrientationType.Default,
                Combine = settings.CombineToSingleFile
            };

            if (settings.CombineToSingleFile)
            {
                string fileName = SanitizeFileName($"{_doc.Title} - Sheets");
                string path = Path.Combine(settings.OutputFolder, fileName + ".pdf");
                var ids = sheets.Select(s => s.Id).ToList();
                progress?.Report($"Exporting {sheets.Count} sheets to single PDF...");
                _doc.Export(ids, path, pdfSettings);
            }
            else
            {
                foreach (var sheet in sheets)
                {
                    string fileName = ApplyNamingPattern(sheet, settings.NamingPattern);
                    string path = Path.Combine(settings.OutputFolder, fileName + ".pdf");
                    progress?.Report($"Exporting: {sheet.FullName}");
                    _doc.Export(new List<ElementId> { sheet.Id }, path, pdfSettings);
                }
            }
        }

        private void ExportToDwg(List<SheetInfo> sheets, ExportSettings settings, IProgress<string> progress)
        {
            var dwgOpts = new DWGExportOptions
            {
                ExportOfSolids = SolidType.Polymesh,
                FileVersion = ACADVersion.R2018,
                TargetUnit = ExportUnit.Millimeter,
                LineScaling = LineScaling.WideLinetypeScaling
            };

            foreach (var sheet in sheets)
            {
                string fileName = ApplyNamingPattern(sheet, settings.NamingPattern);
                string path = Path.Combine(settings.OutputFolder, fileName + ".dwg");
                progress?.Report($"Exporting DWG: {sheet.FullName}");
                _doc.Export(settings.OutputFolder, fileName, new List<ElementId> { sheet.Id }, dwgOpts);
            }
        }

        private void ExportToDwf(List<SheetInfo> sheets, ExportSettings settings, IProgress<string> progress)
        {
            var dwfOpts = new DWFExportOptions
            {
                ExportingAreas = false,
                ImageFormat = ImageFormat.Lossless,
                ImageQuality = ImageQuality.Medium
            };

            foreach (var sheet in sheets)
            {
                string fileName = ApplyNamingPattern(sheet, settings.NamingPattern);
                progress?.Report($"Exporting DWF: {sheet.FullName}");
                _doc.Export(settings.OutputFolder, fileName, new List<ElementId> { sheet.Id }, dwfOpts);
            }
        }

        public static string ApplyNamingPattern(SheetInfo sheet, string pattern)
        {
            string result = pattern
                .Replace("{SheetNumber}", sheet.SheetNumber)
                .Replace("{SheetName}", sheet.SheetName)
                .Replace("{Revision}", sheet.Revision)
                .Replace("{DrawnBy}", sheet.DrawnBy)
                .Replace("{CheckedBy}", sheet.CheckedBy)
                .Replace("{ApprovedBy}", sheet.ApprovedBy)
                .Replace("{DesignedBy}", sheet.DesignedBy);
            return SanitizeFileName(result);
        }

        public static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }
    }
}
