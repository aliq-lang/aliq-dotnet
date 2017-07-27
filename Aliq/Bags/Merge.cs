namespace Aliq.Bags
{
    public sealed class Merge<T> : Bag<T>
    {
        public Bag<T> InputA { get; }

        public Bag<T> InputB { get; }

        public Merge(Bag<T> inputA, Bag<T> inputB)
        {
            InputA = inputA;
            InputB = inputB;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
