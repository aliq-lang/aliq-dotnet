using System;
using System.Collections.Generic;

namespace Aliq.Bags
{
    public sealed class GroupBy<T, I> : Bag<T>
    {
        public Bag<(string Key, I Value)> Input { get; }

        /// <summary>
        /// Reduce values.
        /// </summary>
        public Func<I, I, I> Reduce { get; }

        /// <summary>
        /// Convert a key/value to a final result.
        /// </summary>
        public Func<(string, I), IEnumerable<T>> GetResult { get; }

        public GroupBy(
            Bag<(string Key, I Value)> input,
            Func<I, I, I> reduce,
            Func<(string Key, I Value), IEnumerable<T>> getResult)
        {
            Input = input;
            Reduce = reduce;
            GetResult = getResult;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
