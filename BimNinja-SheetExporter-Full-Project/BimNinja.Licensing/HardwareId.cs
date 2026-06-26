using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace BimNinja.Licensing
{
    public static class HardwareId
    {
        public static string Generate()
        {
            try
            {
                string cpuId = GetWmiProperty("Win32_Processor", "ProcessorId");
                string motherboard = GetWmiProperty("Win32_BaseBoard", "SerialNumber");
                string disk = GetWmiProperty("Win32_DiskDrive", "SerialNumber");
                string combined = $"{cpuId}-{motherboard}-{disk}";
                using (var sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    return Convert.ToBase64String(hash).Substring(0, 16).ToUpper();
                }
            }
            catch
            {
                // Fallback
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(Environment.MachineName)).Substring(0, 16).ToUpper();
            }
        }

        private static string GetWmiProperty(string className, string property)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {className}"))
                using (var results = searcher.Get())
                {
                    foreach (ManagementObject obj in results)
                    {
                        var val = obj[property]?.ToString();
                        if (!string.IsNullOrEmpty(val)) return val;
                    }
                }
            }
            catch { }
            return "UNKNOWN";
        }
    }
}
