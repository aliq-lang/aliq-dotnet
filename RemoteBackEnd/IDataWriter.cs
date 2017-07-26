using System;

namespace RemoteBackEnd
{
    interface IDataWriter<T> : IDisposable
    {
        void Append(T value);
    }
}
