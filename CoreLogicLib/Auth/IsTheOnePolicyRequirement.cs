using Microsoft.AspNetCore.Authorization;

namespace CoreLogicLib.Auth
{
    public class IsTheOnePolicyRequirement : IAuthorizationRequirement
    {
        public string EmailAddress { get; }

        public IsTheOnePolicyRequirement(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
