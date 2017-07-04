namespace Aliq
{
    public interface INumericPolicy<T>
    {
        T Add(T a, T b);
        T FromLong(long v);
        T Div(T a, T b);
    }
}
