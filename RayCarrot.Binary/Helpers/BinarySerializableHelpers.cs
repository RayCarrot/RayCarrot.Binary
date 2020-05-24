using System;
using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Helper methods for serializing binary data
    /// </summary>
    public static class BinarySerializableHelpers
    {
        /// <summary>
        /// Reads a binary file using the default deserializer
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="filePath">The file path</param>
        /// <param name="settings">The serializer settings</param>
        /// <param name="logger">An optional logger to use for logging</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        /// <returns>The serialized file data</returns>
        public static T ReadFromFile<T>(string filePath, IBinarySerializerSettings settings, IBinarySerializerLogger logger = null, Action<IBinarySerializer, T> onPreSerializing = null)
            where T : IBinarySerializable, new()
        {
            using (logger)
            {
                // Open the file as a stream
                using (var stream = File.OpenRead(filePath))
                    // Read from the stream
                    return ReadFromStream<T>(stream, settings, logger, onPreSerializing);
            }
        }

        /// <summary>
        /// Reads from a binary stream using the default deserializer
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="stream">The stream</param>
        /// <param name="settings">The serializer settings</param>
        /// <param name="logger">An optional logger to use for logging</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        /// <returns>The serialized data</returns>
        public static T ReadFromStream<T>(Stream stream, IBinarySerializerSettings settings, IBinarySerializerLogger logger = null, Action<IBinarySerializer, T> onPreSerializing = null)
            where T : IBinarySerializable, new()
        {
            using (logger)
            {
                // Create the deserializer
                var s = new BinaryDeserializer(settings, stream, logger);

                // Serialize the object
                var obj = s.SerializeObject<T>(default, onPreSerializing);

                // Return the object
                return obj;
            }
        }

        /// <summary>
        /// Writes to a binary file using the default serializer
        /// </summary>
        /// <typeparam name="T">The type to serialize from</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="filePath">The file path</param>
        /// <param name="settings">The serializer settings</param>
        /// <param name="logger">An optional logger to use for logging</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        public static void WriteToFile<T>(T obj, string filePath, IBinarySerializerSettings settings, IBinarySerializerLogger logger = null, Action<IBinarySerializer, T> onPreSerializing = null)
            where T : IBinarySerializable, new()
        {
            using (logger)
            {
                // Open the file as a stream
                using (var stream = File.OpenWrite(filePath))
                    // Write to the stream
                    WriteToStream<T>(obj, stream, settings, logger, onPreSerializing);
            }
        }

        /// <summary>
        /// Writes to a binary stream using the default serializer
        /// </summary>
        /// <typeparam name="T">The type to serialize from</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="stream">The stream</param>
        /// <param name="settings">The serializer settings</param>
        /// <param name="logger">An optional logger to use for logging</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        public static void WriteToStream<T>(T obj, Stream stream, IBinarySerializerSettings settings, IBinarySerializerLogger logger = null, Action<IBinarySerializer, T> onPreSerializing = null)
            where T : IBinarySerializable, new()
        {
            using (logger)
            {
                // Create the serializer
                var s = new BinarySerializer(settings, stream, logger);

                // Serialize the object
                s.SerializeObject<T>(obj, onPreSerializing);
            }
        }
    }
}