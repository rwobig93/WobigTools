using System;

namespace DataAccessLib.Models
{
    public class WatcherAudit
    {
        public int WatcherAuditID { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now.ToLocalTime();
        public Guid TrackerID { get; set; }
        public string WatcherName { get; set; }
        public string ChangingUser { get; set; }
        public string State { get; set; }
    }
}
