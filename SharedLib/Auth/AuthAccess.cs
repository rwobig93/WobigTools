using Serilog;
using SharedLib.Dto;
using System;
using System.Collections.Generic;

namespace SharedLib.Auth
{
    public class AuthAccess
    {
        public Guid AccessID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string AccessDomain { get; set; }
        public AccessType AccessLevel { get; set; }
    }

    public static class AuthAccessExtensions
    {
        public static AuthAccess CreateAccess(this List<AuthAccess> accesses, string accessName, string accessDomain, AccessType accessType)
        {
            Log.Debug("Attempting to create the {AuthAccessName} Access on AccessDomain {AuthAccessDomain}", accessName, accessDomain);
            if (accesses.GetAccess(accessName, accessDomain) == null)
            {
                Log.Debug("Access {AuthAccessName} on AccessDomain {AuthAccessDomain} doesn't exist, creating", accessName, accessDomain);
                accesses.Add(new AuthAccess()
                {
                    Name = accessName,
                    AccessDomain = accessDomain,
                    AccessLevel = accessType
                });
                Log.Information("New Access created: Access {AuthAccessName} on AccessDomain {AuthAccessDomain} with AccessLevel {AuthAccessLevel}", accessName, accessDomain, accessType);
                return accesses.GetAccess(accessName, accessDomain);
            }
            else
            {
                Log.Warning("An attempt was made to create an already existing Access: {AuthAccessName} on AccessDomain {AuthAccessDomain}", accessName, accessDomain);
                return accesses.GetAccess(accessName, accessDomain);
            }
        }

        public static StatusReturn DeleteAccess(this List<AuthAccess> accesses, string accessName, string accessDomain)
        {
            Log.Debug("Attempting to delete the {AuthAccessName} Access on AccessDomain {AuthAccessDomain}", accessName, accessDomain);
            if (accesses.GetAccess(accessName, accessDomain) != null)
            {
                Log.Warning("An attempt was made to delete a non existant Access: {AuthAccessName} on AccessDomain {AuthAccessDomain}", accessName, accessDomain);
                return StatusReturn.NotFound;
            }
            else
            {
                var accessToRemove = accesses.GetAccess(accessName, accessDomain);
                Log.Information("Deleting Access {AuthAccessName} on AccessDomain {AuthAccessDomain}", accessToRemove.Name, accessToRemove.AccessDomain, accessToRemove.AccessLevel);
                accesses.Remove(accessToRemove);
                Log.Information("Access deleted");
                return StatusReturn.Success;
            }
        }

        public static AuthAccess GetAccess(this List<AuthAccess> accesses, string accessName, string accessDomain)
        {
            Log.Debug("Attempting to get Access {AuthAccessName} on AccessDomain {AuthAccessDomain}", accessName, accessDomain);
            if (accesses.Find(x => x.Name == accessName && x.AccessDomain == accessDomain) != null)
            {
                Log.Debug("Access {AuthAccessName}on AccessDomain {AuthAccessDomain} was found, returning");
                return accesses.Find(x => x.Name == accessName && x.AccessDomain == accessDomain);
            }
            else
            {
                Log.Debug("Access {AuthAccessName} on AccessDomain {AuthAccessDomain} doesn't exist");
                return null;
            }
        }
    }
}
