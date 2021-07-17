using DataAccessLib.External;
using DataAccessLib.Queriables;
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

namespace WobigTools
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHostedService<LifetimeEventsHostedService>();
            services.AddSingleton<WeatherForecastService>();
            services.AddTransient<ISqlDA, MySqlDA>();
            services.AddTransient<IPeopleData, PeopleData>();
            services.AddMatBlazor();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 5000;
            });
            services.AddAuthentication("Cookies").AddCookie(opt =>
            {
                opt.Cookie.Name = "GoogleAuth";
                opt.LoginPath = "/oauth/google";
            }).AddGoogle(opt =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }

    public class CustomAuthenticationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthenticationMessageHandler(IAccessTokenProvider provider, NavigationManager nav) : base(provider, nav)
        {
            ConfigureHandler(new string[] { "https://auth.wobigtech.net/auth/" });
        }
    }

    internal class LifetimeEventsHostedService : IHostedService
    {
        public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnStarted()
        {
            Log.Information("App is now started");
        }

        private void OnStopping()
        {
            Log.Information("App is now stopping");
        }

        private void OnStopped()
        {
            Log.Information("App is now stopped");
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("App is now started async");
            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("App is now stopped async");
            return Task.CompletedTask;
        }
    }
}
