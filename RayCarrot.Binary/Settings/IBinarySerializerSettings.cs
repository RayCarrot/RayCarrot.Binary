using System.Text;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Defines settings for a <see cref="IBinarySerializer"/>
    /// </summary>
    public interface IBinarySerializerSettings
    {
        /// <summary>
        /// The endianness to use
        /// </summary>
        Endian Endian { get; }

        /// <summary>
        /// The string encoding to use
        /// </summary>
        Encoding StringEncoding { get; }
    }
}