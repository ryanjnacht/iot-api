using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using iot_api.Scheduler;
using Microsoft.Extensions.DependencyInjection;

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