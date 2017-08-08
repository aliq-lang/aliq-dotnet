using System;
using System.Collections.Generic;

namespace Aliq.Bags
{
    public sealed class SelectMany<T, I> : Bag<T>
    {
        public Bag<I> Input { get; }

        public Func<I, IEnumerable<T>> Func { get; }

        public SelectMany(Bag<I> input, Func<I, IEnumerable<T>> func)
        {
            Input = input;
            Func = func;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
