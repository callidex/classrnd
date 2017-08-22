using System;
using System.Dynamic;
using Genetics;

namespace GeneTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int generations = 1000;
            Genetics.Genome a = new Genome(100);
            a.Dump();
            Genetics.Genome b = new Genome(100);
            b.Dump();
            Genome c =a ;
            for (int i = 0; i < generations; i++)
            {
                c = c.Mate(b);
                c.Dump();
            }
        }
    }
}