using System;
using AsyncArchitecture.Events.Handlers;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.TaskTracker.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AsyncArchitecture.TaskTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<TaskTrackerDbContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception exception)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "An error occurred while app initialization");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureServices(services => services
                    .AddHostedService(serviceProvider => new KafkaConsumerHandler(
                        serviceProvider,
                        "localhost:9092",
                        "user_stream")));
        }
    }
}