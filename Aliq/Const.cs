namespace Aliq
{
    public sealed class Const<T> : Bag<T>
    {
        public T Value { get; }

        public Const(T value)
        {
            Value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
