using Aliq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RemoteBackEnd
{
    sealed class BackEnd
    {
        public DataBinding DataBinding { get; }
            = new DataBinding();

        public void Create<T>(Bag<T> bag)
        {
            // TODO: check if file exist then exit.
            var output = Get(bag);
            // TODO: output to a file.
        }

        private IEnumerable<T> Get<T>(Bag<T> bag)
            => bag.Accept(new GetVisitor<T>(this));

        private sealed class GetVisitor<T> : Bag<T>.IVisitor<IEnumerable<T>>
        {
            public GetVisitor(BackEnd backEnd)
            {
                BackEnd = backEnd;
            }

            private BackEnd BackEnd { get; }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
                => BackEnd.Get(selectMany.Input).SelectMany(selectMany.Func);

            public IEnumerable<T> Visit(DisjointUnion<T> disjointUnion)
            {
                var a = BackEnd.Get(disjointUnion.InputA);
                var b = BackEnd.Get(disjointUnion.InputB);
                return a.Concat(b);
            }

            public IEnumerable<T> Visit(ExternalInput<T> externalInput)
            {
                // TODO: read from a file.
                throw new NotImplementedException();
            }

            public IEnumerable<T> Visit(Const<T> const_)
            {
                yield return const_.Value;
            }

            public IEnumerable<T> Visit<I>(GroupBy<T, I> groupBy)
            {
                // TODO: read from a file.
                throw new NotImplementedException();
            }

            public IEnumerable<T> Visit<A, B>(Product<T, A, B> product)
            {
                // TODO: read from a file.
                throw new NotImplementedException();
            }
        }
    }
}
