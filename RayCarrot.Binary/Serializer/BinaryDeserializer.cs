using System;
using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Default binary deserializer, for reading from a stream
    /// </summary>
    public class BinaryDeserializer : IBinarySerializer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="settings">The serializer settings</param>
        /// <param name="stream">The stream to deserialize from</param>
        /// <param name="logger">An optional logger to use for logging</param>
        public BinaryDeserializer(IBinarySerializerSettings settings, Stream stream, IBinarySerializerLogger logger = null)
        {
            Settings = settings;
            Stream = stream;
            Logger = logger;
            Reader = new Reader(stream, Settings.Endian);
        }

        /// <summary>
        /// Reads a value of the specified type from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <returns>The read value</returns>
        protected object Read<T>()
        {
            // Get the type
            var type = typeof(T);

            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    var b = Reader.ReadByte();

                    if (b != 0 && b != 1)
                        Logger?.WriteLogLine($"{LogPrefix}({typeof(T)}): Binary boolean was not correctly formatted ({b})");

                    return b == 1;

                case TypeCode.SByte:
                    return Reader.ReadSByte();

                case TypeCode.Byte:
                    return Reader.ReadByte();

                case TypeCode.Int16:
                    return Reader.ReadInt16();

                case TypeCode.UInt16:
                    return Reader.ReadUInt16();

                case TypeCode.Int32:
                    return Reader.ReadInt32();

                case TypeCode.UInt32:
                    return Reader.ReadUInt32();

                case TypeCode.Int64:
                    return Reader.ReadInt64();

                case TypeCode.UInt64:
                    return Reader.ReadUInt64();

                case TypeCode.Single:
                    return Reader.ReadSingle();

                case TypeCode.Double:
                    return Reader.ReadDouble();

                case TypeCode.String:
                    return Reader.ReadNullDelimitedString(Settings.StringEncoding);

                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    throw new NotSupportedException("The specified generic type can not be read from the reader");
            }
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
            // Read and cast the value
            var v = (T)Read<T>();

            Logger?.WriteLogLine($"{LogPrefix}({typeof(T)}) {name ?? "<no name>"}: {v}");

            return v;
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
            // Use byte reading method if it's a byte array
            if (typeof(T) == typeof(byte))
            {
                byte[] bytes = Reader.ReadBytes(length);

                string normalLog = $"{LogPrefix}({typeof(T)}[{length}]) {name ?? "<no name>"}: ";
                Logger?.WriteLogLine($"{normalLog}{BinaryHelpers.ByteArrayToHexString(bytes, 16, new string(' ', normalLog.Length))}");

                return (T[])(object)bytes;
            }

            Logger?.WriteLogLine($"{LogPrefix}({typeof(T)}[{length}]) {name ?? "<no name>"}");

            // Create the buffer to read to
            var buffer = new T[length];

            // Read each value to the buffer
            for (int i = 0; i < length; i++)
                // Read the value
                buffer[i] = Serialize<T>(default, name: name == null ? null : $"{name}[{i}]");

            // Return the buffer
            return buffer;
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
            // Create the size
            V Size = default;

            // Serialize the size
            Size = Serialize<V>(Size, name: name + ".Length");
            
            // If the array is null, create it with the specified size
            if (array == null)
            {
                // Convert the size to an int
                int intSize = (int)Convert.ChangeType(Size, typeof(int));
                
                // Create the array
                array = new T[intSize];
            }

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
            // Read the string from the reader
            var v = Reader.ReadString(length, Settings.StringEncoding);

            Logger?.WriteLogLine($"{LogPrefix}(string) {name ?? "<no name>"}: {v}");

            return v;
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
            // Create a new instance of the object
            var instance = new T();

            Logger?.WriteLogLine($"{LogPrefix}(Object: {typeof(T)}) {(name ?? "<no name>")}");

            Depth++;

            // Call pre-serializing action
            onPreSerializing?.Invoke(this, instance);
            
            // Serialize the object
            instance.Serialize(this);

            Depth--;

            // Return the object
            return instance;
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
            Logger?.WriteLogLine($"{LogPrefix}(Object[]: {typeof(T)}[{length}]) {name ?? "<no name>"}");

            // Create the buffer
            var buffer = new T[length];

            // Read each object to the buffer
            for (int i = 0; i < length; i++)
                // Read the object
                buffer[i] = SerializeObject<T>(default, onPreSerializing: onPreSerializing, name: name == null ? null : $"{name}[{i}]");

            return buffer;
        }

        /// <summary>
        /// Begins using an xor key for serializing
        /// </summary>
        /// <param name="xorKey">The xor key to use</param>
        public void BeginXOR(byte xorKey) => Reader.BeginXOR(xorKey);

        /// <summary>
        /// Stops using an xor key for serializing
        /// </summary>
        public void EndXOR() => Reader.EndXOR();

        /// <summary>
        /// Gets a value indicating if the serializer is currently reading values. If false it is writing values.
        /// </summary>
        public bool IsReading => true;

        /// <summary>
        /// The serializer settings
        /// </summary>
        public IBinarySerializerSettings Settings { get; }

        /// <summary>
        /// The stream to deserialize from
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// An optional logger to use for logging
        /// </summary>
        public IBinarySerializerLogger Logger { get; }

        /// <summary>
        /// The binary reader
        /// </summary>
        protected Reader Reader { get; }

        /// <summary>
        /// The log prefix to use when writing a log
        /// </summary>
        protected string LogPrefix => $"(READ) 0x{Stream.Position:X8}:{new string(' ', (Depth + 1) * 2)}";

        /// <summary>
        /// The depths, for logging
        /// </summary>
        protected int Depth { get; set; }
    }
}