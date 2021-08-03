using DataAccessLib.Models;
using System;

namespace CoreLogicLib.Standard
{
    public static class WobigToolsEvents
    {
        public static event EventHandler<WatcherEvent> WatcherEventOccured;
        public static event EventHandler<WatcherAudit> WatcherAuditOccured;

        public static void WatcherEventTrigger(object sender, WatcherEvent watcherEvent)
        {
            WatcherEventOccured?.Invoke(sender, watcherEvent);
        }

        public static void WatcherAuditTrigger(object sender, WatcherAudit watcherAudit)
        {
            WatcherAuditOccured?.Invoke(sender, watcherAudit);
        }
    }
}
