using Serilog;
using SharedLib.Auth;
using SharedLib.General;

namespace CoreLogicLib.Auth
{
    public static class Operations
    {
        public static void InitializeAuth()
        {
            EnsureDefaultRoles();
        }

        public static void ValidateUserProfile(string emailAddress)
        {
            Log.Debug("Starting user profile validation: {EmailAddress}", emailAddress);
            if (Constants.SavedData.UserProfiles.GetUserProfile(emailAddress) == null)
            {
                Constants.SavedData.UserProfiles.CreateUserProfile(emailAddress);
            }
            Log.Debug("Finished user profile validation: {EmailAddress}", emailAddress);
        }

        private static void EnsureDefaultRoles()
        {
            Log.Debug("Starting default role ensurance");
            var everyoneRole = Constants.SavedData.AccessRoles.GetRole("Everyone");
            var generalReadAccess = Constants.SavedData.Accesses.GetAccess("GeneralReadAccess", "Global");
            if (everyoneRole.Access.GetAccess(generalReadAccess.Name, generalReadAccess.AccessDomain) == null)
            {
                Log.Debug("The {AuthRoleName} role doesn't have {AuthAccessName} access, updating role", everyoneRole.Name, generalReadAccess.Name);
                everyoneRole.Access.Add(generalReadAccess);
                Log.Information("The {AuthRoleName} role didn't have {AuthAccessName} access, role updated", everyoneRole.Name, generalReadAccess.Name);
            }
            Log.Information("Default role ensurance finished");
        }
    }
}
