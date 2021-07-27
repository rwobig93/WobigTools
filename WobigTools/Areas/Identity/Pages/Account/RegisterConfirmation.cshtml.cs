using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using CoreLogicLib.Comm;
using MimeKit;

namespace WobigTools.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme);
            var emailTitle = "WobigTools - Email Confirmation";
            //var bodyBuilder = new BodyBuilder()
            //{
            //    HtmlBody = $"<img src=\"https://wobigtech.net/wp-content/uploads/2020/10/WobigIconnoBack-1-300x300-1.png\" alt=\"WobigTech\" /><br />Please confirm your email to use WobigTools by going to the <a href=\"{EmailConfirmationUrl}\">confirmation page</a>",
            //    TextBody = $"Please confirm your email to use WobigTools by going here: {EmailConfirmationUrl}"
            //};
            var emailBody = $"<img src=\"https://wobigtech.net/wp-content/uploads/2020/10/WobigIconnoBack-1-300x300-1.png\" alt=\"WobigTech\" /><br />Please confirm your email to use WobigTools by going to the <a href=\"{EmailConfirmationUrl}\">confirmation page</a>";
            await Communication.SendEmailAsync(emailTitle, emailBody, new string[] { email }, true);
            //// Once you add a real email sender, you should remove this code that lets you confirm the account
            //DisplayConfirmAccountLink = true;
            //if (DisplayConfirmAccountLink)
            //{
            //    var userId = await _userManager.GetUserIdAsync(user);
            //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //    EmailConfirmationUrl = Url.Page(
            //        "/Account/ConfirmEmail",
            //        pageHandler: null,
            //        values: new { area = "Identity", userId, code, returnUrl },
            //        protocol: Request.Scheme);
            //}

            return Page();
        }
    }
}
