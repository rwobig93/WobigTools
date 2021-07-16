using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WobigTools.API.Auth
{
    [Route("/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("google")]
        public async Task<ActionResult> Google()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
    }
}
