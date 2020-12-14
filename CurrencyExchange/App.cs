using CurrencyExchange.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange
{
    public static class ConsoleWriter
    {
        private static object _MessageLock = new object();

        public static void WriteLine(string message)
        {
            lock (_MessageLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                //Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
    public class App
    {
        public static bool keepRunning = true;
        private readonly IForexRateService _forexRateService;
        private readonly IDataService _dataService;
        private readonly IGraphService _graphService;
        private readonly ILogger<App> _logger;

        public App(IForexRateService forexRateService, IDataService dataService, IGraphService graphService, ILogger<App> logger)
        {
            _forexRateService = forexRateService;
            _dataService = dataService;
            _graphService = graphService;
            _logger = logger;
        }
        public void Run()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                _logger.LogInformation("key pressed", e.SpecialKey);
                keepRunning = false;
                ConsoleWriter.WriteLine("Shutting down...");
                Environment.Exit(0);
            };
            while (keepRunning)
            {
                ConsoleWriter.WriteLine("Enter the date in format 'yyyy-mm-dd' For example 2020-12-12");
                var date = Console.ReadLine();
                if (!string.IsNullOrEmpty(date))
                {
                    _logger.LogInformation($"Date is {date}");

                    //// Retrieve forex rate from Bank of Canada API
                    var data = _dataService.GetForexRateFromBankOfCanada(date).GetAwaiter().GetResult();
                    if (data == null || data.Count == 0)
                    {
                        ConsoleWriter.WriteLine($"No forex rate available for the {date}\r\n");
                        ConsoleWriter.WriteLine("Please enter another date\r\n");
                        date = Console.ReadLine();
                        data = _dataService.GetForexRateFromBankOfCanada(date).GetAwaiter().GetResult();
                    }

                    // Generate adjaceny list from the data.Note: data has equations and values
                    var graph = _graphService.CreateGraph(data);
                    do
                    {
                        decimal product = 1;
                        ConsoleWriter.WriteLine("\r\n Enter two currencies to find forex rate separated by ','.For example 'AUD,CAD'.\r\n");
                        var input = Console.ReadLine();
                        if (!input.Contains(','))
                        {
                            Console.WriteLine("Incorrect format for input. Use format 'AUD,CAD'");
                            input = "";
                            input = Console.ReadLine();
                        }
                        var currencies = input.Trim().ToUpperInvariant().Split(',');
                        var start = currencies[0]; var end = currencies[1];
                        var rate = _forexRateService.TryFindForexRateUsingDijkstra(start, end, graph);
                        ConsoleWriter.WriteLine($"\r\n using dijkstra the forex rate between {start} and {end} is {rate}");
                        var success = _forexRateService.TryFindForexRateUsingRecursion(start, end, ref product, graph);
                        ConsoleWriter.WriteLine($"using recursion the forex rate between {start} and {end} is {product}\r\n");
                        ConsoleWriter.WriteLine("\r\nPress q to find forex rate on different dates\r\n");
                        ConsoleWriter.WriteLine("\r\nPress Enter or UP Arrow to continue\r\n");
                    }
                    while (Console.ReadKey() != new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false));




                }


            }


        }
        public async void Run(IHost host)
        {

            Console.CancelKeyPress += async (sender, e) =>
            {
                _logger.LogInformation("key pressed", e.SpecialKey);
                keepRunning = false;
                ConsoleWriter.WriteLine("Shutting down...");
                await host.StopAsync(new CancellationTokenSource(3000).Token);
                host.Dispose();
                Environment.Exit(0);
            };
            _logger.LogInformation("Starting the application");


            while (keepRunning)
            {
                ConsoleWriter.WriteLine("Enter the date in format 'yyyy-mm-dd' For example 2020-12-12");
                var date = Console.ReadLine();
                if (!string.IsNullOrEmpty(date))
                {
                    _logger.LogInformation($"Date is {date}");

                    //// Retrieve forex rate from Bank of Canada API
                    var data = await _dataService.GetForexRateFromBankOfCanada(date);

                    // Generate adjaceny list from the data.Note: data has equations and values
                    var graph = _graphService.CreateGraph(data);
                    while (Console.ReadKey() != new ConsoleKeyInfo('q', ConsoleKey.E, false, false, false))
                    {
                        decimal product = 1;
                        Console.WriteLine("\r\n Enter two currencies to find forex rate\r\n.");
                        var input = Console.ReadLine();
                        var currencies = input.Trim().Split(',');
                        var start = currencies[0]; var end = currencies[1];
                        var success = _forexRateService.TryFindForexRateUsingRecursion(start, end, ref product, graph);
                        if (success)
                        {
                            ConsoleWriter.WriteLine($"forex rate between {start} and {end} is {product}\r\n");
                        }
                        else
                        {
                            ConsoleWriter.WriteLine($"No rate found between {start} and {end}");
                        }
                        ConsoleWriter.WriteLine("\r\nPress q to find forex rate on different dates\r\n");
                    }



                }


            }



        }


    }
}