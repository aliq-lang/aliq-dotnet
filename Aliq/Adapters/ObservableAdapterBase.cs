using Aliq.Bags;
using System;

namespace Aliq.Adapters
{
    public abstract class ObservableAdapterBase
    {
        public IObservable<T> Get<T>(Bag<T> bag)
            => Map.GetOrCreate(bag, () => bag.Accept(CreateCreateVisitor<T>()));

        internal abstract CreateVisitorBase<T> CreateCreateVisitor<T>();

        internal ObservableMap Map { get; } = new ObservableMap();

        internal abstract class CreateVisitorBase<T> : Bag<T>.IVisitor<IObservable<T>>
        {
            public abstract IObservable<T> Visit<I>(SelectMany<T, I> selectMany);
            public abstract IObservable<T> Visit(Merge<T> merge);
            public abstract IObservable<T> Visit(ExternalInput<T> externalInput);
            public abstract IObservable<T> Visit(Const<T> const_);
            public abstract IObservable<T> Visit<I>(GroupBy<T, I> groupBy);
            public abstract IObservable<T> Visit<A, B>(Product<T, A, B> product);
        }

        /*
        internal abstract class CreateVisitorBase<T> : Bag<T>.IVisitor<IObservable<T>>
        {
            public CreateVisitorBase(ObservableAdapterBase adapter)
            {
                Adapter = adapter;
            }

            public abstract IObservable<T> Visit<I>(SelectMany<T, I> selectMany);
            public abstract IObservable<T> Visit(Merge<T> merge);
            public abstract IObservable<T> Visit(ExternalInput<T> externalInput);
            public abstract IObservable<T> Visit(Const<T> const_);
            public abstract IObservable<T> Visit<I>(GroupBy<T, I> groupBy);
            public abstract IObservable<T> Visit<A, B>(Product<T, A, B> product);

            private ObservableAdapterBase Adapter { get; }
        }
        */
    }
}
