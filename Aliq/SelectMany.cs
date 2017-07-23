using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aliq
{
    public sealed class SelectMany<T, I> : Bag<T>
    {
        public Bag<I> Input { get; }

        public CompiledExpression<Func<I, IEnumerable<T>>> Func { get; }

        public SelectMany(Bag<I> input, Expression<Func<I, IEnumerable<T>>> func)
        {
            Input = input;
            Func = func.Compiled();
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
