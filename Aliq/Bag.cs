using System.Linq;

namespace Aliq
{
    // Bag is a multiset (unordered list of elements which may contain duplicates)
    public abstract class Bag
    {
        public interface IVisitor<R>
        {
            
            R Visit<T>(Bag<T> bag);
        }

        // Example:
        // Bag b;
        // class Visitor : Bag.IVisitor<string>
        // {
        //    public string Visit<T>(Bag<T> bag)
        //    {
        //        return typeof(T).Name;
        //    }
        // }
        //
        // string itemTypeName = b.Accept(new Visitor());
        public abstract R Accept<R>(IVisitor<R> visitor);

        public static Bag<T> Empty<T>()
            => false.ToConstBag().SelectMany(v => Enumerable.Empty<T>());
    }

    // A Bag which contains elements of type T
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
            R Visit<I>(GroupBy<T, I> groupBy);
            R Visit<A, B>(Product<T, A, B> product);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);

        internal interface IVisitor
        {
        }
    }
}
