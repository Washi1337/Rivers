using System;
using System.Collections.Generic;
using System.Linq;
using Rivers.Collections;

namespace Rivers
{
    public class SubGraph
    {
        public SubGraph(Graph parentGraph)
        {
            ParentGraph = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
            UserData = new Dictionary<object, object>();
            Nodes = new SubGraphNodeCollection(this);
        }

        public SubGraph(Graph parentGraph, string name)
            : this(parentGraph)
        {
            Name = name;
        }

        public SubGraph(params Node[] nodes)
            : this((ICollection<Node>) nodes)
        {
        }

        public SubGraph(ICollection<Node> nodes)
            : this(null, nodes)
        {
        }

        public SubGraph(string name, params Node[] nodes)
            : this(name, (ICollection<Node>) nodes)
        {
        }

        public SubGraph(string name, ICollection<Node> nodes)
            : this(nodes.First().ParentGraph, name)
        {
            if (nodes.Any(x => x.ParentGraph != ParentGraph))
                throw new ArgumentException("Nodes should all be present in the same graph.");
            foreach (var node in nodes)
                Nodes.Add(node);
        }

        public Graph ParentGraph
        {
            get;
        }

        public string Name
        {
            get;
            set;
        }

        public IDictionary<object, object> UserData
        {
            get;
        }

        public SubGraphNodeCollection Nodes
        {
            get;
        }
    }
}