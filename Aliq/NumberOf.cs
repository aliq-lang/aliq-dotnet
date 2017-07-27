using Aliq.Linq;
using System.Collections.Generic;

namespace Aliq
{
    public struct NumberOf<T>
    {
        public T Value { get; }

        public long Count { get; }

        public NumberOf(T value, long count)
        {
            Value = value;
            Count = count;
        }

        public NumberOf<T> Add(long more)
            => Value.NumberOf(Count + more);

        public IEnumerable<T> Repeat()
        {
            for (long i = Count; i > 0; --i)
                yield return Value;
        }
    }
}
