using System;
using System.Linq;

namespace Homework.ProblemA.TestExe
{
    class Program
    {
        static void Main(string[] args)
        {
            ModelService ms = new ModelService();

            TestClassFoo foo = new TestClassFoo()
            {
                Foo = "I am foo"
            };

            TestClassBar bar = new TestClassBar()
            {
                Bar = $"I am {nameof(TestClassBar.Bar)}",
                Foo = 3229
            };

            TestClassFooBar fooBar = new TestClassFooBar()
            {
                Bar = $"I am {nameof(TestClassFooBar.Bar)}",
                Foo = "I am foo an I am also a string"
            };

            ObjectDifferential foo_bardiff = ms.GetDifferential(foo, bar);
            ObjectDifferential foo_foobarDiff = ms.GetDifferential(foo, fooBar);

            WriteDifference(foo_bardiff, nameof(foo_bardiff));
            WriteDifference(foo_foobarDiff, nameof(foo_foobarDiff));

            Console.WriteLine(("done"));

        }

        static void WriteDifference(ObjectDifferential od, string differentialName)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Object differential for {differentialName}");
            Console.ResetColor();

            if (od.MatchingProperties.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Matching and equal properties");
                Console.ResetColor();

                foreach (ObjectProperty op in od.MatchingProperties)
                {
                    Console.WriteLine($"The Property '{op.PropertyName}' == '{op.Value}' on both objects A and B");
                }
            }

            if (od.MismatchingProperties.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Properties with same names, but mismatching types or values");
                Console.ResetColor();

                foreach (ObjectPropertyPair opp in od.MismatchingProperties)
                {
                    Console.WriteLine($"The Property '{opp.PropertyName}' is different between objects A and B");
                    WriteMismatchValue(nameof(opp.A), opp.A);
                    WriteMismatchValue(nameof(opp.B), opp.B);
                }
            }

            if (od.OrphanPropertiesInA.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Properties only in object 'A'");
                Console.ResetColor();

                foreach (ObjectProperty op in od.OrphanPropertiesInA)
                {
                    Console.WriteLine($"The Property ['{op.PropertyName}' == '{op.Value}'] only exists on Object A");
                }
            }

            if (od.OrphanPropertiesInB.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Properties only in object 'B'");
                Console.ResetColor();

                foreach (ObjectProperty op in od.OrphanPropertiesInB)
                {
                    Console.WriteLine($"The Property ['{op.PropertyName}' == '{op.Value}'] only exists on Object B");
                }
            }
        }

        static void WriteMismatchValue(string objref, string value)
        {
            Console.Write("\t\t");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(objref);
            Console.ResetColor();
            Console.Write(" == ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(value);
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    public class TestClassFoo 
    {
        public string Foo { get; set; }
    }

    public class TestClassBar {
        public int Foo { get; set; }

        public string Bar { get; set; }
    }

    public class TestClassFooBar {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }
}
