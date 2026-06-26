using System;
using System.Security.Cryptography;
using System.Text;

namespace BimNinja.Licensing.KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  BIM Ninja - License Key Generator");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("Enter customer Hardware ID (or press Enter to generate a test key):");
            string hardwareId = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(hardwareId))
            {
                hardwareId = "DEMO1234567890";
                Console.WriteLine($"Using demo Hardware ID: {hardwareId}");
            }

            string key = GenerateLicenseKey(hardwareId);
            Console.WriteLine();
            Console.WriteLine($"Generated License Key: {key}");
            Console.WriteLine();
            Console.WriteLine("Give this key to the customer. It will be validated on their machine.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static string GenerateLicenseKey(string hardwareId)
        {
            // Simple key generation: prefix + hardware hash + checksum
            // For production, use RSA signing or a server API
            string prefix = "BIMN";
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(hardwareId + "SecretSalt"));
                string hashStr = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
                string raw = prefix + "-" + hashStr;
                // Add checksum segment to make it pass ValidateLicenseKey
                int check = 0;
                foreach (char c in raw.Replace("-", "")) check += c;
                int checkDigit = (97 - (check % 97)) + 1;
                string checkStr = checkDigit.ToString("X2");
                return $"{raw}-{checkStr}";
            }
        }
    }
}
