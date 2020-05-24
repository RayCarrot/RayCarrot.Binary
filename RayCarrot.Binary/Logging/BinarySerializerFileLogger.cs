using System.IO;

namespace RayCarrot.Binary
{
    /// <summary>
    /// Logs the logs from binary serialization to a file
    /// </summary>
    public class BinarySerializerFileLogger : IBinarySerializerLogger
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logPath">The log file path</param>
        public BinarySerializerFileLogger(string logPath)
        {
            LogPath = logPath;
            FileStream = new StreamWriter(logPath);
        }

        /// <summary>
        /// The log file path
        /// </summary>
        public string LogPath { get; }

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