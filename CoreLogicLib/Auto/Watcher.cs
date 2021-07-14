using System;
using System.Collections.Generic;
using CoreLogicLib.Comm;
using Serilog;
using SharedLib.Dto;
using SharedLib.Extensions;
using SharedLib.General;

namespace CoreLogicLib.Auto
{
    public static class Watcher
    {
        public static void CheckOnTrackers(TrackInterval interval)
        {
            Log.Debug("Running Tracker Check: {Interval}", interval);
            foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts.FindAll(x => x.CheckInterval == interval))
            {
                if (tracker.Enabled)
                {
                    Log.Verbose("Attempting to Run {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                    ProcessAlertNeedOnTracker(tracker);
                    Log.Verbose("Successfully Ran {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                }
                else
                {
                    Log.Verbose("Tracker {Tracker} is disabled, skipping it", tracker.FriendlyName);
                }
            }
        }

        public static async void ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                WebCheck attempt1 = (await Web.WebCheckForKeyword(tracker.PageURL, tracker.Keyword));
                if (attempt1 == null)
                {
                    Log.Verbose("Attempt1 page is empty, not alerting");
                    return;
                }
                WebCheck attempt2 = (await Web.WebCheckForKeyword(tracker.PageURL, tracker.Keyword));
                if (attempt2 == null)
                {
                    Log.Verbose("Attempt2 page is empty, not alerting");
                    return;
                }

                if (attempt1.KeywordExists == attempt2.KeywordExists)
                {
                    if ((attempt1.KeywordExists && !tracker.AlertOnKeywordNotExist) || (!attempt1.KeywordExists && tracker.AlertOnKeywordNotExist))
                    {
                        if (!tracker.Triggered)
                        {
                            Log.Verbose("Alerting on tracker as logic matches", tracker, attempt1.KeywordExists);
                            ProcessAlertToSend(tracker);
                            Constants.WatcherEvents.AddMessage($"Alert Triggered: {tracker.FriendlyName} | Keyword: {tracker.Keyword}");
                        }
                    }
                    else
                    {
                        if (tracker.Triggered)
                        {
                            Log.Verbose("Resetting on tracker as logic matches", tracker, attempt1.KeywordExists);
                            ProcessAlertToReset(tracker);
                            Constants.WatcherEvents.AddMessage($"Alert Reset: {tracker.FriendlyName} | Keyword: {tracker.Keyword}");
                        }
                        else
                        {
                            Constants.WatcherEvents.AddMessage($"Alert Checked: {tracker.FriendlyName} | Keyword: {tracker.Keyword}");
                        }
                    }
                }
                else
                {
                    Log.Verbose("Keyword found [{KWFound}] and Validation [{KWValidation}] don't match, not alerting", attempt1.KeywordExists, attempt2.KeywordExists);
                    Log.Information("Checked watcher for {TrackerName}, Keyword: {TrackerKeyword} | Alert not triggered", tracker.FriendlyName, tracker.Keyword);
                    Constants.WatcherEvents.AddMessage($"Alert Checked: {tracker.FriendlyName} | Keyword: {tracker.Keyword}");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error on tracker: [{Tracker}]{Error}", tracker.FriendlyName, ex.Message);
            }
        }

        public static void CheckForUpdates()
        {
            try
            {
                // TO-DO: Write new update process for webservice
                Log.Debug("App update check success: {UpdateChecked}", true);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error occured checking for updates");
            }
        }

        public static void ProcessAlertToSend(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type", tracker.AlertDestination.AlertType);
            tracker.Triggered = true;
            switch (tracker.AlertDestination.AlertType)
            {
                case AlertType.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case AlertType.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker, _color: "2813191");
                    break;
                case AlertType.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }

        public static void ProcessAlertToReset(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type", tracker.AlertDestination.AlertType);
            tracker.Triggered = false;
            var msg = $"Alert has cleared for the following page:{Environment.NewLine}{tracker.PageURL}";
            var title = $"Alert has cleared for the {tracker.FriendlyName}, back to waiting :cry:";
            var color = "15730439";
            switch (tracker.AlertDestination.AlertType)
            {
                case AlertType.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case AlertType.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker, title, msg, color);
                    break;
                case AlertType.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }

        public static void ProcessAlertToTest(TrackedProduct tracker)
        {
            Log.Debug("Processing Alert Type For Testing", tracker.AlertDestination.AlertType);
            string title = $"Testing alert on the the {tracker.FriendlyName} tracker, Get Pumped!";
            string msg = $"Testing the tracker for the following page: {Environment.NewLine}{tracker.PageURL}";
            string color = "16445954";
            switch (tracker.AlertDestination.AlertType)
            {
                case AlertType.Email:
                    Communication.SendAlertEmail(tracker);
                    break;
                case AlertType.Webhook:
                    Communication.SendAlertWebhookDiscord(tracker, title, msg, color);
                    break;
                case AlertType.Email_Webhook:
                    Log.Warning("Processed Alert Type Webhook + Email when it isn't implemented yet", tracker);
                    break;
            }
        }
    }
}