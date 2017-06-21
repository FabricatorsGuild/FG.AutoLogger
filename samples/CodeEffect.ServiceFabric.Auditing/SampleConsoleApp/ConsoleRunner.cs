using System;
using System.Diagnostics;
using System.Net.Http;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Samples
{
        public class ConsoleRunner
        {
            private readonly IConsoleRunnerLogger _logger;
            public Process Process { get; set; }
            public string Name { get; set; }

            public ConsoleRunner(IConsoleRunnerLogger logger)
            {
                _logger = logger;
                _logger.RunnerCreated();
            }

            public void Run()
            {
                System.Console.WriteLine("Console Runner running. Press a key to start loop.");
                System.Console.ReadKey();

                var key = ConsoleKey.NoName;
                while (key != ConsoleKey.X)
                {
                    _logger.StartLoop();
                    try
                    {
                        _logger.WaitingForKeyPress();
                        key = System.Console.ReadKey().Key;

                        if (key == ConsoleKey.E)
                        {
                            throw new NotSupportedException($"Pressing the key {key} is not supported. Please don't do that.");
                        }
                        else if (key == ConsoleKey.A)
                        {
                            var httpClient = new HttpClient();
                            var result = httpClient.GetStringAsync("http://www.google.se").GetAwaiter().GetResult();
                        }
                        _logger.KeyPressed(key);
                    }
                    catch (Exception ex)
                    {
                        _logger.UnsupportedKeyError(ex);
                    }
                    _logger.StopLoop();
                }
            }

            ~ConsoleRunner()
            {
                _logger.RunnerDestroyed();
            }
        }
}