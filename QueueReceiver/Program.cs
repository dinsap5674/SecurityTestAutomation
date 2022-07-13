using BlobStorage.BlobService;
using BlobStorage.BlobService.Interfaces;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace QueueReceiver
{
    class Program
    {
        static async Task Main(/***string[] args**/)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    
                    configHost.AddEnvironmentVariables();
                })
                //This webjobs sdk method adds a appsetting.json configuration by default.
                .ConfigureWebJobs(webJobConfig =>
                {
                    webJobConfig.AddAzureStorageCoreServices();
                    webJobConfig.AddAzureStorage();
                    
                })

                //This configuration is set to have different environment configuration for production, development or stagging 
                .ConfigureAppConfiguration((hostContext, appConfig) =>
                {

                    appConfig.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    appConfig.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true, reloadOnChange: true);
                })

                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddSingleton<Functions, Functions>();
                    services.AddLogging();
                    services.AddTransient<IQueueMessageRepository, QueueMessageRepository>();
                    services.AddSingleton<IBlobService, BlobService>();

                })
                .ConfigureLogging((hostContext, logConfig) =>
                {
                    logConfig.ClearProviders();
                    //Defining the logging configuration setting explicitely to read the logger configuration fron appsetting.json file.
                    logConfig.AddConfiguration(hostContext.Configuration.GetSection("Logging"));

                    //Enabling filter in code works but it does not work in appsetting.json  
                    logConfig.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.RunAsync();
            }
        }

    }
}
