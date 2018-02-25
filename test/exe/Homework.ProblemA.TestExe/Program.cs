using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Homework.ProblemA.TestExe
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            bool bool_a = false;
            bool bool_b = false;

            object obj_a = bool_a;
            object obj_b = bool_b;

            int* addr_bool_a = &bool_a;

            Console.WriteLine($"{nameof(bool_a)} @ {(&bool_a).ToString()} == {bool_a})
        }
    }
}
      