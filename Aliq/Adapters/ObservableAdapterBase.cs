using Aliq.Bags;
using System;
using System.Reactive.Linq;

namespace Aliq.Adapters
{
    public abstract class ObservableAdapterBase<O>
        where O : ObservableAdapterBase<O>
    {
        public IObservable<T> Get<T>(Bag<T> bag)
            => Map.GetOrCreate(bag, () => bag.Accept(CreateCreateVisitor<T>()));

        internal abstract CreateVisitorBase<T> CreateCreateVisitor<T>();

        internal ObservableMap Map { get; } = new ObservableMap();

        internal abstract class CreateVisitorBase<T> : Bag<T>.IVisitor<IObservable<T>>
        {
            protected CreateVisitorBase(O adapter)
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

            public abstract IObservable<T> Visit(ExternalInput<T> externalInput);
            public abstract IObservable<T> Visit(Const<T> const_);

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

            protected O Adapter { get; }
        }
    }
}
