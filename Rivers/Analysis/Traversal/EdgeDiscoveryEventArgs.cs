namespace Rivers.Analysis.Traversal
{
    public class EdgeDiscoveryEventArgs : DiscoveryEventArgs
    {
        public EdgeDiscoveryEventArgs(Edge edge)
        {
            Edge = edge;
        }
        
        public Edge Edge
        {
            get;
        }
    }
}