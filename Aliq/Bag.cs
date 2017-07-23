using System.Linq;

namespace Aliq
{
    public abstract class Bag
    {
        public interface IVisitor<R>
        {
            R Visit<T>(Bag<T> bag);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);

        public static Bag<T> Empty<T>()
            => false.ToConstBag().SelectMany(v => Enumerable.Empty<T>());
    }

    public abstract class Bag<T> : Bag
    {
        public sealed override R Accept<R>(Bag.IVisitor<R> visitor)
            => visitor.Visit(this);

        public new interface IVisitor<R>
        {
            R Visit<I>(SelectMany<T, I> selectMany);
            R Visit(DisjointUnion<T> disjointUnion);
            R Visit(ExternalInput<T> externalInput);
            R Visit(Const<T> const_);
            R Visit<K>(GroupBy<T, K> groupBy);
            R Visit<A, B>(Product<T, A, B> product);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);
    }
}
