using System;
using Genetics;

namespace GeneTest
{
    public class Solution
    {
        public int Fitness()
        {

            // not fit at all
            return 0;
        }
    }


    internal class Program
    {
        private static void Main(string[] args)
        {
            var generations = 500000;
            OrganismFactory<Organism>.GeneCount = 1000;
            var a = OrganismFactory<Organism>.CreateByDivineIntervention();
            a.Name = "Frank";
            var b = OrganismFactory<Organism>.CreateByDivineIntervention();
            b.Name = "Mary";
            var  c = a.Mate(b);
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