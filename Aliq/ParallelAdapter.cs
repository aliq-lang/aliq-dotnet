using System.Collections.Generic;

namespace Aliq
{
    public sealed class ParallelAdapter
    {
        public ParallelAdapter()
        {
        }

        public void SetInput<T>(ExternalInput<T> input, IMultiEnumerable<T> data)
        {
            Map[input] = data;
        }

        private Dictionary<Bag, IMultiEnumerable> Map { get; }
            = new Dictionary<Bag, IMultiEnumerable>();
    }
}
