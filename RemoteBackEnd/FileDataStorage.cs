using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteBackEnd
{
    class FileDataStorage : IDataStorage
    {
        public IDataWriter<T> Create<T>(string id)
        {
            return new FileDataWriter<T>(id);
        }

        public bool Exist(string id)
        {
            return File.Exists(id);
        }

        public IDataReader<T> Open<T>(string id)
        {
            return new FileDataReader<T>(id);
        }
    }
}
