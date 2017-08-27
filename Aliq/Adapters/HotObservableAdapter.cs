using Aliq.Bags;
using Aliq.Linq;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Aliq.Adapters
{
    public sealed class HotObservableAdapter : ObservableAdapterBase<HotObservableAdapter>
    {
        public void SetInput<T>(ExternalInput<T> bag, IObservable<T> input)
        {
            var subject = (Subject<T>)Get(bag);
            StartSubject.Subscribe(_ => input.Subscribe(subject));
        }

        public void Start()
            => Observable
                .Return(new Void())
                .Subscribe(StartSubject);

        internal override CreateVisitorBase<T> CreateCreateVisitor<T>()
            => new CreateVisitor<T>(this);

        private Subject<Void> StartSubject { get; }
            = new Subject<Void>();

        private sealed class CreateVisitor<T> : CreateVisitorBase<T>
        {
            public CreateVisitor(HotObservableAdapter adapter): base(adapter)
            {
            }

            public override IObservable<T> Visit(ExternalInput<T> externalInput)
                => new Subject<T>();

            public override IObservable<T> Visit(Const<T> const_)
                => Adapter.StartSubject.Select(_ => const_.Value);
        }
    }
}
