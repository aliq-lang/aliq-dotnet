using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Reactive.Linq;

namespace Aliq.Adapters
{
    public sealed class ObservableAdapter : ObservableAdapterBase<ObservableAdapter>
    {
        public void SetInput<T>(ExternalInput<T> bag, IObservable<T> input)
            => Map.GetOrCreate(bag, () => input);

        internal override CreateVisitorBase<T> CreateCreateVisitor<T>()
            => new CreateVisitor<T>(this);

        private sealed class CreateVisitor<T> : CreateVisitorBase<T>
        {
            public CreateVisitor(ObservableAdapter adapter) : base(adapter)
            {
            }

            public override IObservable<T> Visit(ExternalInput<T> externalInput)
                => throw new Exception("no input");

            public override IObservable<T> Visit(Const<T> const_)
                => Observable.Return(const_.Value);
        }
    }
}
