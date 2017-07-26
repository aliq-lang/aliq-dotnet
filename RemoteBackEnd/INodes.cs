using System.Collections.Generic;

namespace RemoteBackEnd
{
    public interface INodes
    {
        void SendData<T>(int nodeId, string name, IEnumerable<(string, T)> data);

        IEnumerable<(string, T)> RecieveData<T>(int nodeId, string name);
    }
}
