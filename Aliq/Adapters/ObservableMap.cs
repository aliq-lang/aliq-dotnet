using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Collections.Generic;

namespace Aliq.Adapters
{
    internal sealed class ObservableMap
    {
        public IObservable<T> GetOrCreate<T>(Bag<T> bag, Func<IObservable<T>> create)
            => Dictionary.GetOrCreate(bag, create);

        private Dictionary<Bag, object> Dictionary { get; }
            = new Dictionary<Bag, object>();
    }
}
