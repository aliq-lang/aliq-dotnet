using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Aliq.Adapters
{
    public sealed class EnumerableAdapter
    {
        public void SetInput<T>(ExternalInput<T> inputBag, IEnumerable<T> inputData)
        {
            InputMap[inputBag] = inputData;
        }

        public IEnumerable<T> Get<T>(Bag<T> bag)
            => bag.Accept(new Visitor<T>(this));

        private Dictionary<Bag, IEnumerable> InputMap { get; }
            = new Dictionary<Bag, IEnumerable>();

        private sealed class Visitor<T> : Bag<T>.IVisitor<IEnumerable<T>>
        {
            public Visitor(EnumerableAdapter inMemory)
            {
                InMemory = inMemory;
            }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
            {
                return InMemory
                    .Get(selectMany.Input)
                    .SelectMany(selectMany.Func);
            }

            public IEnumerable<T> Visit(Merge<T> disjointUnion)
            {
                var a = InMemory.Get(disjointUnion.InputA);
                var b = InMemory.Get(disjointUnion.InputB);
                return a.Concat(b);
            }

            public IEnumerable<T> Visit(ExternalInput<T> externalInput)
                => (IEnumerable<T>)InMemory.InputMap[externalInput];

            public IEnumerable<T> Visit(Const<T> const_)
            {
                yield return const_.Value;
            }

            public IEnumerable<T> Visit<I>(GroupBy<T, I> groupBy)
                => InMemory
                    .Get(groupBy.Input)
                    .GroupBy(
                        v => v.Item1,
                        v => v.Item2,
                        (key, list) => (key, list.Aggregate(groupBy.Reduce)))
                    .SelectMany(groupBy.GetResult);

            public IEnumerable<T> Visit<A, B>(Product<T, A, B> product)            
            {
                var a = InMemory.Get(product.InputA);
                var b = InMemory.Get(product.InputB);
                return a.SelectMany(ai => b.SelectMany(bi => product.Func(ai, bi)));
            }

            private EnumerableAdapter InMemory;
        }
    }
}
