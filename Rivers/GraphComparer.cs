using System.Collections.Generic;

namespace Rivers
{
    public class GraphComparer : IEqualityComparer<Graph>
    {
        public GraphComparer()
        {
            NodeComparer = new NodeComparer();
            EdgeComparer = new EdgeComparer();
        }
        
        public IEqualityComparer<Node> NodeComparer
        {
            get;
            set;
        }

        public IEqualityComparer<Edge> EdgeComparer
        {
            get;
            set;
        }
        
        public bool Equals(Graph x, Graph y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            if (x.Nodes.Count != y.Nodes.Count || x.Edges.Count != y.Edges.Count)
                return false;

            foreach (var n1 in x.Nodes)
            {
                if (!y.Nodes.TryGetNode(n1.Name, out var n2) || NodeComparer.Equals(n1, n2))
                    return false;
            }

            foreach (var e1 in x.Edges)
            {
                if (!y.Edges.TryGetEdge(e1.Source.Name, e1.Target.Name, out var e2) || !EdgeComparer.Equals(e1, e2))
                    return false;
            }

            return true;
        }

        public int GetHashCode(Graph obj)
        {
            throw new System.NotImplementedException();
        }
    }
}