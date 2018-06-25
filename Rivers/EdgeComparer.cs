using System;
using System.Collections.Generic;

namespace Rivers
{
    public class EdgeComparer : IEqualityComparer<Edge>
    {
        public EdgeComparer()
        {
            NameComparer = StringComparer.InvariantCulture;
        }

        public IEqualityComparer<string> NameComparer
        {
            get;
            set;
        }

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