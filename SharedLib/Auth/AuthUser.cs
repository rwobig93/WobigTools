using Serilog;
using SharedLib.Dto;
using SharedLib.General;
using System.Collections.Generic;

namespace SharedLib.Auth
{
    public class AuthUser
    {
        public string EmailAddress { get; set; }
        public List<AuthRole> AuthRoles { get; set; } = new List<AuthRole>() { new AuthRole( )};
    }

    public static class AuthUserExtensions
    {
        public static AuthUser CreateUserProfile(this List<AuthUser> userProfiles, string emailAddress)
        {
            var everyoneRole = Constants.SavedData.AccessRoles.GetRole("Everyone");
            if (userProfiles.GetUserProfile(emailAddress) == null)
            {
                Log.Debug("Creating user profile: {EmailAddress}", emailAddress);
                userProfiles.Add(new AuthUser
                {
                    EmailAddress = emailAddress,
                    AuthRoles = new List<AuthRole>() { everyoneRole }
                });
                Log.Information("Created user profile: {EmailAddress}", emailAddress);
                return userProfiles.GetUserProfile(emailAddress);
            }
            else
            {
                Log.Warning("A user creation was attempted when one already existed! Email: {EmailAddress}", emailAddress);
                return userProfiles.GetUserProfile(emailAddress);
            }
        }

        public static AuthUser GetUserProfile(this List<AuthUser> userProfiles, string emailAddress)
        {
            Log.Debug("Attempting to get user profile {AuthUserEmailAddress}", emailAddress);
            if (userProfiles.Find(x => x.EmailAddress == emailAddress) != null)
            {
                Log.Debug("User profile for {AuthUserEmailAddress} was found, returning");
                return userProfiles.Find(x => x.EmailAddress == emailAddress);
            }
            else
            {
                Log.Error("User profile for {AuthUserEmailAddress} doesn't exist", emailAddress);
                return null;
            }
        }
    }
}
