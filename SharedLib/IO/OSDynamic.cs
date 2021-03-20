using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SharedLib.Dto;
using SharedLib.General;

namespace SharedLib.IO
{
    public static class OSDynamic
    {
        public static string GetStoragePath()
        {
            var userPath = Environment.GetEnvironmentVariable(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                "LOCALAPPDATA" : "Home");
            ProductAssembly proAss = GetProductAssembly();
            string basePath = Path.Combine(Path.Combine(userPath, proAss.CompanyName), proAss.ProductName);
            if (Constants.DebugMode)
                basePath = Path.Combine(basePath, "Test");
            else
                basePath = Path.Combine(basePath, "Prod");
            return basePath;
        }

        public static ProductAssembly GetProductAssembly(string assemblyPath = null)
        {
            Assembly assy;
            Version version;
            string companyName = "Wobigtech";
            string productName;
            bool isBeta;
            if (assemblyPath == null)
            {
                assy = Assembly.GetExecutingAssembly();
                companyName = assy.GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault().Company;
                productName = assy.GetCustomAttribute<AssemblyProductAttribute>().Product;
                version = assy.GetName().Version;
                isBeta = AppContext.BaseDirectory.ToLower().Contains("beta");
            }
            else
            {
                productName = new FileInfo(assemblyPath).Name.Replace(".exe", "");
                version = new Version(FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion);
                isBeta = assemblyPath.ToLower().Contains("beta");
            }
            return new ProductAssembly
            {
                CompanyName = string.IsNullOrWhiteSpace(companyName) ? "Unknown" : companyName,
                ProductName = string.IsNullOrWhiteSpace(productName) ? "Unknown" : productName,
                AppVersion = version ?? new Version("0.0.0.0"),
                IsBeta = isBeta
            };
        }

        public static Version GetRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static string GetConfigPath()
        {
            return Path.Combine(GetStoragePath(), "Config");
        }

        public static string GetLoggingPath()
        {
            return Path.Combine(GetStoragePath(), "Logs");
        }

        public static string GetSavedDataPath()
        {
            return Path.Combine(GetStoragePath(), "SavedData");
        }

        public static string GetFilePath(string directory, string fileName)
        {
            return Path.Combine(directory, fileName);
        }

        public static OSPlatform GetCurrentOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else
            {
                return OSPlatform.FreeBSD;
            }
        }
    }
}
