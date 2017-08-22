using System;
using System.Collections.Generic;
using Genetics;

namespace GeneTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var generations = 1000;
            var a = new Genome(100);
            var b = new Genome(100);
            Genome c = a.Mate(b);
            Console.WriteLine("First Child");
            c.Dump();
            for (var i = 0; i < generations; i++)
            {
                c = a.Mate(b);
            }

            Console.WriteLine("Original Parents");
            
            a.Dump();
            b.Dump();
            Console.WriteLine("Why cousins don't marry");
            c.Dump();
            Console.ReadKey();
        }
    }
}