using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLib.Models
{
    public class WatcherEvent
    {
        public int WatcherEventID { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now.ToLocalTime();
        public string Event { get; set; }
        public Guid TrackerID { get; set; }
        public string FriendlyName { get; set; }
        public bool Triggered { get; set; }
        public string PageURL { get; set; }
        public string Keyword { get; set; }
        public bool AlertOnKeywordNotExist { get; set; }
        public bool Enabled { get; set; } = true;
        public string CheckInterval { get; set; }
        public string AlertDestination { get; set; }
    }
}
