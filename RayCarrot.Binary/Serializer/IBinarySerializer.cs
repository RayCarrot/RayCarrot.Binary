using System;
using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Defines a binary serializer
    /// </summary>
    public interface IBinarySerializer
    {
        /// <summary>
        /// Serializes a value
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="value">The value</param>
        /// <param name="name">The value name, for logging</param>
        /// <returns>The serialized value</returns>
        T Serialize<T>(T value, string name = null);

        /// <summary>
        /// Serializes an array
        /// </summary>
        /// <typeparam name="T">The array value type</typeparam>
        /// <param name="array">The array</param>
        /// <param name="length">The array length</param>
        /// <param name="name">The array name, for logging</param>
        /// <returns>The serialized array</returns>
        T[] SerializeArray<T>(T[] array, int length, string name = null);

        /// <summary>
        /// Serializes an array size
        /// </summary>
        /// <typeparam name="T">The array value type</typeparam>
        /// <typeparam name="V">The array size value type to serialize</typeparam>
        /// <param name="array">The array</param>
        /// <param name="name">The array name, for logging</param>
        /// <returns>The array with the serialized size</returns>
        T[] SerializeArraySize<T, V>(T[] array, string name = null);

        /// <summary>
        /// Serializes a string of a specified length
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="length">The string length</param>
        /// <param name="name">The string value name, for logging</param>
        /// <returns>The serialized string value</returns>
        string SerializeString(string value, int length, string name = null);

        /// <summary>
        /// Serializes a serializable object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="value">The serializable object</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The serialized object</returns>
        T SerializeObject<T>(T value, Action<IBinarySerializer, T> onPreSerializing = null, string name = null)
            where T : IBinarySerializable, new();

        /// <summary>
        /// Serializes an array of serializable objects
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="array">The serializable object array</param>
        /// <param name="length">The array length</param>
        /// <param name="onPreSerializing">Optional action to run before serializing each object</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The serialized object array</returns>
        T[] SerializeObjectArray<T>(T[] array, int length, Action<IBinarySerializer, T> onPreSerializing = null, string name = null)
            where T : IBinarySerializable, new();

        /// <summary>
        /// Begins using an xor key for serializing
        /// </summary>
        /// <param name="xorKey">The xor key to use</param>
        void BeginXOR(byte xorKey);

        /// <summary>
        /// Stops using an xor key for serializing
        /// </summary>
        void EndXOR();

        /// <summary>
        /// Gets a value indicating if the serializer is currently reading values. If false it is writing values.
        /// </summary>
        bool IsReading { get; }

        /// <summary>
        /// The serializer settings
        /// </summary>
        IBinarySerializerSettings Settings { get; }

        /// <summary>
        /// The stream
        /// </summary>
        Stream Stream { get; }
    }
}