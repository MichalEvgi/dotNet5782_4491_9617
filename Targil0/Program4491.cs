using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome4491();
            Welcome9617();
            Console.ReadKey();
        }
        static partial void Welcome9617();
        private static void Welcome4491()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}