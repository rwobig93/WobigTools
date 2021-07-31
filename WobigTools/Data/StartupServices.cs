using DataAccessLib.External;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using WobigTools.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using CoreLogicLib.Auth;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WobigTools.Data
{
    public static class StartupServices
    {
        public static void ConfigureWobigTechAuth(this IServiceCollection services, IConfiguration Configuration)
        {
            // Authentication & Authorization
            services.AddDefaultIdentity<IdentityUser>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedAccount = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequiredLength = 12;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireDigit = true;
                opt.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(15);
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();
            services.AddTransient<UserManager<IdentityUser>>();
            services.AddTransient<RoleManager<IdentityRole>>();
            services.AddTransient<SignInManager<IdentityUser>>();
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddSingleton<IAuthorizationHandler, IsTheOnePolicyHandler>();
            services.AddAuthentication()
                .AddGoogle(opt =>
                {
                    opt.ClientId = Configuration["Google:ClientId"];
                    opt.ClientSecret = Configuration["Google:Secret"];
                    opt.Scope.Add("profile");
                    opt.Events.OnCreatingTicket = context =>
                    {
                        string picUri = context.User.GetProperty("picture").GetString();
                        context.Identity.AddClaim(new System.Security.Claims.Claim("picture", picUri));
                        return Task.CompletedTask;
                    };
                });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsTheOne", policy => policy.AddRequirements(new IsTheOnePolicyRequirement("rickwobig93@gmail.com")));
            });
        }
    }
}
