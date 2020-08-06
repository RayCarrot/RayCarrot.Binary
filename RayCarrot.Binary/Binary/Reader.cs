using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Expanded binary reader
    /// </summary>
    public class Reader : BinaryReader
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="endian">The endianness to use when reading</param>
        /// <param name="leaveOpen">Indicates if the stream should be left open after disposing the reader</param>
        public Reader(Stream stream, Endian endian, bool leaveOpen = false) : base(stream, Encoding.Default, leaveOpen)
        {
            Endian = endian;
            CurrentXORKey = 0;
            CurrentChecksumCalculator = null;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The endianness to use when reading
        /// </summary>
        protected Endian Endian { get; }

        /// <summary>
        /// The current XOR key to use when reading
        /// </summary>
        protected byte CurrentXORKey { get; set; }

        /// <summary>
        /// The current checksum calculator to add to when reading
        /// </summary>
        protected IChecksumCalculator CurrentChecksumCalculator { get; set; }

        #endregion

        #region Public Reading Methods

        public override int ReadInt32()
        {
            var data = ReadBytes(sizeof(int));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian) 
                Array.Reverse(data);

            return BitConverter.ToInt32(data, 0);
        }

        public override float ReadSingle()
        {
            var data = ReadBytes(sizeof(float));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToSingle(data, 0);
        }

        public override short ReadInt16()
        {
            var data = ReadBytes(sizeof(short));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt16(data, 0);
        }

        public override ushort ReadUInt16()
        {
            var data = ReadBytes(sizeof(ushort));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }

        public override long ReadInt64()
        {
            var data = ReadBytes(sizeof(long));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt64(data, 0);
        }

        public override uint ReadUInt32()
        {
            var data = ReadBytes(sizeof(uint));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt32(data, 0);
        }

        public override ulong ReadUInt64()
        {
            var data = ReadBytes(sizeof(ulong));

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt64(data, 0);
        }

        public override byte[] ReadBytes(int count)
        {
            byte[] bytes = base.ReadBytes(count);

            if (CurrentChecksumCalculator?.CalculateForDecryptedData == false)
                CurrentChecksumCalculator?.AddBytes(bytes);

            if (CurrentXORKey != 0)
                for (int i = 0; i < count; i++)
                    bytes[i] = (byte)(bytes[i] ^ CurrentXORKey);

            if (CurrentChecksumCalculator?.CalculateForDecryptedData == true)
                CurrentChecksumCalculator?.AddBytes(bytes);

            return bytes;
        }

        public override sbyte ReadSByte() => (sbyte)ReadByte();

        public override byte ReadByte()
        {
            byte result = base.ReadByte();

            if (CurrentChecksumCalculator?.CalculateForDecryptedData == false)
                CurrentChecksumCalculator?.AddByte(result);

            if (CurrentXORKey != 0)
                result = (byte)(result ^ CurrentXORKey);

            if (CurrentChecksumCalculator?.CalculateForDecryptedData == true)
                CurrentChecksumCalculator?.AddByte(result);

            return result;
        }

        public string ReadNullDelimitedString(Encoding encoding)
        {
            List<byte> bytes = new List<byte>();

            byte b = ReadByte();

            while (b != 0x0)
            {
                bytes.Add(b);
                b = ReadByte();
            }

            if (bytes.Count > 0)
                return encoding.GetString(bytes.ToArray());

            return String.Empty;
        }
            
        public string ReadString(int size, Encoding encoding)
        {
            // Read the bytes
            byte[] bytes = ReadBytes(size * encoding.GetByteCount("A"));
            
            // Get the string from the bytes using the specified encoding
            var str = encoding.GetString(bytes);

            // Trim null characters
            str = str.TrimEnd((char)0x00);

            // Return the string
            return str;
        }

        #endregion

        #region XOR & Checksum

        public void BeginXOR(byte xorKey) => CurrentXORKey = xorKey;

        public void EndXOR() => CurrentXORKey = 0;

        public void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) => CurrentChecksumCalculator = checksumCalculator;

        public T EndCalculateChecksum<T>()
        {
            IChecksumCalculator c = CurrentChecksumCalculator;
            CurrentChecksumCalculator = null;
            return ((IChecksumCalculator<T>)c).ChecksumValue;
        }

        #endregion
    }
}