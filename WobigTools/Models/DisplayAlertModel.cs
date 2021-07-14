using SharedLib.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WobigTools.Models
{
    public class DisplayAlertModel
    {
        public Guid AlertID { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "Alert Name is too long.")]
        [MinLength(3, ErrorMessage = "Alert Name is too short.")]
        public string AlertName { get; set; }
        [Required]
        public int AlertType { get; set; } = -1;
        [Required]
        [EmailAddress]
        public string Emails { get; set; }
        [Required]
        [Url]
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
