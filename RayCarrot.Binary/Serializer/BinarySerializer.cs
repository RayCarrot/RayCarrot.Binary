using System;
using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Default binary serializer, for writing to a stream
    /// </summary>
    public class BinarySerializer : IBinarySerializer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="settings">The serializer settings</param>
        /// <param name="stream">The stream to serialize to</param>
        /// <param name="logger">An optional logger to use for logging</param>
        public BinarySerializer(IBinarySerializerSettings settings, Stream stream, IBinarySerializerLogger logger = null)
        {
            Settings = settings;
            Stream = stream;
            Logger = logger;
            Writer = new Writer(stream, Settings.Endian);
        }

        /// <summary>
        /// Writes a supported value to the stream
        /// </summary>
        /// <param name="value">The value</param>
        public void Write(object value)
        {
            if (value is byte[] ba)
                Writer.Write(ba);

            else if (value is Array a)
                foreach (var item in a)
                    Write(item);

            else if (value.GetType().IsEnum)
                Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));

            else if (value is bool bo)
                Writer.Write((byte)(bo ? 1 : 0));

            else if (value is sbyte sb)
                Writer.Write((byte)sb);

            else if (value is byte by)
                Writer.Write(by);

            else if (value is short sh)
                Writer.Write(sh);

            else if (value is ushort ush)
                Writer.Write(ush);

            else if (value is int i32)
                Writer.Write(i32);

            else if (value is uint ui32)
                Writer.Write(ui32);

            else if (value is long lo)
                Writer.Write(lo);

            else if (value is ulong ulo)
                Writer.Write(ulo);

            else if (value is float fl)
                Writer.Write(fl);

            else if (value is double dou)
                Writer.Write(dou);

            else if (value is string s)
                Writer.WriteNullDelimitedString(s, Settings.StringEncoding);

            else
                throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported.");
        }

        /// <summary>
        /// Serializes a value
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="value">The value</param>
        /// <param name="name">The value name, for logging</param>
        /// <returns>The serialized value</returns>
        public T Serialize<T>(T value, string name = null)
        {
            Logger?.WriteLogLine($"{LogPrefix}({typeof(T)}) {name ?? "<no name>"}: {value}");

            // Write the value
            Write(value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Serializes an array
        /// </summary>
        /// <typeparam name="T">The array value type</typeparam>
        /// <param name="array">The array</param>
        /// <param name="length">The array length</param>
        /// <param name="name">The array name, for logging</param>
        /// <returns>The serialized array</returns>
        public T[] SerializeArray<T>(T[] array, int length, string name = null)
        {
            // Use byte writing method if it's a byte array
            if (typeof(T) == typeof(byte))
            {
                if (Logger != null)
                {
                    string normalLog = $"{LogPrefix}({typeof(T)}[{length}]) {name ?? "<no name>"}: ";
                    Logger?.WriteLogLine($"{normalLog}{BinaryHelpers.ByteArrayToHexString((byte[])(object)array, 16, new string(' ', normalLog.Length))}");
                }

                Writer.Write((byte[])(object)array);
            }
            else
            {
                Logger?.WriteLogLine($"{LogPrefix}({typeof(T)}[{length}]) {name ?? "<no name>"}");

                // Write every value
                for (int i = 0; i < length; i++)
                    Serialize<T>(array[i], name: name == null || Logger == null ? null : $"{name}[{i}]");
            }

            // Return the array
            return array;
        }

        /// <summary>
        /// Serializes an array size
        /// </summary>
        /// <typeparam name="T">The array value type</typeparam>
        /// <typeparam name="V">The array size value type to serialize</typeparam>
        /// <param name="array">The array</param>
        /// <param name="name">The array name, for logging</param>
        /// <returns>The array with the serialized size</returns>
        public T[] SerializeArraySize<T, V>(T[] array, string name = null)
        {
            // Get the size of the specified type
            V size = (V)Convert.ChangeType(array?.Length ?? 0, typeof(V));

            // Write the value using the serializer
            Serialize<V>(size, name: Logger == null ? null : $"{name}.Length");
            
            // Return the array
            return array;
        }

        /// <summary>
        /// Serializes a string of a specified length
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="length">The string length</param>
        /// <param name="name">The string value name, for logging</param>
        /// <returns>The serialized string value</returns>
        public string SerializeString(string value, int length, string name = null)
        {
            Logger?.WriteLogLine($"{LogPrefix}(string) {name ?? "<no name>"}: {value}");

            // Write the string
            Writer.WriteString(value, length, Settings.StringEncoding);

            // Return the value
            return value;
        }

        /// <summary>
        /// Serializes a serializable object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="value">The serializable object</param>
        /// <param name="onPreSerializing">Optional action to run before serializing</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The serialized object</returns>
        public T SerializeObject<T>(T value, Action<IBinarySerializer, T> onPreSerializing = null, string name = null) 
            where T : IBinarySerializable, new()
        {
            Logger?.WriteLogLine($"{LogPrefix}(Object: {typeof(T)}) {name ?? "<no name>"}");

            Depth++;

            // Run pre-serializing method
            onPreSerializing?.Invoke(this, value);

            // Serialize the value
            value.Serialize(this);

            Depth--;

            // Return the value
            return value;
        }

        /// <summary>
        /// Serializes an array of serializable objects
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="array">The serializable object array</param>
        /// <param name="length">The array length</param>
        /// <param name="onPreSerializing">Optional action to run before serializing each object</param>
        /// <param name="name">The object value name, for logging</param>
        /// <returns>The serialized object array</returns>
        public T[] SerializeObjectArray<T>(T[] array, int length, Action<IBinarySerializer, T> onPreSerializing = null, string name = null)
            where T : IBinarySerializable, new()
        {
            Logger?.WriteLogLine($"{LogPrefix}(Object[] {typeof(T)}[{length}]) {name ?? "<no name>"}");

            // Write every value
            for (int i = 0; i < length; i++)
                SerializeObject<T>(array[i], onPreSerializing: onPreSerializing, name: name == null || Logger == null ? null : $"{name}[{i}]");

            // Return the array
            return array;
        }

        /// <summary>
        /// Begins using an xor key for serializing
        /// </summary>
        /// <param name="xorKey">The xor key to use</param>
        public void BeginXOR(byte xorKey) => Writer.BeginXOR(xorKey);

        /// <summary>
        /// Stops using an xor key for serializing
        /// </summary>
        public void EndXOR() => Writer.EndXOR();

        /// <summary>
        /// Gets a value indicating if the serializer is currently reading values. If false it is writing values.
        /// </summary>
        public bool IsReading => false;

        /// <summary>
        /// The serializer settings
        /// </summary>
        public IBinarySerializerSettings Settings { get; }

        /// <summary>
        /// The stream to serialize to
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// An optional logger to use for logging
        /// </summary>
        public IBinarySerializerLogger Logger { get; }

        /// <summary>
        /// The binary writer
        /// </summary>
        protected Writer Writer { get; }

        /// <summary>
        /// The log prefix to use when writing a log
        /// </summary>
        protected string LogPrefix => Logger == null ? null : $"(W) 0x{Stream.Position:X8}:{new string(' ', (Depth + 1) * 2)}";

        /// <summary>
        /// The depths, for logging
        /// </summary>
        protected int Depth { get; set; }
    }
}