using System;
using System.Collections.Generic;

namespace Aliq
{
    public sealed class GroupBy<T, I> : Bag<T>
    {
        public Bag<(string, I)> Input { get; }

        /// <summary>
        /// Reduce values.
        /// </summary>
        public Func<I, I, I> Reduce { get; }

        /// <summary>
        /// Convert a key/value to a final result.
        /// </summary>
        public Func<(string, I), IEnumerable<T>> GetResult { get; }

        public GroupBy(
            Bag<(string, I)> input,
            Func<I, I, I> reduce,
            Func<(string, I), IEnumerable<T>> getResult)
        {
            Input = input;
            Reduce = reduce;
            GetResult = getResult;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
