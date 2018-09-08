namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Provides members for serializing and deserializing attribute values in nodes and graphs in a dot file.
    /// </summary>
    public interface IUserDataSerializer
    {
        /// <summary>
        /// Serializes an attribute value to a string. 
        /// </summary>
        /// <param name="attributeName">The name of the attribute to serialize the value of.</param>
        /// <param name="attributeValue">The value to serialize.></param>
        /// <returns>The string representation of the attribute value.</returns>
        string Serialize(string attributeName, object attributeValue);

        /// <summary>
        /// Deserializes a raw string to an attribute value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to deserialize the value from.</param>
        /// <param name="rawValue">The string representation of the attribute value.</param>
        /// <returns>The deserialized attribute value.</returns>
        object Deserialize(string attributeName, string rawValue);
    }
}