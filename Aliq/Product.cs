using System;
using System.Linq.Expressions;

namespace Aliq
{
    public sealed class Product<T, A, B> : Bag<T>
    {
        public Bag<A> InputA { get; }

        public Bag<B> InputB { get; }

        public Expression<Func<A, B, T>> Func { get; }

        public Product(Bag<A> inputA, Bag<B> inputB, Expression<Func<A, B, T>> func)
        {
            InputA = inputA;
            InputB = inputB;
            Func = func;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
