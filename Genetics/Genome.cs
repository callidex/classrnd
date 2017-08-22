using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetics
{
    public class Genome
    {
        private int MaxGene()
        {
            return (int)Enum.GetValues(typeof(Gene)).Cast<Gene>().Max();
        }

        private List<Gene> _genes = new List<Gene>();

        public Genome(int geneCount)
        {
            var r = new Random(DateTime.Now.Millisecond);

            for (int i=0;i<geneCount;i++)
                _genes.Add((Gene) (r.Next() % MaxGene()));
        }
        public Genome()
        {
        }


        public void Dump()
        {
            _genes.ForEach(x=>Console.Write(x));
            Console.WriteLine();
            
        }
        public void AddGene(Gene g)
        {
            _genes.Add(g);
        }

        public Genome Mate(Genome mate)
        {
            Genome child = Clone(mate);

            // new blend (allow for cross species)
            var min = Math.Min(child._genes.Count, _genes.Count);
            var r = new Random(DateTime.Now.Millisecond);
            
            for (int i = 0; i < min;i++)
            {
                int ra = r.Next();
                if (ra % 50 > 25)
                {
                    child._genes[i] = this._genes[i];
                }
                if (ra % 50 > 45)
                {
                    Console.WriteLine("Mutation!");
                    int newg = (int)child._genes[i] ;
                    child._genes[i] =(Gene) (++newg% MaxGene()); 
                }
                if (ra % 50 == 48)
                {
                    // lose a gene
                    Console.WriteLine("Lost one!");
                    child._genes.RemoveAt(i);
                    min--;
                }
                if (ra % 50 == 49)
                {
                    // gain a gene
                    Console.WriteLine("Gained one!");
                    child._genes.Add((Gene)(ra % MaxGene()));
                    
                }

            }
            return child;
        }


        public static Genome Clone(Genome me)
        {
            Genome newChild = new Genome();
            newChild._genes.AddRange(me._genes);
            return newChild;
        }

    }

    public enum Gene
    {
        G=0,A,C,T
    }
}
