using System;
using System.Collections.Generic;
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

        public SubGraph(Graph parentGraph, string name) : this(parentGraph)
        {
            Name = name;
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