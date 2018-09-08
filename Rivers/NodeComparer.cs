using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers
{
    public class NodeComparer : IEqualityComparer<Node>
    {
        public IEqualityComparer<string> NameComparer
        {
            get;
            set;
        } = StringComparer.InvariantCulture;

        public IEqualityComparer<IDictionary<object, object>> UserDataComparer
        {
            get;
            set;
        } = new UserDataComparer();

        public bool IncludeName
        {
            get;
            set;
        } = true;

        public bool IncludeUserData
        {
            get;
            set;
        } = false;

        public bool IncludeInDegree
        {
            get;
            set;
        } = true;

        public bool IncludeOutDegree
        {
            get;
            set;
        } = true;
        
        public bool Equals(Node x, Node y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            if (IncludeName && !NameComparer.Equals(x.Name , y.Name))
                return false;

            if (IncludeUserData && !UserDataComparer.Equals(x.UserData, y.UserData))
                return false;

            if (IncludeInDegree && x.InDegree != y.InDegree)
                return false;

            if (IncludeOutDegree && x.OutDegree != y.OutDegree)
                return false;

            return true;
        }

        public int GetHashCode(Node obj)
        {
            throw new System.NotImplementedException();
        }
    }
}