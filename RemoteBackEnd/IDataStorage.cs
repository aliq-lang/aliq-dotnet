using System.Collections.Generic;

namespace RemoteBackEnd
{
    interface IDataStorage
    {
        bool Exist(string id);

        IEnumerable<T> Read<T>(string id);

        void Save<T>(string id, IEnumerable<T> data);

        IDataWriter<T> Create<T>(string id);

        IDataReader<T> Open<T>(string id);
    }
}
