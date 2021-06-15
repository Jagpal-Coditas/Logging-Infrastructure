using System;
using System.IO;

namespace Logging.Common
{
    public class FileSinkOptions: IFileSinkOptions
    {
        public FileSinkOptions(Func<Stream> getFileStream)
        {
            if (getFileStream == null)
                throw new ArgumentNullException("GetFileStream");

            GetFileStream = getFileStream;
        }
        public Func<Stream> GetFileStream { get; }
    }
}
