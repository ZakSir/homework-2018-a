using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Homework.ProblemA.TestExe
{
    class Program
    {
        static void Main(string[] args)
        {
            Homework.FrontEnd.Models.TestObjects to = new FrontEnd.Models.TestObjects();

            Indexed i = new Indexed(to.Bowser);

            foreach (var x in i.ToFlatDictionary())
            {
                Console.WriteLine($"[{x.Key}] = {x.Value}");
            }

            Console.ReadLine();

        }
    }
}
