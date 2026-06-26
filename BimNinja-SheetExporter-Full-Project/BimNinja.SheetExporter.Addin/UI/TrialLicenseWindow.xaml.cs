using BimNinja.Licensing;
using System;
using System.Windows;

namespace BimNinja.SheetExporter.UI
{
    public partial class TrialLicenseWindow : Window
    {
        private readonly LicenseManager _licenseManager;

        public TrialLicenseWindow(LicenseManager licenseManager)
        {
            InitializeComponent();
            _licenseManager = licenseManager;
            HardwareIdBox.Text = licenseManager.GetHardwareId();
            UpdateTrialStatus();
        }

        private void UpdateTrialStatus()
        {
            int days = _licenseManager.DaysRemaining();
            if (_licenseManager.IsTrial())
            {
                if (days > 0)
                {
                    TrialStatusText.Text = $"Your trial expires in {days} day(s). You can activate a full license below.";
                    TrialStatusText.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    TrialStatusText.Text = "Your trial has expired. Please enter a license key to continue.";
                    TrialStatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                TrialStatusText.Text = "License is activated. Thank you for your purchase!";
                TrialStatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void CopyIdBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HardwareIdBox.Text);
            MessageBox.Show("Hardware ID copied to clipboard.", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartTrialBtn_Click(object sender, RoutedEventArgs e)
        {
            _licenseManager.StartTrial(14);
            UpdateTrialStatus();
            MessageBox.Show("Trial started! You have 14 days to evaluate the software.", "Trial Started", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void ActivateBtn_Click(object sender, RoutedEventArgs e)
        {
            string key = LicenseKeyBox.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Please enter a license key.", "No Key", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_licenseManager.ActivateLicense(key))
            {
                MessageBox.Show("License activated successfully!", "Activated", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid license key. Please check and try again.", "Invalid Key", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
