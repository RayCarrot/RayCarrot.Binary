using System;
using System.IO;
using System.Text;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Expanded binary writer
    /// </summary>
    public class Writer : BinaryWriter
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="endian">The endianness to use when writing</param>
        public Writer(Stream stream, Endian endian) : base(stream)
        {
            Endian = endian;
            CurrentXORKey = 0;
            CurrentChecksumCalculator = null;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The endianness to use when writing
        /// </summary>
        protected Endian Endian { get; }

        /// <summary>
        /// The current XOR key to use when writing
        /// </summary>
        protected byte CurrentXORKey { get; set; }

        /// <summary>
        /// The current checksum calculator to add to when writing
        /// </summary>
        protected IChecksumCalculator CurrentChecksumCalculator { get; set; }

        #endregion

        #region Public Writing Methods

        public override void Write(int value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(short value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(uint value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(ushort value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(long value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(ulong value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(float value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public override void Write(double value)
        {
            var data = BitConverter.GetBytes(value);

            if (Endian == Endian.Little != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            Write(data);
        }

        public void WriteNullDelimitedString(string value, Encoding encoding)
        {
            byte[] data = encoding.GetBytes(value + '\0');

            Write(data);
        }

        public void WriteString(string value, int size, Encoding encoding)
        {
            byte[] data = encoding.GetBytes(value + '\0');

            size *= encoding.GetByteCount("A");

            if (data.Length != size)
                Array.Resize<byte>(ref data, size);

            Write(data);
        }

        public override void Write(byte[] buffer)
        {
            var data = buffer;

            CurrentChecksumCalculator?.AddBytes(data);

            if (CurrentXORKey != 0)
            {
                // Avoid changing data in array, so create a copy
                data = new byte[buffer.Length];

                Array.Copy(buffer, 0, data, 0, buffer.Length);

                for (int i = 0; i < data.Length; i++)
                    data[i] = (byte)(data[i] ^ CurrentXORKey);
            }

            base.Write(data);
        }

        public override void Write(byte value)
        {
            byte data = value;

            CurrentChecksumCalculator?.AddByte(data);

            if (CurrentXORKey != 0)
                data = (byte)(data ^ CurrentXORKey);

            base.Write(data);
        }

        public override void Write(sbyte value) => Write((byte)value);

        #endregion

        #region XOR & Checksum

        /// <summary>
        /// Begins XOR encryption when writing
        /// </summary>
        /// <param name="xorKey">The XOR key</param>
        public void BeginXOR(byte xorKey) => CurrentXORKey = xorKey;

        /// <summary>
        /// Ends XOR encryption when writing
        /// </summary>
        public void EndXOR() => CurrentXORKey = 0;

        /// <summary>
        /// Begins checksum calculation when writing
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to use</param>
        public void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) => CurrentChecksumCalculator = checksumCalculator;

        /// <summary>
        /// Ends checksum calculation when writing and returns the current checksum value
        /// </summary>
        /// <typeparam name="T">The type of checksum value</typeparam>
        /// <returns>The checksum value</returns>
        public T EndCalculateChecksum<T>()
        {
            IChecksumCalculator c = CurrentChecksumCalculator;
            CurrentChecksumCalculator = null;
            return ((IChecksumCalculator<T>)c).ChecksumValue;
        }
        
        #endregion
    }
}