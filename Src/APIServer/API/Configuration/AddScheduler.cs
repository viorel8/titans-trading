
using Hangfire;
// using Hangfire.SQLite;
using Hangfire.Storage.SQLite;
using SharedCore.Aplication.Services;
using SharedCore.Aplication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace APIServer.Configuration
{
    public static partial class ServiceExtension
    {

        public static IServiceCollection AddScheduler(
            this IServiceCollection serviceCollection,
            IConfiguration Configuration)
        {

            serviceCollection.AddHangfire((provider, configuration) =>
            {
                // This is alternative for PostgreSQL
                // configuration.UsePostgreSqlStorage(
                //     Configuration["ConnectionStrings:HangfireConnection"]);

                configuration.UseSQLiteStorage();

                configuration.UseFilter(new AutomaticRetryAttribute
                {
                    Attempts = 5
                });
            });

            serviceCollection.AddScoped<ICommandHandler, CommandHandler>();

            serviceCollection.AddScoped<IScheduler, Scheduler>();

            // serviceCollection.AddHangfireServer(options => {
            //     options.Queues = new[] { "systemqueue", "default" };
            //     options.WorkerCount = 2;
            // });

            return serviceCollection;
        }
    }
}