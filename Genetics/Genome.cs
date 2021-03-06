﻿using System;
using System.Collections.Generic;

namespace Genetics
{
    public class Genome
    {
        private static int MutationCount;

        public int DefectRate = 5;
        private const int MaxRand = 100;

        private static int MaxGene()
        {
            return  Enum.GetValues(typeof(Gene)).Length;
        }

        private readonly List<Gene> _genes = new List<Gene>();

        public Genome(int geneCount)
        {
            var r = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < geneCount; i++)
            {
                var v = r.Next(MaxGene());
                _genes.Add((Gene)v); 
                
            }

        }

        public Genome()
        {
        }


        public void Dump()
        {
            Console.WriteLine();
            _genes.ForEach(x => Console.Write(x));
            Console.WriteLine($"  -  {MutationCount} mutations");

        }

        public void AddGene(Gene g)
        {
            _genes.Add(g);
        }

        public Genome Mate(Genome mate)
        {
            // first method, copy all from mate, then cross
            //var child = Clone(mate);
            var child = Clone(mate, this);

            // new blend (allow for cross species)
            var min = Math.Min(child._genes.Count, _genes.Count);
            var r = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < min; i++)    // compare in blocks of 2 (make it interesting)
            {
                var ra = r.Next();
                if (ra % MaxRand > MaxRand / 2)
                {
                    child._genes[i] = _genes[i];
     
                }
                if (ra % MaxRand > MaxRand - DefectRate)
                {
                    MutationCount++;
                    child._genes[i] = (Gene) (ra % MaxGene());
                }
                if (ra % MaxRand < 5)
                {
                    // lose a gene
                    child._genes.RemoveAt(i);
                    min--;
                }
                else if (ra % MaxRand < 10)
                {
                    // gain a gene
                    child._genes.Add((Gene) (ra % MaxGene()));
                }

            }
            return child;
        }


        private static Genome Clone(Genome me)
        {
            var newChild = new Genome();
            newChild._genes.AddRange(me._genes);
            return newChild;
        }

        private static Genome Clone(Genome me, Genome other)
        {
            var newChild = new Genome();
            var min = Math.Min(me._genes.Count, other._genes.Count);
            var r = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < min; i++)
            {
                newChild.AddGene(r.Next(2)==1 ? me._genes[i] : other._genes[i]);
                
            }
            return newChild;
        }

        // Note:
        // genes denote modifiers against natural abilities of organism
    }

    public enum ModifierRank
    {
        
    }
}
