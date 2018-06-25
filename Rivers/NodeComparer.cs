using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers
{
    public class NodeComparer : IEqualityComparer<Node>
    {
        public NodeComparer()
        {
            NameComparer = StringComparer.InvariantCulture;
        }

        public IEqualityComparer<string> NameComparer
        {
            get;
            set;
        }
        
        public bool IncludeName
        {
            get; 
            set;
        }
        
        public bool IncludeUserData
        {
            get;
            set;
        }

        public bool IncludeInDegree
        {
            get;
            set;
        }

        public bool IncludeOutgoingEdges
        {
            get;
            set;
        }
        
        public bool Equals(Node x, Node y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            if (IncludeName && !NameComparer.Equals(x.Name , y.Name))
                return false;

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

            if (IncludeInDegree && x.InDegree != y.InDegree)
                return false;

            if (IncludeOutgoingEdges && x.OutDegree != y.OutDegree)
                return false;

            return true;
        }

        public int GetHashCode(Node obj)
        {
            throw new System.NotImplementedException();
        }
    }
}