namespace RayCarrot.Binary
{
    /// <summary>
    /// Wrapper for serializable values
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public class SerializableObjectWrapper<T> : IBinarySerializable
    {
        /// <summary>
        /// The value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Handles the serialization using the specified serializer
        /// </summary>
        /// <param name="s">The serializer</param>
        public void Serialize(IBinarySerializer s)
        {
            Value = s.Serialize<T>(Value, name: nameof(Value));
        }
    }
}