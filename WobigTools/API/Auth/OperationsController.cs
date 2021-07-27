using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace WobigTools.API.Auth
{
    [Route("/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OperationsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("validate-defaults")]
        public async Task ValidateDefaultRolesAndUsers()
        {
            Log.Debug("Starting default role and user validation");
            
            var defaultRoles = new string[]
            {
                "Admin",
                "TestRole",
                "WatcherEvents",
                "WatcherList",
                "Developer"
            };

            var defaultUsers = new string[]
            {
                "superperson@wobigtools.com"
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

            foreach (var user in defaultUsers)
            {
                if (await _userManager.FindByNameAsync(user) == null)
                {
                    await _userManager.CreateAsync(new IdentityUser()
                    {
                        UserName = user,
                        Email = user,
                        EmailConfirmed = true
                    }, "superpassword");
                    Log.Information("Created missing default user: {EmailAddress}", user);
                }
                else
                {
                    Log.Debug("Default user already exists: {EmailAddress}", user);
                }
            }

            Log.Debug("Finished default role and user validation");
        }
    }
}
