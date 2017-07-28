using System.Collections.Generic;

namespace RemoteBackEnd
{
    public interface INodes
    {
        void ShareData<T>(int nodeId, string name, IEnumerable<(string, T)> data);

        IEnumerable<(string, T)> GetData<T>(int nodeId, string name);
    }
}
