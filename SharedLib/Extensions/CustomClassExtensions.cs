using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MatBlazor;
using Serilog;
using SharedLib.Dto;
using SharedLib.General;

namespace SharedLib.Extensions
{
    public static class CustomClassExtensions
    {
        public static void Save(this TrackedProduct tracker)
        {
            TrackedProduct foundTracker;

            Log.Debug("Adding or updating tracker on the {Interval} queue: {Tracker}", tracker.CheckInterval, tracker.FriendlyName);
            foundTracker = Constants.SavedData.TrackedProducts.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                foundTracker = tracker;
                Log.Debug("Updated tracker on the {Interval} queue: {Tracker}", tracker.CheckInterval, tracker.FriendlyName);
            }
            else
            {
                Constants.SavedData.TrackedProducts.Add(tracker);
                Log.Debug("Added tracker on the {Interval} queue: {Tracker}", tracker.CheckInterval, tracker.FriendlyName);
            }
        }

        public static void Delete(this TrackedProduct tracker)
        {
            TrackedProduct foundTracker;

            Log.Debug("Removing tracker on {Interval} queue: {Tracker}", tracker.CheckInterval, tracker.FriendlyName);
            foundTracker = Constants.SavedData.TrackedProducts.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                Log.Debug("Removed tracker on {Interval} queue: {Tracker}", tracker.CheckInterval, tracker.FriendlyName);
                Constants.SavedData.TrackedProducts.Remove(foundTracker);
            }
            else
            {
                Log.Warning("Attempted to remove tracker on {Interval} queue: {Tracker} | Couldn't find the tracker", tracker.CheckInterval, tracker.FriendlyName);
            }
        }

        public static void CustomToast(this IMatToaster toaster, string message, string title, MatToastType toastType, int duration = 5000, string icon = "")
        {
            if (icon != "")
            {
                toaster.Add(message, toastType, title, configure: config =>
                {
                    config.VisibleStateDuration = duration;
                });
            }
            else
            {
                toaster.Add(message, toastType, title, icon: icon, configure: config =>
                {
                    config.VisibleStateDuration = duration;
                });
            }
            Log.Information("Toast {ToastType}: {ToastTitle} - {ToastMessage}", toastType, title, message);
        }
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
