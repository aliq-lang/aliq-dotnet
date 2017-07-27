using Aliq;
using Aliq.Bags;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RemoteBackEnd
{
    public class Server : IDataBinding
    {
        public Server(IEnumerable<INode> nodes)
        {
            Nodes = nodes.ToImmutableList();
        }

        private ImmutableList<INode> Nodes { get; }

        public void Run()
        {
            foreach (var (id, bag) in InputOutputList)
            {
                bag.Accept(new Visitor(this, string.Empty));
            }
        }

        private sealed class Visitor : Bag.IVisitor<Aliq.Void>
        {
            public Visitor(Server server, string indent)
            {
                Server = server;
                Indent = indent;
            }

            public Aliq.Void Visit<T>(Bag<T> bag)
            {
                Server.Produce(bag, Indent);
                return new Aliq.Void();
            }

            private Server Server { get; }

            private string Indent;
        }

        private void Produce<T>(Bag<T> bag, string indent)
        {
            var id = DataBinding.GetId<T>(bag);
            if (!StateSet.Contains(id))
            {
                Console.WriteLine(indent + id);
                bag.Accept(new Visitor<T>(this, indent + "  "));
            }
        }

        private sealed class Visitor<T> : Bag<T>.IVisitor<Aliq.Void>
        {
            public Visitor(Server server, string indent)
            {
                Server = server;
                Indent = indent;
            }

            private Server Server { get; }

            private string Indent { get; }

            public Aliq.Void Visit<I>(SelectMany<T, I> selectMany)
            {
                Server.Produce(selectMany.Input, Indent);
                return new Aliq.Void();
            }

            public Aliq.Void Visit(Merge<T> disjointUnion)
            {
                Server.Produce(disjointUnion.InputA, Indent);
                Server.Produce(disjointUnion.InputB, Indent);
                return new Aliq.Void();
            }

            public Aliq.Void Visit(ExternalInput<T> externalInput)
                => new Aliq.Void();

            public Aliq.Void Visit(Const<T> const_)
                => new Aliq.Void();

            public Aliq.Void Visit<I>(GroupBy<T, I> groupBy)
            {
                Server.Produce(groupBy.Input, Indent);
                return new Aliq.Void();
            }

            public Aliq.Void Visit<A, B>(Product<T, A, B> product)
            {
                Server.Produce(product.InputA, Indent);
                Server.Produce(product.InputB, Indent);
                return new Aliq.Void();
            }
        }

        public void Set<T>(Bag<T> bag, string objectId)
        {
            DataBinding.Set(bag, objectId);
            InputOutputList.Add((objectId, bag));
        }

        private DataBinding DataBinding { get; } 
            = new DataBinding();

        private HashSet<string> StateSet { get; }
            = new HashSet<string>();

        private List<(string Id, Bag Bag)> InputOutputList { get; } 
            = new List<(string, Bag)>();
    }
}
