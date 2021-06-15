using System;
using System.IO;

namespace Logging.Common
{
    public interface IFileSinkOptions
    {
        Func<Stream> GetFileStream { get; }
    }
}
