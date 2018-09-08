using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rivers
{
    public class UserDataComparer : IEqualityComparer<IDictionary<object, object>>
    {
        public bool Equals(IDictionary<object, object> x, IDictionary<object, object> y)
        {
            if (x.Count != y.Count)
                return false;
                
            foreach (var entry in x)
            {
                if (!y.TryGetValue(entry.Key, out var value))
                    return false;

                if (entry.Value is IEnumerable collection1 && value is IEnumerable collection2)
                {
                    if (!collection1.Cast<object>().SequenceEqual(collection2.Cast<object>()))
                        return false;
                }
                else if (!Equals(entry.Value, value))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(IDictionary<object, object> obj)
        {
            return obj.GetHashCode();
        }
    }
}