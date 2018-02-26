namespace Homework.FrontEnd
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Root
    /// </summary>
    public class Program
    {
        /// <summary>
        /// First entry Point
        /// </summary>
        /// <param name="args">Args from cmdline</param>
        public static void Main(string[] args)
        {
            Telemetry.Info("Entered Into Main", "4863d534-1278-4762-807c-2f1d93bfb6a6");
            try
            {
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Telemetry.Info("Unexpected exception @ Main", "561ae17d-b549-4987-b461-aa91e395d0c3");
            }

            Telemetry.Info("Ended Main", "b3c0f245-d0c8-4161-9ef4-20a4d55adece");
        }

        /// <summary>
        /// Build the web host. 
        /// </summary>
        /// <param name="args">Args pulled from main</param>
        /// <returns>A Web Host.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseApplicationInsights()
                    .UseStartup<Startup>()
                    .Build();
    }
}
