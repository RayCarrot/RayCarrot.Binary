using System;
using System.Text;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Helper methods for binary operations
    /// </summary>
    public static class BinaryHelpers
    {
        /// <summary>
        /// Convert a byte array to a hex string
        /// </summary>
        /// <param name="Bytes">The byte array to convert</param>
        /// <param name="Align">Should the byte array be split in different lines, this defines the length of one line</param>
        /// <param name="NewLinePrefix">The prefix to add to each new line</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] Bytes, int? Align = null, string NewLinePrefix = null)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            const string hexAlphabet = "0123456789ABCDEF";

            for (int i = 0; i < Bytes.Length; i++)
            {
                if (i > 0 && Align.HasValue && i % Align == 0)
                    Result.Append("\n" + (NewLinePrefix ?? String.Empty));

                byte B = Bytes[i];
                Result.Append(hexAlphabet[B >> 4]);
                Result.Append(hexAlphabet[B & 0xF]);

                if (i < Bytes.Length - 1) 
                    Result.Append(' ');
            }

            return Result.ToString();
        }
    }
}