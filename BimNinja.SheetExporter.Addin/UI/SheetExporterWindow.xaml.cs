using Autodesk.Revit.DB;
using BimNinja.SheetExporter.Models;
using BimNinja.SheetExporter.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BimNinja.SheetExporter.UI
{
    public partial class SheetExporterWindow : Window
    {
        private readonly Document _doc;
        private List<SheetInfo> _sheets;
        private readonly SheetExportService _service;

        public SheetExporterWindow(Document doc)
        {
            InitializeComponent();
            _doc = doc;
            _service = new SheetExportService(doc);
            NamingPattern.Text = "{SheetNumber} - {SheetName}";
            LoadSheets();
        }

        private void LoadSheets()
        {
            _sheets = _service.GetAllSheets();
            SheetsGrid.ItemsSource = _sheets;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadSheets();
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputPath.Text = dialog.SelectedPath;
            }
        }

        private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var s in _sheets) s.IsSelected = true;
            SheetsGrid.Items.Refresh();
        }

        private void DeselectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var s in _sheets) s.IsSelected = false;
            SheetsGrid.Items.Refresh();
        }

        private async void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = _sheets.Where(s => s.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one sheet to export.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(OutputPath.Text))
            {
                MessageBox.Show("Please select an output folder.", "No Output Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var settings = new ExportSettings
            {
                Format = (ExportFormat)FormatCombo.SelectedIndex,
                OutputFolder = OutputPath.Text,
                NamingPattern = NamingPattern.Text,
                CombineToSingleFile = CombineCheck.IsChecked ?? false,
                OpenFolderAfterExport = OpenFolderCheck.IsChecked ?? true,
                HideCropBoundaries = HideCropCheck.IsChecked ?? true,
                HideScopeBoxes = HideScopeCheck.IsChecked ?? true
            };

            ExportBtn.IsEnabled = false;
            try
            {
                var progress = new Progress<string>(msg => { });
                await System.Threading.Tasks.Task.Run(() => _service.ExportSheets(selected, settings, progress));

                MessageBox.Show($"Successfully exported {selected.Count} sheet(s).", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                if (settings.OpenFolderAfterExport && Directory.Exists(settings.OutputFolder))
                    Process.Start("explorer.exe", settings.OutputFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed:
{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ExportBtn.IsEnabled = true;
            }
        }
    }
}
