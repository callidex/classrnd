namespace Genetics
{
    public static class OrganismFactory<T> where T : Organism, new()
    {
        public static int GeneCount = 10;
   
        public static T CreateByDivineIntervention() 
        {
            
            T t = new T();
            t.Genome = new Genome(GeneCount);

            return t;
        }
    }
}