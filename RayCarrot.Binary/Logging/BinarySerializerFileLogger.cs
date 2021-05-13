using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Logs the logs from binary serialization to a file
    /// </summary>
    public class BinarySerializerFileLogger : IBinarySerializerLogger
    {
        /// <summary>
        /// Creates a new file logger which creates a new log file
        /// </summary>
        /// <param name="logPath">The log file path</param>
        public BinarySerializerFileLogger(string logPath)
        {
            FileStream = new StreamWriter(logPath);
        }

        /// <summary>
        /// Creates a new file logger from the stream
        /// </summary>
        /// <param name="stream">The log output stream</param>
        public BinarySerializerFileLogger(Stream stream)
        {
            FileStream = new StreamWriter(stream);
        }

        /// <summary>
        /// The file stream
        /// </summary>
        protected StreamWriter FileStream { get; }

        /// <summary>
        /// Writes a new log line
        /// </summary>
        /// <param name="log">The log to write</param>
        public void WriteLogLine(string log)
        {
            FileStream.WriteLine(log);
        }

        public void Dispose()
        {
            FileStream?.Dispose();
        }
    }
}