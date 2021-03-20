using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using CoreLogicLib.Standard;

namespace WobigTools
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.InitializeLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
