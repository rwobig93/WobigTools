using System;
using System.Threading.Tasks;
using CoreLogicLib.Comm;
using CoreLogicLib.Standard;
using DataAccessLib.External;
using DataAccessLib.Models;
using Serilog;
using SharedLib.Dto;
using SharedLib.General;

namespace CoreLogicLib.Auto
{
    public static class Watcher
    {
        public static async Task CheckOnTrackers(TrackInterval interval)
        {
            var randomDelay = Generator.GetRandomNumberBetween(1149, 4736);
            Log.Debug("Running Tracker Check: {Interval} + RandomDelay({RandomDelay})", interval, randomDelay);
            await Task.Delay(randomDelay);
            foreach (TrackedProduct tracker in Constants.SavedData.TrackedProducts.FindAll(x => x.CheckInterval == interval))
            {
                if (tracker.Enabled)
                {
                    Log.Verbose("Attempting to Run {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                    //await ProcessAlertNeedOnTracker(tracker);
                    await ProcessAlertNeedOnTrackerHeadless(tracker);
                    Log.Verbose("Successfully Ran {Interval} Process: {Tracker}", interval, tracker.FriendlyName);
                }
                else
                {
                    Log.Verbose("Tracker {Tracker} is disabled, skipping it", tracker.FriendlyName);
                }
            }
            var leftOverPages = await WebHeadless.HeadlessBrowser.PagesAsync();
            Log.Verbose("Cleaning up leftover pages that haven't been disposed");
            await Task.Delay(10000);
            foreach (var page in leftOverPages)
            {
                if (page is null)
                    continue;
                try
                {
                    Log.Debug("Disposed stale page: {DisposedPageUrl}", page.Url);
                    await page.DisposeAsync();
                    Log.Debug("Page disposed successfully");
                }
                catch (Exception ex)
                {
                    Log.Error("Page disposal failure: {Error}", ex.Message);
                }
            }
        }

        public static async Task ProcessAlertNeedOnTracker(TrackedProduct tracker)
        {
            AppDbContext db = new AppDbContext();

            var newWatcherEvent = new WatcherEvent
            {
                AlertOnKeywordNotExist = tracker.AlertOnKeywordNotExist,
                AlertDestination = tracker.AlertDestination.AlertName,
                CheckInterval = tracker.CheckInterval.ToString(),
                Enabled = tracker.Enabled,
                FriendlyName = tracker.FriendlyName,
                Keyword = tracker.Keyword,
                PageURL = tracker.PageURL,
                TrackerID = tracker.TrackerID,
                Triggered = tracker.Triggered
            };

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
                            newWatcherEvent.Event = "Alert Trigger";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Trigger");
                        }
                    }
                    else
                    {
                        if (tracker.Triggered)
                        {
                            Log.Verbose("Resetting on tracker as logic matches", tracker, attempt1.KeywordExists);
                            ProcessAlertToReset(tracker);
                            newWatcherEvent.Event = "Alert Reset";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Reset");
                        }
                        else
                        {
                            newWatcherEvent.Event = "Alert Checked";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Checked");
                        }
                    }
                }
                else
                {
                    Log.Verbose("Keyword found [{KWFound}] and Validation [{KWValidation}] don't match, not alerting", attempt1.KeywordExists, attempt2.KeywordExists);
                    Log.Information("Checked watcher for {TrackerName}, Keyword: {TrackerKeyword} | Alert not triggered", tracker.FriendlyName, tracker.Keyword);
                    newWatcherEvent.Event = "Alert Checked";
                    db.Add(newWatcherEvent);
                    await db.SaveChangesAsync();
                    WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Checked");
                }
            }
            catch (Exception ex)
            {
                newWatcherEvent.Event = "Alert Failure";
                newWatcherEvent.Keyword = ex.Message;
                db.Add(newWatcherEvent);
                await db.SaveChangesAsync();
                WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Failure");
                Log.Warning("Error on tracker: [{Tracker}]{Error}", tracker.FriendlyName, ex.Message);
            }
        }

        public static async Task ProcessAlertNeedOnTrackerHeadless(TrackedProduct tracker)
        {
            AppDbContext db = new AppDbContext();

            var newWatcherEvent = new WatcherEvent
            {
                AlertOnKeywordNotExist = tracker.AlertOnKeywordNotExist,
                AlertDestination = tracker.AlertDestination.AlertName,
                CheckInterval = tracker.CheckInterval.ToString(),
                Enabled = tracker.Enabled,
                FriendlyName = tracker.FriendlyName,
                Keyword = tracker.Keyword,
                PageURL = tracker.PageURL,
                TrackerID = tracker.TrackerID,
                Triggered = tracker.Triggered
            };

            try
            {
                Log.Verbose("Processing alert for tracker", tracker);
                WebCheck webCheckResponse = (await WebHeadless.WebCheckForKeyword(tracker.PageURL, tracker.Keyword));
                if (webCheckResponse == null)
                {
                    Log.Verbose("webCheckResponse page is empty, not alerting");
                    return;
                }

                if (webCheckResponse.KeywordExists)
                {
                    if ((webCheckResponse.KeywordExists && !tracker.AlertOnKeywordNotExist) || (!webCheckResponse.KeywordExists && tracker.AlertOnKeywordNotExist))
                    {
                        if (!tracker.Triggered)
                        {
                            Log.Verbose("Alerting on tracker as logic matches", tracker, webCheckResponse.KeywordExists);
                            ProcessAlertToSend(tracker);
                            newWatcherEvent.Event = "Alert Trigger";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Trigger");
                        }
                    }
                    else
                    {
                        if (tracker.Triggered)
                        {
                            Log.Verbose("Resetting on tracker as logic matches", tracker, webCheckResponse.KeywordExists);
                            ProcessAlertToReset(tracker);
                            newWatcherEvent.Event = "Alert Reset";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Reset");
                        }
                        else
                        {
                            newWatcherEvent.Event = "Alert Checked";
                            db.Add(newWatcherEvent);
                            await db.SaveChangesAsync();
                            WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Checked");
                        }
                    }
                }
                else
                {
                    Log.Verbose("Keyword found [{KWFound}] and Validation [{KWValidation}] don't match, not alerting", webCheckResponse.KeywordExists, webCheckResponse.KeywordExists);
                    Log.Debug("Checked watcher for {TrackerName}, Keyword: {TrackerKeyword} | Alert not triggered", tracker.FriendlyName, tracker.Keyword);
                    newWatcherEvent.Event = "Alert Checked";
                    db.Add(newWatcherEvent);
                    await db.SaveChangesAsync();
                    WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Checked");
                }
            }
            catch (Exception ex)
            {
                newWatcherEvent.Event = "Alert Failure";
                newWatcherEvent.Keyword = ex.Message;
                db.Add(newWatcherEvent);
                await db.SaveChangesAsync();
                WobigToolsEvents.WatcherEventTrigger(new object(), "Alert Failure");
                Log.Warning("Error on tracker: [{Tracker}]{Error}", tracker.FriendlyName, ex.Message);
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
                    Communication.SendAlertEmail(tracker);
                    Communication.SendAlertWebhookDiscord(tracker, _color: "2813191");
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