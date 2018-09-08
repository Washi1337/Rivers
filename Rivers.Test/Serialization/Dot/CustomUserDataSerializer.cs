using System.Collections.Generic;
using System.Linq;
using Rivers.Serialization.Dot;

namespace Rivers.Test.Serialization.Dot
{
    public class CustomUserDataSerializer : IUserDataSerializer
    {
        public string Serialize(string attributeName, object attributeValue)
        {
            if (attributeName == "MyIntegerList" && attributeValue is IList<int> items)
                return string.Join(",", items);

            return attributeValue.ToString();
        }

        public object Deserialize(string attributeName, string rawValue)
        {
            if (attributeName == "MyIntegerList")
                return rawValue.Split(',').Select(int.Parse).ToList();

            return rawValue;
        }
    }
}