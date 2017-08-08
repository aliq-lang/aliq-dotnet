using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteBackEnd
{
    public class FileDataWriter<T> : IDataWriter<T>
    {
        private TextWriter file;
        
        public FileDataWriter(string name)
        {
            FileStream namedFile = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write);
            file = new StreamWriter(namedFile, Encoding.UTF8);
        }

        public void Append(T value)
        {
            string serializedJSON = JsonConvert.SerializeObject(value);
            file.Write(serializedJSON + "\0");
        }

        public void Dispose()
        {
            file.Dispose();
        }
    }
}
