using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteBackEnd
{
    public class FileDataReader<T> : IDataReader<T>
    {
        private TextReader file;

        public FileDataReader(string name)
        {
            FileStream namedFile = new FileStream(name, FileMode.Open, FileAccess.Read);
            file = new StreamReader(namedFile, Encoding.UTF8);
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public (bool, T) Read()
        {

            StringBuilder currentSerializedBuilder = new StringBuilder();

            char currentChar;
            int num;

            while ((num = file.Read()) > -1)
            {
                currentChar = (char)num;
                if (num == 0)
                {
                    string finalString = currentSerializedBuilder.ToString();
                    T finalObject = JsonConvert.DeserializeObject<T>(finalString);

                    return (true, finalObject);
                }
                else
                    currentSerializedBuilder.Append(currentChar);
            }
            

            return (false, default(T));
        }
    }
}
