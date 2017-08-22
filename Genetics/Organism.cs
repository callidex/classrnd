using System;

namespace Genetics
{
    public class Organism
    {
        public Genome Genome { get; set; }
        private string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public Organism Mate(Organism mate)
        {
            return new Organism(){Genome = mate.Genome.Mate(this.Genome),Name= "unnamed child"};
        }

        public void Dump()
        {
            Console.Write($" {Name}: ");
            this.Genome.Dump();
        }
    }
}