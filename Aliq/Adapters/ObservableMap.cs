using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Collections.Generic;

namespace Aliq.Adapters
{
    internal sealed class ObservableMap
    {
        public R GetOrCreate<T, R>(Bag<T> bag, Func<R> create)
            => Dictionary.GetOrCreate(bag, create);

        private Dictionary<Bag, object> Dictionary { get; }
            = new Dictionary<Bag, object>();
    }
}
