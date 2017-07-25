using Aliq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace RemoteBackEnd
{
    sealed class Node
    {
        public Node(DataBinding dataBinding, int nodeId, int nodeCount)
        {
            DataBinding = dataBinding;
            NodeId = nodeId;
            NodeCount = nodeId;
        }

        private int NodeId;

        private int NodeCount;

        public DataBinding DataBinding { get; }

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

        public Node(IDataStorage dataStorage)
        {
            DataStorage = dataStorage;
        }

        private void Save<T>(string id, IEnumerable<T> list)
        {
            using (var writer = DataStorage.Create<T>(id))
            {
                foreach (var item in list)
                {
                    writer.Append(item);
                }
            }
        }

        private IEnumerable<T> Load<T>(string id)
        {
            using (var reader = DataStorage.Open<T>(id))
            {
                while (true)
                {
                    var (hasItem, value) = reader.Read();
                    if (!hasItem) break;
                    yield return value;
                }                
            }
        }

        private sealed class CreateVisitor<T> : Bag<T>.IVisitor<Aliq.Void>
        {
            public CreateVisitor(Node node, string id)
            {
                Node = node;
                Id = id;
            }

            private Node Node { get; }

            private string Id { get; }

            public Aliq.Void Visit<I>(SelectMany<T, I> selectMany)
            {
                Node.Save(Id, Node.GetSelectMany(selectMany));
                return new Aliq.Void();
            }

            public Aliq.Void Visit(DisjointUnion<T> disjointUnion)
            {
                Node.Save(Id, Node.GetDisjointUnion(disjointUnion));
                return new Aliq.Void();
            }

            public Aliq.Void Visit(ExternalInput<T> externalInput)
            {
                // this should never happen!
                return new Aliq.Void();
            }

            public Aliq.Void Visit(Const<T> const_)
            {
                Node.Save(Id, Node.GetConst(const_, Id));
                return new Aliq.Void();
            }

            public Aliq.Void Visit<I>(GroupBy<T, I> groupBy)
            {
                var result = Node.Get(groupBy.Input).GroupBy(
                    v => v.Item1,
                    v => v.Item2,
                    (key, list) => (key, list.Aggregate(groupBy.Reduce)));
                var writerList = Enumerable
                    .Range(0, Node.NodeCount)
                    .Select(i => Node.DataStorage.Create<(string, I)>(Id + "_" + i))
                    .ToImmutableList();
                try
                {
                    foreach (var item in result)
                    {
                        writerList[Node.GetNodeId(item.Item1)].Append(item);
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

        private int GetNodeId(string key)
            => key.GetHashCode() % NodeCount;

        private IEnumerable<T> GetConst<T>(Const<T> const_, string id)
        {
            if (GetNodeId(id) == NodeId)
            {
                yield return const_.Value;
            }
        }

        private IEnumerable<T> Get<T>(Bag<T> bag)
        {
            var id = DataBinding.GetId(bag);
            return DataStorage.Exist(id)
                ? Load<T>(id)
                : bag.Accept(new GetVisitor<T>(this, id));
        }

        private sealed class GetVisitor<T> : Bag<T>.IVisitor<IEnumerable<T>>
        {
            public GetVisitor(Node node, string id)
            {
                Node = node;
                Id = id;
            }

            private Node Node { get; }
            private string Id { get; }

            public IEnumerable<T> Visit<I>(SelectMany<T, I> selectMany)
                => Node.GetSelectMany(selectMany);

            public IEnumerable<T> Visit(DisjointUnion<T> disjointUnion)
                => Node.GetDisjointUnion(disjointUnion);

            public IEnumerable<T> Visit(ExternalInput<T> externalInput)
            {
                throw new Exception("no external input data");
            }

            public IEnumerable<T> Visit(Const<T> const_)
                => Node.GetConst(const_, Id);

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
