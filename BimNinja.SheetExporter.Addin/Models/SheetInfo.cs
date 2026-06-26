using Autodesk.Revit.DB;

namespace BimNinja.SheetExporter.Models
{
    public class SheetInfo
    {
        public ElementId Id { get; set; }
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public string FullName => $"{SheetNumber} - {SheetName}";
        public bool IsSelected { get; set; } = true;
        public bool IsPlaceholder { get; set; }
        public string Revision { get; set; }
        public string DrawnBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DesignedBy { get; set; }

        public static SheetInfo FromViewSheet(ViewSheet sheet)
        {
            var info = new SheetInfo
            {
                Id = sheet.Id,
                SheetNumber = sheet.SheetNumber,
                SheetName = sheet.Name,
                IsPlaceholder = sheet.IsPlaceholder
            };

            try { info.DrawnBy = sheet.GetParameters("Drawn By")?.FirstOrDefault()?.AsString() ?? ""; } catch { }
            try { info.CheckedBy = sheet.GetParameters("Checked By")?.FirstOrDefault()?.AsString() ?? ""; } catch { }
            try { info.ApprovedBy = sheet.GetParameters("Approved By")?.FirstOrDefault()?.AsString() ?? ""; } catch { }
            try { info.DesignedBy = sheet.GetParameters("Designed By")?.FirstOrDefault()?.AsString() ?? ""; } catch { }
            try { info.Revision = sheet.GetParameters("Current Revision")?.FirstOrDefault()?.AsString() ?? ""; } catch { }

            return info;
        }
    }
}
