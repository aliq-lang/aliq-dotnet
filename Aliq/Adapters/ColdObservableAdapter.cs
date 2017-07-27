using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Aliq.Adapters
{
    public sealed class ColdObservableAdapter
    {
        public void SetInput<T>(ExternalInput<T> bag, IObservable<T> input)
            => Map.GetOrCreate(bag, () => input);

        public IObservable<T> Get<T>(Bag<T> bag)
            => Map.GetOrCreate(bag, () => bag.Accept(new CreateVisitor<T>(this)));

        private sealed class CreateVisitor<T> : Bag<T>.IVisitor<IObservable<T>>
        {
            public CreateVisitor(ColdObservableAdapter adapter)
            {
                Adapter = adapter;
            }

            public IObservable<T> Visit<I>(SelectMany<T, I> selectMany)
                => Adapter.Get(selectMany.Input).SelectMany(selectMany.Func);

            public IObservable<T> Visit(Merge<T> merge)
            {
                var a = Adapter.Get(merge.InputA);
                var b = Adapter.Get(merge.InputB);
                return a.Merge(b);
            }

            public IObservable<T> Visit(ExternalInput<T> externalInput)
            {
                throw new Exception("No external input");
            }

            public IObservable<T> Visit(Const<T> const_)
                => Observable.Return(const_.Value);

            public IObservable<T> Visit<I>(GroupBy<T, I> groupBy)
                => Adapter
                    .Get(groupBy.Input)
                    .GroupBy(x => x.Key, x => x.Value)
                    .SelectMany(g => g.Aggregate(groupBy.Reduce).Select(v => (g.Key, v)))
                    .SelectMany(groupBy.GetResult);

            public IObservable<T> Visit<A, B>(Product<T, A, B> product)
            {
                var a = Adapter.Get(product.InputA);
                var b = Adapter.Get(product.InputB);
                return a.SelectMany(ai => b.SelectMany(bi => product.Func(ai, bi)));
            }

            private ColdObservableAdapter Adapter { get; }
        }

        private sealed class BagMap
        {
            public IObservable<T> GetOrCreate<T>(Bag<T> bag, Func<IObservable<T>> create)
                => Dictionary.GetOrCreate(bag, create);

            private Dictionary<Bag, object> Dictionary { get; }
                = new Dictionary<Bag, object>();
        }

        private BagMap Map { get; } = new BagMap();
    }
}
