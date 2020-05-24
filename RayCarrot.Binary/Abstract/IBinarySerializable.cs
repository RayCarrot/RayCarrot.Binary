namespace RayCarrot.Binary
{
    /// <summary>
    /// The interface to use for a class which is serializable using a binary serializer
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Handles the serialization using the specified serializer
        /// </summary>
        /// <param name="s">The serializer</param>
        void Serialize(IBinarySerializer s);
    }
}