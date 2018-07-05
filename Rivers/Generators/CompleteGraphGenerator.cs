using System;

namespace Rivers.Generators
{
    public class CompleteGraphGenerator : IGraphGenerator
    {
        public CompleteGraphGenerator(int size)
        {
            if (Size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be a non-negative number.");
            
            Size = size;
        }
        
        public int Size
        {
            get;
        }
        
        public Graph GenerateGraph()
        {
            var g = new Graph(false);

            for (int i = 1; i <= Size; i++)
                g.Nodes.Add(i.ToString());

            for (int i = 1; i <= Size; i++)
            {
                for (int j = 1; j <= Size; j++)
                {
                    if (i != j)
                        g.Edges.Add(i.ToString(), j.ToString());
                }
            }

            return g;
        }
    }
}