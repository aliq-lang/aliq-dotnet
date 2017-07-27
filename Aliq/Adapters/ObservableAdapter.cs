using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Aliq.Adapters
{
    public sealed class ObservableAdapter
    {
        public void SetInput<T>(ExternalInput<T> bag, IObservable<T> input)
            => Map.GetOrCreate(bag, () => input);

        public IObservable<T> Get<T>(Bag<T> bag)
            => Map.GetOrCreate(bag, () => bag.Accept(new CreateVisitor<T>(this)));

        private sealed class CreateVisitor<T> : Bag<T>.IVisitor<IObservable<T>>
        {
            public CreateVisitor(ObservableAdapter adapter)
            {
                Adapter = adapter;
            }

            public IObservable<T> Visit<I>(SelectMany<T, I> selectMany)
                => Adapter.Get(selectMany.Input).SelectMany(selectMany.Func);

            public IObservable<T> Visit(Merge<T> disjointUnion)
            {
                var a = Adapter.Get(disjointUnion.InputA);
                var b = Adapter.Get(disjointUnion.InputB);
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
                    .GroupBy(x => x.Item1, x => x.Item2)
                    .SelectMany(g => g.Aggregate(groupBy.Reduce).Select(v => (g.Key, v)))
                    .SelectMany(groupBy.GetResult);

            public IObservable<T> Visit<A, B>(Product<T, A, B> product)
            {
                var a = Adapter.Get(product.InputA);
                var b = Adapter.Get(product.InputB);
                return a.SelectMany(ai => b.SelectMany(bi => product.Func(ai, bi)));
            }

            private ObservableAdapter Adapter { get; }
        }

        private sealed class BagMap
        {
            public IObservable<T> GetOrCreate<T>(Bag<T> bag, Func<IObservable<T>> create)
            {
                if (Dictionary.TryGetValue(bag, out var observable))
                {
                    return observable as IObservable<T>;
                }
                else
                {
                    var observableT = create();
                    Dictionary[bag] = observableT;
                    return observableT;
                }
            }

            private Dictionary<Bag, object> Dictionary { get; }
                = new Dictionary<Bag, object>();
        }

        private BagMap Map { get; } = new BagMap();
    }
}
