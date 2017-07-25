using System;

namespace RemoteBackEnd
{
    interface IDataReader<T> : IDisposable
    {
        T Read();
    }
}
