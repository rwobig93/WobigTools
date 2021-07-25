using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WobigTools.API.Auth
{
    [Route("/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public OperationsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet("validate-defaults")]
        public async Task ValidateDefaultRoles()
        {
            Log.Debug("Starting default role validation");
            
            var defaultRoles = new string[]
            {
                "Admin",
                "TestRole",
                "WatcherEvents",
                "WatcherList",
                "Developer"
            };

            foreach (var role in defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    Log.Information("Created missing default role: {RoleName}", role);
                }
                else
                {
                    Log.Debug("Default role already exists: {RoleName}", role);
                }
            }

            Log.Debug("Finished default role validation");
        }
    }
}
