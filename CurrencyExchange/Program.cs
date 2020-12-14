using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            //MainAsync().GetAwaiter().GetResult();
            var app = ServiceProvider.GetRequiredService<App>();
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(cfg =>
            {
                cfg.ClearProviders();
                cfg.AddSimpleConsole();
            });
            services.AddHttpClient();
            services.AddScoped<IDataService, DataService>();
            services.AddTransient<IGraphService, GraphService>();
            services.AddScoped<IForexRateService, ForexRateService>();
            services.AddSingleton<App>();
        }

        static async Task MainAsync()
        {

            var host = new HostBuilder()
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddDebug();


                })
                .ConfigureServices(services =>
                   {


                       services.AddHttpClient();
                       services.AddScoped<IDataService, DataService>();
                       services.AddTransient<IGraphService, GraphService>();
                       services.AddTransient<IForexRateService, ForexRateService>();
                       services.AddSingleton<App>();
                   })
                   .Build();
            Console.WriteLine("Starting host");
            await host.StartAsync();
            var app = host.Services.GetRequiredService<App>();
            app.Run(host);


        }


    }
}
