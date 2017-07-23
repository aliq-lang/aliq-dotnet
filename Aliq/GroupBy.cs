using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aliq
{
    public sealed class GroupBy<T, K, V> : Bag<T>
    {
        public Bag<(K, V)> Input { get; }

        public IEqualityComparer<K> Comparer { get; }

        /// <summary>
        /// Reduce values.
        /// </summary>
        public CompiledExpression<Func<V, V, V>> Reduce { get; }

        /// <summary>
        /// Convert a key/value to a final result.
        /// </summary>
        public CompiledExpression<Func<(K, V), IEnumerable<T>>> GetResult { get; }

        public GroupBy(
            Bag<(K, V)> input,
            Expression<Func<V, V, V>> reduce,
            Expression<Func<(K, V), IEnumerable<T>>> getResult,
            IEqualityComparer<K> comparer)
        {
            Input = input;
            Reduce = reduce.Compiled();
            GetResult = getResult.Compiled();
            Comparer = comparer;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
