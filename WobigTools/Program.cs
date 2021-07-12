using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using CoreLogicLib.Standard;
using SharedLib.Dto;
using WobigTools.Shared;

namespace WobigTools
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.InitializeApp();
            Core.InitializeLogger();
            if (Core.LoadAllFiles() != StatusReturn.Success)
            {
                Core.InitializeFirstRun();
            }
            Core.ProcessSettingsFromConfig();
            Core.StartServices();
            
            CreateHostBuilder(args).Build().Run();

            Core.CatchAppClose();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
