namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Provides a default implementation for the serialization of user data attributes in graphs stored in dot files.
    /// </summary>
    public class DefaultUserDataSerializer : IUserDataSerializer
    {
        /// <inheritdoc />
        public string Serialize(string attributeName, object attributeValue)
        {
            return attributeValue.ToString();
        }

        /// <inheritdoc />
        public object Deserialize(string attributeName, string rawValue)
        {
            return rawValue;
        }
    }
}