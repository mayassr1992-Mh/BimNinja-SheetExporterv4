namespace BimNinja.SheetExporter.Models
{
    public enum ExportFormat
    {
        PDF,
        DWG,
        DWF,
        IFC
    }

    public class ExportSettings
    {
        public ExportFormat Format { get; set; } = ExportFormat.PDF;
        public string OutputFolder { get; set; } = "";
        public string NamingPattern { get; set; } = "{SheetNumber} - {SheetName}";
        public bool CombineToSingleFile { get; set; } = false;
        public bool ExportViewports { get; set; } = true;
        public bool ExportTitleBlock { get; set; } = true;
        public bool OpenFolderAfterExport { get; set; } = true;
        public string PaperSize { get; set; } = "Auto";
        public string Orientation { get; set; } = "Auto";
        public bool ForceRaster { get; set; } = false;
        public bool HideCropBoundaries { get; set; } = true;
        public bool HideScopeBoxes { get; set; } = true;
        public bool HideReferencePlanes { get; set; } = true;
    }
}
