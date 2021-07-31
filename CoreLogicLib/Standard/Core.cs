using System;
using Serilog;
using Serilog.Events;
using SharedLib.Dto;
using SharedLib.IO;
using SharedLib.General;
using SharedLib.Extensions;
using CoreLogicLib.Auto;
using Microsoft.Extensions.Configuration;
using CoreLogicLib.Auth;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace CoreLogicLib.Standard
{
    public static class Core
    {
        public static void InitializeLogger()
        {
            HouseKeeping.ValidateLoggingReqs();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(c => c.File(Path.Combine(Constants.PathLogs, OSDynamic.GetProductAssembly().ProductName, "_.log"), rollingInterval: RollingInterval.Day,
                  fileSizeLimitBytes: 10000000,
                  rollOnFileSizeLimit: true,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                  levelSwitch: Constants.LogLevelLocal))
                .WriteTo.Async(c => c.Seq("http://dev.wobigtech.net:5341", apiKey: Constants.LogUri, controlLevelSwitch: Constants.LogLevelCloud))
                .Enrich.WithCaller()
                .Enrich.WithThreadName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("Application", OSDynamic.GetProductAssembly().ProductName)
                .Enrich.WithProperty("SessionID", Guid.NewGuid())
                .Enrich.WithProperty("AppVersion", "0.1.0.0")
                .CreateLogger();

            ChangeLoggingLevelCloud();
            ChangeLoggingLevelLocal();

            Log.Information("==START-STOP== Application Started");

            if (Constants.DebugMode)
                Log.Information("LogUri is Public1");
            else
                Log.Information("LogUri is Public2");
        }

        public static void SetupAuth()
        {
            //Operations.InitializeAuth();
        }

        public static void SaveEverything()
        {
            Log.Verbose("Attempting to save all application data");
            Config.Save();
            SavedData.Save();
            MessageHandler.Save();
            Log.Debug("Saved all application data");
        }

        public static void CleanupAllOldFiles()
        {
            Log.Debug("Attempting to cleanup all old files");
            HouseKeeping.CleanupOldFiles(AppFile.Config);
            HouseKeeping.CleanupOldFiles(AppFile.Log);
            Log.Information("Cleaned up all old files");
        }

        internal static void BackupEverything()
        {
            Log.Debug("Attempting to backup everything");
            HouseKeeping.BackupFiles(AppFile.Config);
            HouseKeeping.BackupFiles(AppFile.SavedData);
            Log.Information("Successfully backed up everything");
        }

        public static void StartServices()
        {
            Log.Debug("Attempting to start all services");
            Jobs.InitializeJobService();
            Jobs.StartAllTimedJobs();
            //Watcher.StartWatcherThread();
            Log.Information("Finished Starting Services");
        }

        public static void ProcessSettingsFromConfig()
        {
            HouseKeeping.ValidateAllFilePaths();
            CleanupAllOldFiles();
            // TODO: Put setting changes from config load
        }

        public static void InitializeFirstRun()
        {
            Config.CreateNew();
            SavedData.CreateNew();
            if (Constants.CloseApp)
            {
                return;
            }
            Config.Save();
            SavedData.Save();
            Log.Information("Finished Initializing First App Run");
        }

        internal static bool CheckForUpdate()
        {
            throw new NotImplementedException();
        }

        internal static void ChangeLoggingLevelLocal(LogEventLevel logLevel = LogEventLevel.Information)
        {
            if (Constants.DebugMode)
                logLevel = LogEventLevel.Debug;

            if (Constants.LogLevelLocal == null)
            {
                Constants.LogLevelLocal = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level for local was null and had to be initialized");
            }
            else
            {
                Constants.LogLevelLocal.MinimumLevel = logLevel;
            }
            Log.Information("Local Logging Set to: {LogLevel}", logLevel);
        }

        internal static void ChangeLoggingLevelCloud(LogEventLevel logLevel = LogEventLevel.Warning)
        {
            if (Constants.DebugMode)
                logLevel = LogEventLevel.Debug;

            if (Constants.LogLevelCloud == null)
            {
                Constants.LogLevelCloud = new Serilog.Core.LoggingLevelSwitch(logLevel);
                Log.Warning("Logging Level for cloud was null and had to be initialized");
            }
            else
            {
                Constants.LogLevelCloud.MinimumLevel = logLevel;
            }
            Log.Information("Cloud Logging Set to: {LogLevel}", logLevel);
        }

        public static void InitializeWebTasks(string baseUrl)
        {
            HttpClient client = new HttpClient();
            client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/operations/validate-defaults"));
        }

        public static void InitializeApp()
        {
            HouseKeeping.ValidateRunningMode();
            HouseKeeping.ValidateAllFilePaths(true);
            HouseKeeping.ValidateLoggingReqs();
        }

        public static void CatchAppClose()
        {
            SaveEverything();
            Jobs.StopJobService();
            Comm.Web.Dispose();
            Log.Information("==START-STOP== Application Stopped");
            Log.CloseAndFlush();
        }

        public static StatusReturn LoadAllFiles()
        {
            StatusReturn confStatus = Config.Load();
            StatusReturn saveStatus = SavedData.Load();
            if (confStatus != StatusReturn.Success && saveStatus != StatusReturn.Success)
            {
                Log.Information("Neither a config or savedata file was found", confStatus, saveStatus);
                return StatusReturn.NotFound;
            }
            else if (confStatus != StatusReturn.Success && saveStatus == StatusReturn.Success)
            {
                Log.Information("A config file wasn't found but a savedata file was, generating config file", confStatus, saveStatus);
                Config.CreateNew();
                return StatusReturn.Success;
            }
            else if (saveStatus != StatusReturn.Success && confStatus == StatusReturn.Success)
            {
                Log.Information("A savedata file wasn't found but a config file was, generating savedata file", confStatus, saveStatus);
                SavedData.CreateNew();
                return StatusReturn.Success;
            }
            else
            {
                Log.Information("All files were found and loaded successfully", confStatus, saveStatus);
                return StatusReturn.Success;
            }
        }
    }
}
