using System;
using System.Linq;
using System.Linq.Expressions;
using Aliq;
using System.Collections.Generic;
namespace SimpleAliq
{
    public class SimpleBag 
    {
        Dictionary<Bag, IEnumerable> Map;

        public SimpleBag()
        {
            Map = new Dictionary<Bag, IEnumerable>();
        }

        public void SetInput<T>(ExternalInput<T> inputBag, IEnumerable<T> input)
        {
            Map[inputBag] = input;
        }

        public IEnumerable<T> Get<T>(Bag<T> inputBag)
        {
            return (IEnumerable<T>)inputBag.Accept(new SimpleBagVisitor<T>(this));
        }

        private class SimpleBagVisitor<T> : Bag<T>.IVisitor<IEnumerable<T>>
        {

            private static SimpleBag SingletonInstance;

            internal SimpleBagVisitor(SimpleBag instance)
            {
                SingletonInstance = instance;
            }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
            {
                throw new NotImplementedException();
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit(ExternalInput<T> externalInput)
            {
                return SingletonInstance.Map[externalInput];
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit<I>(SelectMany<T, I> selectMany)
            {
                // what does this do really???
                return SingletonInstance.Get(selectMany.Input).SelectMany(selectMany.Func.Compile());
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit(DisjointUnion<T> disjointUnion)
            {
                var a = SingletonInstance.Get(disjointUnion.InputA);
                var b = SingletonInstance.Get(disjointUnion.InputB);
                return a.Concat(b);
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit(Const<T> const_)
            {
                throw new NotImplementedException();
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit<K>(GroupBy<T, K> groupBy)
            {
                throw new NotImplementedException();
            }

            IEnumerable<T> Bag<T>.IVisitor<IEnumerable<T>>.Visit<A, B>(Product<T, A, B> product)
            {
                throw new NotImplementedException();
            }
        }
    }
}
