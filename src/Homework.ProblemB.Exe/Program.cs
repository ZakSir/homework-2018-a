using Homework.Models.Sports;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Homework.ProblemB.Exe
{
    class Program
    {
        private static readonly string[] validPropertyNames = new[] { "MASCOT", "CONFERENCE", "NAMES" };
        static void Main(string[] args)
        {
            Task mainAsyncTask = Task.Run(() => MainAsync(args));
            mainAsyncTask.Wait();
        }
        private static async Task MainAsync(string[] args)
        { 
            if (args.Length != 3 && args.Length != 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Incorrect number of arguments (expected: 2, actual: {args.Length})");
                Console.WriteLine();
                Console.WriteLine($"Please use syntax:");
                Console.WriteLine("\thwp2cli.exe [sport] [team] [property] [[baseUri]]");
                Console.WriteLine("baseUri is optional");
                Console.ResetColor();

                Environment.Exit(1);
            }

            string command = args[2].ToUpperInvariant();
            string team = args[1];
            string sport = args[0];

            if (!validPropertyNames.Contains(command))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"ERROR: Invalid Command. Please use: [ ");

                foreach (string validPropertyName in validPropertyNames)
                {
                    Console.Write($"{validPropertyName} ");
                }

                Console.WriteLine("]");
                Console.ResetColor();

                Environment.Exit(2);
            }

            string baseUri = args.Length == 4 ? args[3] : "https://hirezak-homework-2.azurewebsites.net/";

            HttpClient client = new HttpClient();

            string apiUri = baseUri + $"api/homework/sports/{sport}/{team}";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"api uri == '{apiUri}'");

            HttpResponseMessage response = await client.GetAsync(apiUri, HttpCompletionOption.ResponseContentRead);
            Console.WriteLine($"response code: {response.StatusCode}");

            string result = await response.Content.ReadAsStringAsync();

            Team teamObject = JsonConvert.DeserializeObject<Team>(result);

            switch(command)
            {
                case "MASCOT":
                    Console.Write($"The mascot for the {teamObject.Name} is ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(teamObject.Mascot);
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
                case "CONFERENCE":
                    Console.Write($"The {teamObject.Name} are in ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(teamObject.Conference);
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
                case "NAMES":
                    if(teamObject.ShortNames == null || teamObject.ShortNames.Count == 0)
                    {
                        Console.Write($"The '{teamObject.Name}' are not known by any other names.");
                        break;
                    }

                    Console.Write($"The '{teamObject.Name}' are also know by (and addressable by in the api: [");

                    int last = teamObject.ShortNames.Count - 1;
                    for(int i=0;i<teamObject.ShortNames.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(teamObject.ShortNames[i]);

                        if(i != last)
                        {
                            Console.ResetColor();
                            Console.Write(", ");
                        }
                    }

                    Console.ResetColor();
                    Console.Write("]");
                    break;
            }
        }
    }
}
