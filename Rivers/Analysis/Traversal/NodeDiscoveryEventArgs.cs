using System;

namespace Rivers.Analysis.Traversal
{
    public class NodeDiscoveryEventArgs : DiscoveryEventArgs
    {
        public NodeDiscoveryEventArgs(Node newNode, Edge origin, int depth)
        {
            NewNode = newNode;
            Origin = origin;
            Depth = depth;
        }
        
        public Node NewNode
        {
            get;
        }

        public Edge Origin
        {
            get;
        }

        public int Depth
        {
            get;
        }
    }
}