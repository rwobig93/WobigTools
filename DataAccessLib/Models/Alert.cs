using System;
using System.Collections.Generic;

namespace DataAccessLib.Models
{
    public class Alert
    {
        public Guid AlertID { get; set; } = Guid.NewGuid();
        public AlertType AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
