using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Aliq
{
    public sealed class InMemory
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
            public Visitor(InMemory inMemory)
            {
                InMemory = inMemory;
            }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
            {
                return InMemory
                    .Get(selectMany.Input)
                    .SelectMany(selectMany.Func.Compiled);
            }

            public IEnumerable<T> Visit(DisjointUnion<T> disjointUnion)
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

            public IEnumerable<T> Visit<K, I>(GroupBy<T, K, I> groupBy)
            {
                
                var reduce = groupBy.Reduce.Compiled;
                return InMemory
                    .Get(groupBy.Input)
                    .GroupBy(
                        groupBy.GetKey.Compiled,
                        groupBy.GetElement.Compiled,
                        (key, list) => list.Aggregate(reduce),
                        groupBy.Comparer);
            }

            public IEnumerable<T> Visit<A, B>(Product<T, A, B> product)            
            {
                var a = InMemory.Get(product.InputA);
                var b = InMemory.Get(product.InputB);
                var p = product.Func.Compiled;
                return a.SelectMany(ai => b.SelectMany(bi => p(ai, bi)));
            }

            private InMemory InMemory;
        }
    }
}
