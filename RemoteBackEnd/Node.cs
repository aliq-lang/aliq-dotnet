using Aliq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace RemoteBackEnd
{
    sealed class Node : INode
    {
        public Node(
            DataBinding dataBinding,
            IDataStorage dataStorage,
            INodes nodes,
            int nodeId, 
            int nodeCount)
        {
            DataBinding = dataBinding;
            DataStorage = dataStorage;
            Nodes = nodes;
            NodeId = nodeId;
            NodeCount = nodeId;
        }

        private INodes Nodes { get; }

        private int NodeId { get; }

        private int NodeCount { get; }

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

            private static void AddRecord<I>(
                Dictionary<string,I> dictionary, (string Key, I Value) record, Func<I, I, I> reduce)
            {
                var result = dictionary.TryGetValue(record.Key, out var old)
                    ? reduce(old, record.Value)
                    : record.Value;
                dictionary[record.Key] = result;
            }

            public Aliq.Void Visit<I>(GroupBy<T, I> groupBy)
            {
                var array = Enumerable
                    .Range(0, Node.NodeCount)
                    .Select(_ => new Dictionary<string, I>())
                    .ToImmutableList();

                var reduce = groupBy.Reduce;
                var input = Node.Get(groupBy.Input);
                foreach (var record in input)
                {
                    var nodeId = Node.GetNodeId(record.Item1);
                    AddRecord(array[nodeId], record, reduce);
                }
                // send data
                var main = array[Node.NodeId];
                foreach (var (v, i) in array
                    .Select((v, i) => (v, i))
                    .Where(x => x.Item2 != Node.NodeId))
                {
                    Node.Nodes.SendData(i, Id, v.SelectValueTuples());
                }
                array = null;
                // recieve data
                foreach (var i in Enumerable
                    .Range(0, Node.NodeCount)
                    .Where(i => i != Node.NodeId))
                {
                    foreach(var record in Node.Nodes.RecieveData<I>(i, Id))
                    {
                        AddRecord(main, record, reduce);
                    }
                }
                Node.Save(Id, main.SelectValueTuples().Select(groupBy.GetResult));
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

        public void Create(string bagId)
        {
            DataBinding.GetBag(bagId).Accept(new CreateVisitor(this, bagId));
        }

        private sealed class CreateVisitor : Bag.IVisitor<Aliq.Void>
        {
            public CreateVisitor(Node node, string id)
            {
                Node = node;
                Id = id; 
            }

            private Node Node { get; }

            private string Id { get; }

            public Aliq.Void Visit<T>(Bag<T> bag)
                => bag.Accept(new CreateVisitor<T>(Node, Id));
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
