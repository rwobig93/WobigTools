using SharedLib.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WobigTools.Models
{
    public class DisplayTrackedProductModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "Friendly Name is too long.")]
        [MinLength(1, ErrorMessage = "Friendly Name is too short.")]
        public string FriendlyName { get; set; }
        public bool Triggered { get; set; }
        [Required]
        [Url]
        public string PageURL { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Keyword must be 3 characters or more")]
        public string Keyword { get; set; }
        public bool AlertOnKeywordNotExist { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public Guid AlertDestinationID { get; set; }
        public TrackInterval AlertInterval { get; set; }
        public AlertType AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
    }
}
