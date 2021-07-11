using System.Collections.Generic;
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

            Log.Debug("Adding or updating tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            foundTracker = Constants.SavedData.TrackedProducts.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                foundTracker = tracker;
                Log.Debug("Updated tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            }
            else
            {
                Constants.SavedData.TrackedProducts.Add(tracker);
                Log.Debug("Added tracker on the {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            }
        }

        public static void Delete(this TrackedProduct tracker)
        {
            TrackedProduct foundTracker;

            Log.Debug("Removing tracker on {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
            foundTracker = Constants.SavedData.TrackedProducts.Find(x => x.TrackerID == tracker.TrackerID);
            if (foundTracker != null)
            {
                Log.Debug("Removed tracker on {Interval} queue: {Tracker}", tracker.AlertInterval, tracker.FriendlyName);
                Constants.SavedData.TrackedProducts.Remove(foundTracker);
            }
            else
            {
                Log.Warning("Attempted to remove tracker on {Interval} queue: {Tracker} | Couldn't find the tracker", tracker.AlertInterval, tracker.FriendlyName);
            }
        }
    }
}
