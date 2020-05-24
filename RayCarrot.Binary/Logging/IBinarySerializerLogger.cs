using System;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Interface for a logger for a binary serializer
    /// </summary>
    public interface IBinarySerializerLogger : IDisposable
    {
        /// <summary>
        /// Writes a new log line
        /// </summary>
        /// <param name="log">The log to write</param>
        void WriteLogLine(string log);
    }
}