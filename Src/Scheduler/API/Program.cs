using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedCore.Configuration;

namespace Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                ServiceExtension.ConfigureLogging(host);

                host.Run();
            }
            catch (Exception ex){
                Log.Fatal(ex, "Runtime unhandled exception");
            }
            finally{
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5021", "https://localhost:5022");
                }).UseSerilog();
    }
}
