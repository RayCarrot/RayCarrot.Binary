using System.Text;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Default implementation for <see cref="IBinarySerializerSettings"/>
    /// </summary>
    public class BinarySerializerSettings : IBinarySerializerSettings
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endian">The endianness to use</param>
        /// <param name="stringEncoding">The string encoding to use</param>
        public BinarySerializerSettings(Endian endian, Encoding stringEncoding)
        {
            Endian = endian;
            StringEncoding = stringEncoding;
        }

        /// <summary>
        /// The endianness to use
        /// </summary>
        public Endian Endian { get; }

        /// <summary>
        /// The string encoding to use
        /// </summary>
        public Encoding StringEncoding { get; }
    }
}