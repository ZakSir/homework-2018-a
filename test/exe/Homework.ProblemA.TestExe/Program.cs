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
            Guid g = Guid.NewGuid();

            Console.WriteLine($"Starting: {g.ToString()}");

            byte[] b = g.ToByteArray();

            Console.WriteLine($"As Bytes: {JsonConvert.SerializeObject(b)}");

            string basesixtyfour = Convert.ToBase64String(b);

            Console.WriteLine($"As Base64: {basesixtyfour}");

            Console.ReadLine();
        }
    }
}
      