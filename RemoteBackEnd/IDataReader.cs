using System;

namespace RemoteBackEnd
{
    interface IDataReader<T> : IDisposable
    {
        (bool, T) Read();
    }
}
