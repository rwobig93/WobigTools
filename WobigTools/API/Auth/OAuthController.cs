using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WobigTools.API.Auth
{
    [Route("/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        [HttpGet("google")]
        public async Task<ActionResult> Google()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };
            await Task.CompletedTask;
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("logout")]
        public async Task Logout()
        {
            await Task.CompletedTask;
            HttpContext.Response.Cookies.Delete("GoogleAuth"); 
            Response.Cookies.Delete("GoogleAuth", new CookieOptions()
            {
                Secure = true
            });
        }
    }
}
