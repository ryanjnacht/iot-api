using System;
using iot_api.Scheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace iot_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Configuration.Configuration.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Startup failed: {ex.Message}");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { services.AddHostedService<SchedulerWorker>(); })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<WebApiStartup>(); });
        }
    }
}