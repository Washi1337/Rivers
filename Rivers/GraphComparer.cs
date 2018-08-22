using System.Collections.Generic;
using System.Linq;

namespace Rivers
{
    public class GraphComparer : IEqualityComparer<Graph>
    {
        public bool IncludeUserData
        {
            get;
            set;
        } = false;

        public bool IncludeSubGraphs
        {
            get;
            set;
        } = true;

        public IEqualityComparer<Node> NodeComparer
        {
            get;
            set;
        } = new NodeComparer();

        public IEqualityComparer<Edge> EdgeComparer
        {
            get;
            set;
        } = new EdgeComparer();
        
        public bool Equals(Graph x, Graph y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            if (x.IsDirected != y.IsDirected)
                return false;
            
            if (x.Nodes.Count != y.Nodes.Count || x.Edges.Count != y.Edges.Count)
                return false;

            foreach (var n1 in x.Nodes)
            {
                if (!y.Nodes.TryGetNode(n1.Name, out var n2)
                    || !NodeComparer.Equals(n1, n2))
                {
                    return false;
                }
            }

            foreach (var e1 in x.Edges)
            {
                if (!y.Edges.TryGetEdge(e1.Source.Name, e1.Target.Name, out var e2)
                    || !EdgeComparer.Equals(e1, e2))
                {
                    return false;
                }
            }

            if (IncludeUserData)
            {
                if (x.UserData.Count != y.UserData.Count)
                    return false;
                
                foreach (var entry in x.UserData)
                {
                    if (!y.UserData.TryGetValue(entry.Key, out var value) || !Equals(entry.Value, value))
                        return false;
                }
            }

            if (IncludeSubGraphs)
            {
                if (x.SubGraphs.Count != y.SubGraphs.Count)
                    return false;

                var candidates = y.SubGraphs.ToList();
                foreach (var subGraph in x.SubGraphs)
                {
                    var names = new HashSet<string>(subGraph.Nodes.Select(n => n.Name));
                    
                    // Find corresponding sub graph.
                    foreach (var other in candidates.Where(g => g.Name == subGraph.Name))
                    {
                        // Check nodes.
                        if (names.SetEquals(subGraph.Nodes.Select(n => n.Name)))
                        {
                            if (IncludeUserData)
                            {
                                if (subGraph.UserData.Count != other.UserData.Count)
                                    return false;
                
                                foreach (var entry in subGraph.UserData)
                                {
                                    if (!other.UserData.TryGetValue(entry.Key, out var value) || !Equals(entry.Value, value))
                                        return false;
                                }
                            }
                            
                            // Sub graph matched. Remove from candidate list.
                            candidates.Remove(other);
                            break;
                        }

                        return false;
                    }
                }

                // If there are candidates left, we didn't match all sub graphs.
                if (candidates.Count > 0)
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