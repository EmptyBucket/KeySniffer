using System;
using System.IO;
using System.Text;

namespace SnifferKey
{
    class Logger : IDisposable
    {
        public readonly string PathLogFile;
        private StringBuilder buffer;
        private readonly StreamWriter streamWriter;
        private readonly FileStream fileStream;

        public Logger(string pathLogFile, int bufferSize = int.MaxValue)
        {
            PathLogFile = pathLogFile;
            fileStream = new FileStream(PathLogFile,
                FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            fileStream.Seek(fileStream.Length, SeekOrigin.Begin);
            streamWriter = new StreamWriter(fileStream);

            buffer = new StringBuilder(8, bufferSize);
        }

        public void Dispose()
        {
            fileStream.Close();
            streamWriter.Close();
        }

        public void WriteBuffer(char symbols)
        {
            try
            {
                buffer.Append(symbols);
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException || ex is ArgumentOutOfRangeException)
                {
                    SaveData();
                    WriteBuffer(symbols);
                    return;
                }
                throw ex;
            }
        }

        private void writeFrame(string str)
        {
            if (str.Length < buffer.MaxCapacity)
            {
                buffer.Append(str);
            }
            else
            {
                string subStr = str.Substring(0, buffer.MaxCapacity);
                buffer.Append(subStr);
                SaveData();
                writeFrame(str.Substring(buffer.MaxCapacity));
            }
        }

        public void WriteBuffer(string str)
        {
            int lengthCapacity = buffer.MaxCapacity - buffer.Length;
            if (str.Length < lengthCapacity)
            {
                buffer.Append(str);
            }
            else
            {
                string subStr = str.Substring(0, lengthCapacity);
                buffer.Append(subStr);
                SaveData();
                writeFrame(str.Substring(lengthCapacity));
            }
        }

        public void ClearLogFile()
        {
            new FileStream(PathLogFile, FileMode.Truncate,
                FileAccess.Write, FileShare.ReadWrite).Close();
            fileStream.Seek(0, SeekOrigin.Begin);
        }

        public void SaveData()
        {
            streamWriter.Write(buffer);
            streamWriter.Flush();
            buffer.Clear();
        }
    }
}
