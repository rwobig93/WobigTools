using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SharedLib.General;
using System.IO;
using System.Linq;
using System.Text;
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
                "Developer",
                "Moderator"
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
                    }, "SuperPassword123!");
                    var newUser = await _userManager.FindByNameAsync(user);
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    await _userManager.ConfirmEmailAsync(newUser, confirmToken);
                    await _userManager.AddToRoleAsync(newUser, "Admin");
                    Log.Information("Created missing default user: UN[{UserName}] EM[{EmailAddress}]", newUser.UserName, newUser.Email);
                }
                else
                {
                    Log.Debug("Default user already exists: {EmailAddress}", user);
                }
            }

            Log.Debug("Finished default role and user validation");
        }

        //public static async Task<ActionResult> DownloadFileFromString(string contentToDownload)
        //{
        //    byte[] auditLogContent = Encoding.ASCII.GetBytes(contentToDownload);
        //    var stream = new MemoryStream(auditLogContent);
        //    var result = new FileStreamResult(stream, "text/plain");
        //    return File(stream, "application/force-download");
        //}
    }
}
