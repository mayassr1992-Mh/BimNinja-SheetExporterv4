using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BimNinja.Licensing
{
    public class LicenseManager
    {
        private readonly string _licenseFilePath;
        private LicenseData _licenseData;
        private readonly string _hardwareId;

        public LicenseManager(string licenseFilePath)
        {
            _licenseFilePath = licenseFilePath;
            _hardwareId = HardwareId.Generate();
            LoadLicense();
        }

        public bool IsValid()
        {
            if (_licenseData == null) return false;
            if (_licenseData.IsTrial)
            {
                return DateTime.UtcNow <= _licenseData.TrialEndDate;
            }
            return _licenseData.IsActivated && _licenseData.HardwareId == _hardwareId;
        }

        public bool IsTrial()
        {
            return _licenseData?.IsTrial ?? true;
        }

        public int DaysRemaining()
        {
            if (_licenseData == null) return 0;
            if (_licenseData.IsTrial)
            {
                var days = (_licenseData.TrialEndDate - DateTime.UtcNow).Days;
                return Math.Max(0, days);
            }
            return _licenseData.IsActivated ? 9999 : 0;
        }

        public string GetHardwareId() => _hardwareId;

        public void StartTrial(int days = 14)
        {
            _licenseData = new LicenseData
            {
                IsTrial = true,
                TrialStartDate = DateTime.UtcNow,
                TrialEndDate = DateTime.UtcNow.AddDays(days),
                HardwareId = _hardwareId
            };
            SaveLicense();
        }

        public bool ActivateLicense(string licenseKey)
        {
            if (ValidateLicenseKey(licenseKey))
            {
                _licenseData = new LicenseData
                {
                    IsTrial = false,
                    IsActivated = true,
                    LicenseKey = licenseKey,
                    HardwareId = _hardwareId,
                    ActivationDate = DateTime.UtcNow
                };
                SaveLicense();
                return true;
            }
            return false;
        }

        private bool ValidateLicenseKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length < 16) return false;
            string normalized = key.Replace("-", "").ToUpper();
            // Simple checksum validation; replace with server validation for production
            int hash = 0;
            foreach (char c in normalized) hash += c;
            return hash % 97 == 1; // Simple mod check as example
        }

        private void LoadLicense()
        {
            try
            {
                if (!File.Exists(_licenseFilePath)) return;
                string encrypted = File.ReadAllText(_licenseFilePath);
                string json = CryptoHelper.Decrypt(encrypted, _hardwareId);
                _licenseData = LicenseData.FromJson(json);
            }
            catch { _licenseData = null; }
        }

        private void SaveLicense()
        {
            try
            {
                string dir = Path.GetDirectoryName(_licenseFilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string json = _licenseData.ToJson();
                string encrypted = CryptoHelper.Encrypt(json, _hardwareId);
                File.WriteAllText(_licenseFilePath, encrypted);
            }
            catch { }
        }
    }

    [Serializable]
    public class LicenseData
    {
        public bool IsTrial { get; set; }
        public bool IsActivated { get; set; }
        public DateTime TrialStartDate { get; set; }
        public DateTime TrialEndDate { get; set; }
        public DateTime ActivationDate { get; set; }
        public string LicenseKey { get; set; }
        public string HardwareId { get; set; }

        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"IsTrial\":{IsTrial.ToString().ToLower()},");
            sb.Append($"\"IsActivated\":{IsActivated.ToString().ToLower()},");
            sb.Append($"\"TrialStartDate\":\"{TrialStartDate:O}\",");
            sb.Append($"\"TrialEndDate\":\"{TrialEndDate:O}\",");
            sb.Append($"\"ActivationDate\":\"{ActivationDate:O}\",");
            sb.Append($"\"LicenseKey\":\"{LicenseKey}\",");
            sb.Append($"\"HardwareId\":\"{HardwareId}\"");
            sb.Append("}");
            return sb.ToString();
        }

        public static LicenseData FromJson(string json)
        {
            var data = new LicenseData();
            try
            {
                var dict = ParseSimpleJson(json);
                if (dict.ContainsKey("IsTrial")) data.IsTrial = bool.Parse(dict["IsTrial"]);
                if (dict.ContainsKey("IsActivated")) data.IsActivated = bool.Parse(dict["IsActivated"]);
                if (dict.ContainsKey("TrialStartDate")) data.TrialStartDate = DateTime.Parse(dict["TrialStartDate"]);
                if (dict.ContainsKey("TrialEndDate")) data.TrialEndDate = DateTime.Parse(dict["TrialEndDate"]);
                if (dict.ContainsKey("ActivationDate")) data.ActivationDate = DateTime.Parse(dict["ActivationDate"]);
                if (dict.ContainsKey("LicenseKey")) data.LicenseKey = dict["LicenseKey"];
                if (dict.ContainsKey("HardwareId")) data.HardwareId = dict["HardwareId"];
            }
            catch { }
            return data;
        }

        private static System.Collections.Generic.Dictionary<string, string> ParseSimpleJson(string json)
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>();
            json = json.Trim('{', '}', ' ');
            var parts = json.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var kv = part.Split(new[] { ':' }, 2);
                if (kv.Length == 2)
                {
                    string key = kv[0].Trim().Trim('"');
                    string value = kv[1].Trim().Trim('"');
                    dict[key] = value;
                }
            }
            return dict;
        }
    }
}
