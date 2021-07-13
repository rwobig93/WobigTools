﻿using System;
using System.Collections.Generic;

namespace SharedLib.Dto
{
    public class Alert
    {
        public Guid AlertID { get; set; } = Guid.NewGuid();
        public string AlertName { get; set; }
        public AlertType AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
