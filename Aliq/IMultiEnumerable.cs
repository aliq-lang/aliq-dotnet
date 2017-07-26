using System.Collections;
using System.Collections.Generic;

namespace Aliq
{
    public interface IMultiEnumerable
    {
        int Count { get; }
        IEnumerable this[int i] { get; }
    }

    public interface IMultiEnumerable<T> : IMultiEnumerable
    {
        new IEnumerable<T> this[int i] { get; }
    }
}
