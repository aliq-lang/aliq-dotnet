using Aliq.Bags;

namespace Aliq
{
    public interface IDataBinding
    {
        void Set<T>(Bag<T> bag, string objectId);
    }
}
