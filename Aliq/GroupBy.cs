using System;
using System.Linq.Expressions;

namespace Aliq
{
    public sealed class GroupBy<T, K> : Bag<T>
    {
        public Bag<T> Input { get; }

        public Expression<Func<T, K>> GetKey { get; }

        public Expression<Func<T, T, T>> Reduce { get; }

        public GroupBy(
            Bag<T> input,
            Expression<Func<T, K>> getKey,
            Expression<Func<T, T, T>> reduce)
        {
            Input = input;
            GetKey = getKey;
            Reduce = reduce;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
