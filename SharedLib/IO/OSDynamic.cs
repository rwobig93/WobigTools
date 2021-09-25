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
            ProductAssembly proAss = GetProductAssembly();
            string basePath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), proAss.CompanyName), proAss.ProductName);
            }
            else
            {
                basePath = ".";
            }
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
                assy = Assembly.GetEntryAssembly(); // GetExecutingAssembly -or GetEntryAssembly
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

        public static Version GetRunningVersion(AssemblyType assemblyType = AssemblyType.Executing)
        {
            Version currentVersion = new Version("0.0.0.0");
            switch (assemblyType)
            {
                case AssemblyType.Entry:
                    currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                    break;
                case AssemblyType.Executing:
                    currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    break;
            }
            return currentVersion;
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
