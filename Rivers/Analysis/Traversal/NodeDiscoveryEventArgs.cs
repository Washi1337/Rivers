using System;

namespace Rivers.Analysis.Traversal
{
    public class NodeDiscoveryEventArgs : DiscoveryEventArgs
    {
        public NodeDiscoveryEventArgs(Node newNode, Edge origin)
        {
            NewNode = newNode;
            Origin = origin;
        }
        
        public Node NewNode
        {
            get;
        }

        public Edge Origin
        {
            get;
        }
    }
}