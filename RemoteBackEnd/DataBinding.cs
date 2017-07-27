using Aliq;
using System;
using System.Collections.Generic;

namespace RemoteBackEnd
{
    public sealed class DataBinding : IDataBinding
    {
        public void Set<T>(Bag<T> bag, string bagId)
        {
            if (BagToId.ContainsKey(bag))
            {
                return;
            }
            if (IdToBag.ContainsKey(bagId))
            {
                throw new Exception(
                    "two bags can't have the same name: " + bagId);
            }
            IdToBag[bagId] = bag;
            BagToId[bag] = bagId;
            bag.Accept(new SetVisitor<T>(this, bagId));
        }

        public string GetId<T>(Bag<T> bag)
            => BagToId[bag];

        public Bag GetBag(string id)
            => IdToBag[id];

        private Dictionary<string, Bag> IdToBag { get; } 
            = new Dictionary<string, Bag>();

        private Dictionary<Bag, string> BagToId { get; }
            = new Dictionary<Bag, string>();

        private sealed class SetVisitor<T> : Bag<T>.IVisitor<Aliq.Void>
        {
            public SetVisitor(DataBinding dataBinding, string objectId)
            {
                DataBinding = dataBinding;
                ObjectId = objectId;
            }

            private DataBinding DataBinding { get; }

            private string ObjectId { get; }

            public Aliq.Void Visit<I>(SelectMany<T, I> selectMany)
            {
                DataBinding.Set(selectMany.Input, ObjectId + "_s");
                return new Aliq.Void();
            }

            public Aliq.Void Visit(Merge<T> disjointUnion)
            {
                DataBinding.Set(disjointUnion.InputA, ObjectId + "_a");
                DataBinding.Set(disjointUnion.InputB, ObjectId + "_b");
                return new Aliq.Void();
            }

            public Aliq.Void Visit(ExternalInput<T> externalInput)
                => new Aliq.Void();

            public Aliq.Void Visit(Const<T> const_)
                => new Aliq.Void();

            public Aliq.Void Visit<I>(GroupBy<T, I> groupBy)
            {
                DataBinding.Set(groupBy.Input, ObjectId + "_g");
                return new Aliq.Void();
            }

            public Aliq.Void Visit<A, B>(Product<T, A, B> product)
            {
                DataBinding.Set(product.InputA, ObjectId + "_i");
                DataBinding.Set(product.InputB, ObjectId + "_j");
                return new Aliq.Void();
            }
        }
    }
}
