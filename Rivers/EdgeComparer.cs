using System;
using System.Collections.Generic;

namespace Rivers
{
    public class EdgeComparer : IEqualityComparer<Edge>
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

        public bool IgnoreDirection
        {
            get;
            set;
        }

        public bool IncludeUserData
        {
            get;
            set;
        }

        public bool Equals(Edge x, Edge y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            if (!EndpointEquals(x, y))
                return false;
             
            if (IncludeUserData && !UserDataComparer.Equals(x.UserData, y.UserData))
                return false;
            
            return true;
        }

        private bool EndpointEquals(Edge x, Edge y)
        {
            if (IgnoreDirection)
            {
                if (NameComparer.Equals(x.Source.Name, y.Source.Name))
                {
                    return NameComparer.Equals(x.Target.Name, y.Target.Name);
                }
                else
                {
                    return NameComparer.Equals(x.Source.Name, y.Target.Name)
                           && NameComparer.Equals(x.Target.Name, y.Source.Name);
                }
            }

            if (!NameComparer.Equals(x.Source.Name, y.Source.Name))
                return false;

            if (!NameComparer.Equals(x.Target.Name, y.Target.Name))
                return false;

            return true;
        }

        public int GetHashCode(Edge obj)
        {
            throw new System.NotImplementedException();
        }
    }
}