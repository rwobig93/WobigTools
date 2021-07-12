using System.ComponentModel.DataAnnotations;

namespace WobigTools.Models
{
    public class DisplaySettingsSMTPModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailFrom { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short.")]
        public string EmailName { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int Port { get; set; }
    }
}
