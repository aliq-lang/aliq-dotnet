using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Reactive.Linq;

namespace Aliq.Adapters
{
    public sealed class ObservableAdapter : ObservableAdapterBase
    {
        public void SetInput<T>(ExternalInput<T> bag, IObservable<T> input)
            => Map.GetOrCreate(bag, () => input);

        internal override CreateVisitorBase<T> CreateCreateVisitor<T>()
            => new CreateVisitor<T>(this);

        private sealed class CreateVisitor<T> : CreateVisitorBase<T>
        {
            public CreateVisitor(ObservableAdapter adapter)
            {
                Adapter = adapter;
            }

            public override IObservable<T> Visit<I>(SelectMany<T, I> selectMany)
                => Adapter.Get(selectMany.Input).SelectMany(selectMany.Func);

            public override IObservable<T> Visit(Merge<T> merge)
            {
                var a = Adapter.Get(merge.InputA);
                var b = Adapter.Get(merge.InputB);
                return a.Merge(b);
            }

            public override IObservable<T> Visit(ExternalInput<T> externalInput)
                => throw new Exception("no input");

            public override IObservable<T> Visit(Const<T> const_)
                => Observable.Return(const_.Value);

            public override IObservable<T> Visit<I>(GroupBy<T, I> groupBy)
                => Adapter
                    .Get(groupBy.Input)
                    .GroupBy(x => x.Key, x => x.Value)
                    .SelectMany(g => g.Aggregate(groupBy.Reduce).Select(v => (g.Key, v)))
                    .SelectMany(groupBy.GetResult);

            public override IObservable<T> Visit<A, B>(Product<T, A, B> product)
            {
                var a = Adapter.Get(product.InputA);
                var b = Adapter.Get(product.InputB);
                return a.SelectMany(ai => b.SelectMany(bi => product.Func(ai, bi)));
            }

            private ObservableAdapter Adapter { get; }
        }
    }
}
