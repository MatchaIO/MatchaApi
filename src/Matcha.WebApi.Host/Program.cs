using System;
using Microsoft.Owin.Hosting;

namespace Matcha.WebApi.Host
{
    class Program
    {
        /// <summary>
        /// http://patorjk.com/software/taag/#p=display&f=Slant&t=Matcha
        /// </summary>
        private const string SplashScreen = @"
    __  ___      __       __             ___    ____  ____
   /  |/  /___ _/ /______/ /_  ____ _   /   |  / __ \/  _/
  / /|_/ / __ `/ __/ ___/ __ \/ __ `/  / /| | / /_/ // /  
 / /  / / /_/ / /_/ /__/ / / / /_/ /  / ___ |/ ____// /   
/_/  /_/\__,_/\__/\___/_/ /_/\__,_/  /_/  |_/_/   /___/   
                                                          ";

        static void Main(string[] args)
        {

            try
            {
                Console.Title = "MATCHA - Web API";
                WriteWithColour(SplashScreen, ConsoleColor.DarkYellow);
                const string url = "http://localhost:51102";
                AppDomain.CurrentDomain.UnhandledException += (s, e) => WriteWithColour(e.ExceptionObject.ToString(), ConsoleColor.Red);
                using (WebApp.Start<Startup>(url))
                {
                    Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                WriteWithColour(ex.ToString(), ConsoleColor.Red);
                Console.ReadLine();
            }
        }

        private static void WriteWithColour(string text, ConsoleColor colour)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ForegroundColor = c;
        }
    }
}
