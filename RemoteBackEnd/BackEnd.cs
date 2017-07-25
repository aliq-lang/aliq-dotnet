using Aliq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace RemoteBackEnd
{
    sealed class BackEnd
    {
        public BackEnd(int nodeId, int nodeCount)
        {
            NodeId = nodeId;
            NodeCount = nodeId;
        }

        private int NodeId;

        private int NodeCount;

        public DataBinding DataBinding { get; }
            = new DataBinding();

        /// <summary>
        /// Create partial data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bag"></param>
        public void Create<T>(Bag<T> bag)
        {
            var id = DataBinding.GetId(bag);
            if (DataStorage.Exist(id))
            {
                return;
            }
            bag.Accept(new CreateVisitor<T>(this, id));
        }

        public BackEnd(IDataStorage dataStorage)
        {
            DataStorage = dataStorage;
        }

        private sealed class CreateVisitor<T> : Bag<T>.IVisitor<Aliq.Void>
        {
            public CreateVisitor(BackEnd backEnd, string id)
            {
                BackEnd = backEnd;
                Id = id;
            }

            private BackEnd BackEnd { get; }

            private string Id { get; }

            public Aliq.Void Visit<I>(SelectMany<T, I> selectMany)
            {
                BackEnd.DataStorage.Save(Id, BackEnd.GetSelectMany(selectMany));
                return new Aliq.Void();
            }

            public Aliq.Void Visit(DisjointUnion<T> disjointUnion)
            {
                BackEnd.DataStorage.Save(Id, BackEnd.GetDisjointUnion(disjointUnion));
                return new Aliq.Void();
            }

            public Aliq.Void Visit(ExternalInput<T> externalInput)
            {
                // this should never happen!
                return new Aliq.Void();
            }

            public Aliq.Void Visit(Const<T> const_)
            {
                BackEnd.DataStorage.Save(Id, BackEnd.GetConst(const_, Id));
                return new Aliq.Void();
            }

            public Aliq.Void Visit<I>(GroupBy<T, I> groupBy)
            {
                var result = BackEnd.Get(groupBy.Input).GroupBy(
                    v => v.Item1,
                    v => v.Item2,
                    (key, list) => (key, list.Aggregate(groupBy.Reduce)));
                var writerList = Enumerable
                    .Range(0, BackEnd.NodeCount)
                    .Select(i => BackEnd.DataStorage.Create<(string, I)>(Id + "_" + i))
                    .ToImmutableList();
                try
                {
                    foreach (var item in result)
                    {
                        writerList[BackEnd.GetNode(item.Item1)].Append(item);
                    }
                }
                finally
                {
                    foreach(var writer in writerList)
                    {
                        writer.Dispose();
                    }
                }
                return new Aliq.Void();
            }

            public Aliq.Void Visit<A, B>(Product<T, A, B> product)
            {
                throw new NotImplementedException();
            }
        }

        private IDataStorage DataStorage { get; }

        private IEnumerable<T> GetSelectMany<T, I>(SelectMany<T, I> selectMany)
            => Get(selectMany.Input).SelectMany(selectMany.Func);

        private IEnumerable<T> GetDisjointUnion<T>(DisjointUnion<T> disjointUnion)
        {
            var a = Get(disjointUnion.InputA);
            var b = Get(disjointUnion.InputB);
            return a.Concat(b);
        }

        private int GetNode(string key)
            => key.GetHashCode() % NodeCount;

        private IEnumerable<T> GetConst<T>(Const<T> const_, string id)
        {
            if (GetNode(id) == NodeId)
            {
                yield return const_.Value;
            }
        }

        private IEnumerable<T> Get<T>(Bag<T> bag)
        {
            var id = DataBinding.GetId(bag);
            return DataStorage.Exist(id)
                ? DataStorage.Read<T>(id)
                : bag.Accept(new GetVisitor<T>(this, id));
        }

        private sealed class GetVisitor<T> : Bag<T>.IVisitor<IEnumerable<T>>
        {
            public GetVisitor(BackEnd backEnd, string id)
            {
                BackEnd = backEnd;
                Id = id;
            }

            private BackEnd BackEnd { get; }
            private string Id { get; }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
                => BackEnd.GetSelectMany(selectMany);

            public IEnumerable<T> Visit(DisjointUnion<T> disjointUnion)
                => BackEnd.GetDisjointUnion(disjointUnion);

            public IEnumerable<T> Visit(ExternalInput<T> externalInput)
            {
                throw new Exception("no external input data");
            }

            public IEnumerable<T> Visit(Const<T> const_)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<T> Visit<I>(GroupBy<T, I> groupBy)
            {
                throw new Exception("groupBy data is not ready");
            }

            public IEnumerable<T> Visit<A, B>(Product<T, A, B> product)
            {
                throw new Exception("product data is not ready");
            }
        }
    }
}
