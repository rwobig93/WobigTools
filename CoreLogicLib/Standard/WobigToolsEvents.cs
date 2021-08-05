using DataAccessLib.Models;
using System;

namespace CoreLogicLib.Standard
{
    public static class WobigToolsEvents
    {
        public static event EventHandler<string> WatcherEventOccured;
        public static event EventHandler<string> WatcherAuditOccured;

        public static void WatcherEventTrigger(object sender = null, string watcherEvent = null)
        {
            WatcherEventOccured?.Invoke(sender, watcherEvent);
        }

        public static void WatcherAuditTrigger(object sender = null, string watcherAudit = null)
        {
            WatcherAuditOccured?.Invoke(sender, watcherAudit);
        }
    }
}
