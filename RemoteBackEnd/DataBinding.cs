using System;
using Aliq;
using System.Collections.Generic;

namespace RemoteBackEnd
{
    class DataBinding : IDataBinding
    {
        public void Set<T>(Bag<T> bag, string objectId)
        {
            Map[objectId] = bag;
        }

        private Dictionary<string, Bag> Map = new Dictionary<string, Bag>();
    }
}
