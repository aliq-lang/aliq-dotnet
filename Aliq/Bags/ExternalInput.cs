namespace Aliq.Bags
{
    public sealed class ExternalInput<T> : Bag<T>
    {
        public override R Accept<R>(IVisitor<R> visitor)
            => visitor.Visit(this);
    }
}
