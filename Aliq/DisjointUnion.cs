namespace Aliq
{
    public sealed class DisjointUnion<T> : Bag<T>
    {
        public Bag<T> InputA { get; }

        public Bag<T> InputB { get; }

        public DisjointUnion(Bag<T> inputA, Bag<T> inputB)
        {
            InputA = inputA;
            InputB = inputB;
        }

        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
