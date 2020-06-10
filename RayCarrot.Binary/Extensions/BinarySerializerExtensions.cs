using System;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Extension methods for <see cref="IBinarySerializer"/>
    /// </summary>
    public static class BinarySerializerExtensions
    {
        /// <summary>
        /// Gets the settings of a specified type
        /// </summary>
        /// <typeparam name="T">The settings type</typeparam>
        /// <param name="s">The serializer</param>
        /// <returns>The settings, or null if in the wrong format</returns>
        public static T GetSettings<T>(this IBinarySerializer s)
            where T : class, IBinarySerializerSettings
        {
            return s.Settings as T;
        }

        /// <summary>
        /// Serializes a string with the length prefixed as a 32-bit integer
        /// </summary>
        /// <param name="s">The serializer</param>
        /// <param name="value">The string value</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The string value</returns>
        public static string SerializeLengthPrefixedString(this IBinarySerializer s, string value, string name = null)
        {
            // Serialize the length
            var length = s.Serialize<int>(value?.Length ?? 0, name: $"{name}.Length");

            // Serialize the string
            return s.SerializeString(value, length, name);
        }

        /// <summary>
        /// Serializes a boolean formatted as the specified integer type
        /// </summary>
        /// <typeparam name="V">The integer type the boolean is formatted as</typeparam>
        /// <param name="s">The serializer</param>
        /// <param name="value">The boolean value</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The boolean value</returns>
        public static bool SerializeBool<V>(this IBinarySerializer s, bool value, string name = null)
        {
            // Get the current value in the correct format
            var formattedValue = (V)Convert.ChangeType(value ? 1 : 0, typeof(V));

            // Serialize the value
            var serializedValue = s.Serialize<V>(formattedValue, name: name);

            // Convert the serialized value to a byte
            var serializedByteValue = (byte)Convert.ChangeType(serializedValue, typeof(byte));

            // TODO: Make sure it's formatted correctly

            // Return as a boolean
            return serializedByteValue == 1;
        }

        /// <summary>
        /// Performs an xor operation on the bytes serialized during the specified action
        /// </summary>
        /// <param name="s">The serializer</param>
        /// <param name="xorKey">The xor key to use</param>
        /// <param name="action">The action</param>
        public static void DoXOR(this IBinarySerializer s, byte xorKey, Action action)
        {
            s.BeginXOR(xorKey);

            action?.Invoke();

            s.EndXOR();
        }
    }
}