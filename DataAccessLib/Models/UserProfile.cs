using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DataAccessLib.Models
{
    public class UserProfile
    {
        public string EmailAddress { get; set; }
        public List<Claim> SecurityRoles { get; set; } = new List<Claim>() { new Claim(ClaimTypes.Role, "Everyone") };
    }
}
