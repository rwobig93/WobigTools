﻿using System;
using CoreLogicLib.Standard;
using Hangfire;
using Hangfire.MySql;
using Microsoft.Extensions.Configuration;
using Serilog;
using SharedLib.Dto;
using SharedLib.Extensions;

namespace CoreLogicLib.Auto
{
    public static class Jobs
    {
        public static IConfiguration _config;
        public static BackgroundJobServer BackgroundJobServer { set; get; }
        public static StatusReturn InitializeJobService(IConfiguration config)
        {
            try
            {
                _config = config;

                if (BackgroundJobServer != null)
                {
                    Log.Warning("Background Job Server is already initialized and was attempted to be started");
                }
                else
                {
                    GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(_config.GetConnectionString("default"), new MySqlStorageOptions() { }));
                    BackgroundJobServer = new BackgroundJobServer();
                }
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Background Job Service Failed to Start");
                return StatusReturn.Failure;
            }
        }

        public static StatusReturn StopJobService()
        {
            try
            {
                Log.Debug("Attempting to stop the background job server");
                BackgroundJobServer.Dispose();
                Log.Information("Successfully stopped the background job server");
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to stop the Background Job Server");
                return StatusReturn.Failure;
            }
        }

        public static StatusReturn StartAllTimedJobs()
        {
            try
            {
                StartJobDataSaver();
                StartJobCleanup();
                StartJobCheckForUpdates();
                StartJobWatcherOneMin();
                StartJobWatcherFiveMin();
                return StatusReturn.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to start all timed jobs");
                return StatusReturn.Failure;
            }
        }

        private static void StartJobCheckForUpdates()
        {
            RecurringJob.AddOrUpdate("Update_Checker", () => Watcher.CheckForUpdates(), CronString.Hourly);
        }

        public static void StartJobWatcherFiveMin()
        {
            RecurringJob.AddOrUpdate("Standard_Tracker", () => Watcher.CheckOnTrackers(TrackInterval.FiveMin), CronString.MinuteInterval(5));
        }

        public static void StartJobWatcherOneMin()
        {
            RecurringJob.AddOrUpdate("Fast_Tracker", () => Watcher.CheckOnTrackers(TrackInterval.OneMin), Cron.Minutely);
        }

        public static void StartJobCleanup()
        {
            RecurringJob.AddOrUpdate("Cleanup", () => Core.CleanupAllOldFiles(), CronString.Daily);
        }

        public static void StartJobDataSaver()
        {
            RecurringJob.AddOrUpdate("DataSaver", () => Core.SaveEverything(), CronString.MinuteInterval(5));
        }
    }
}
