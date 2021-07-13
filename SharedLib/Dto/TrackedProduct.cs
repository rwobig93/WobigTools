﻿using System;
using System.Collections.Generic;

namespace SharedLib.Dto
{
    public class TrackedProduct
    {
        public Guid TrackerID { get; } = Guid.NewGuid();
        public string FriendlyName { get; set; }
        public bool Triggered { get; set; } = false;
        public string PageURL { get; set; }
        public string Keyword { get; set; }
        public bool AlertOnKeywordNotExist { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public TrackInterval CheckInterval { get; set; }
        public Alert AlertDestination { get; set; }
        public AlertType AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
