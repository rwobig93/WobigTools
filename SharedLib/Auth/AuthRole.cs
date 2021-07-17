using Serilog;
using SharedLib.Dto;
using SharedLib.General;
using System.Collections.Generic;

namespace SharedLib.Auth
{
    public class AuthRole
    {
        public string Name { get; set; }
        public List<AuthAccess> Access { get; set; } = new List<AuthAccess>();
    }

    public static class AuthRoleExtensions
    {
        public static AuthRole CreateRole(this List<AuthRole> roles, string roleName, List<AuthAccess> accesses = null)
        {
            Log.Debug("Attempting to create the {AuthRoleName} role", roleName);
            if (roles.GetRole(roleName) == null)
            {
                Log.Debug("{AuthRoleName} role doesn't exist, creating", roleName);
                roles.Add(new AuthRole()
                {
                    Name = roleName,
                    Access = accesses
                });
                Log.Information("New role created: {AuthRoleName}", roleName);
                return roles.GetRole(roleName);
            }
            else
            {
                Log.Warning("An attempt was made to create an already existing role: {AuthRoleName}", roleName);
                return roles.GetRole(roleName);
            }
        }

        public static StatusReturn DeleteRole(this List<AuthRole> roles, string roleName)
        {
            Log.Debug("Attempting to delete the {AuthRoleName} role", roleName);
            if (roles.GetRole(roleName) != null)
            {
                Log.Warning("An attempt was made to delete a non existant role: {AuthRoleName}", roleName);
                return StatusReturn.NotFound;
            }
            else
            {
                var roleToDelete = roles.GetRole(roleName);
                Log.Information("{AuthRoleName} role exists, deleting", roleToDelete.Name);
                roles.Remove(roleToDelete);
                Log.Information("Role deleted");
                return StatusReturn.Success;
            }
        }

        public static AuthRole GetRole(this List<AuthRole> roles, string roleName)
        {
            Log.Debug("Attempting to get role {AuthRoleName}", roleName);
            if (roles.Find(x => x.Name == roleName) != null)
            {
                Log.Debug("Role {AuthRoleName} was found, returning");
                return roles.Find(x => x.Name == roleName);
            }
            else
            {
                Log.Debug("Role {AuthRoleName} doesn't exist", roleName);
                return null;
            }
        }

        public static void AddAccess(this string roleName, AuthAccess access)
        {
            Log.Debug("Attempting to add {AuthAccessName} access to the {AuthRoleName} role", access.Name, roleName);
            var desiredRole = Constants.SavedData.AccessRoles.Find(x => x.Name == roleName);
            if (desiredRole != null)
            {
                if (desiredRole.Access.Find(x => x.AccessID == access.AccessID) == null)
                {
                    Log.Debug("{AuthAccessName} access isn't in the {AuthRoleName} role, adding", access.Name, desiredRole.Name);
                    desiredRole.Access.Add(access);
                    Log.Information("Added {AuthAccessName} access to the {AuthRoleName} role", access.Name, desiredRole.Name);
                }
                else
                {
                    Log.Warning("An attempt was made to add {AuthAccessName} access on the {AuthRoleName} role when it already existed", access.Name, desiredRole.Name);
                }
            }
            else
            {
                Log.Warning("The {AuthRoleName} role doesn't exist, we can't add {AuthAccessName} access", roleName, access.Name);
            }
        }

        public static void RemoveAccess(this string roleName, AuthAccess access)
        {
            Log.Debug("Attempting to remove {AuthAccessName} access from the {AuthRoleName} role", access.Name, roleName);
            var desiredRole = Constants.SavedData.AccessRoles.GetRole(roleName);
            if (desiredRole != null)
            {
                if (desiredRole.Access.GetAccess(access.Name, access.AccessDomain) != null)
                {
                    Log.Warning("An attempt was made to remove {AuthAccessName} access from the {AuthRoleName} role when it doesn't exist", access.Name, desiredRole.Name);
                }
                else
                {
                    Log.Information("{AuthAccessName} access is in the {AuthRoleName} role, removing", access.Name, desiredRole.Name);
                    desiredRole.Access.Remove(access);
                    Log.Information("Removed access");
                }
            }
            else
            {
                Log.Warning("The {AuthRoleName} role doesn't exist, we can't remove {AuthAccessName} access", roleName, access.Name);
            }
        }
    }
}
