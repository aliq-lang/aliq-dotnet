using Aliq;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace RemoteBackEnd
{
    public class Program
    {
        public static void InitDataBinding(Assembly assembly, IDataBinding dataBinding)
            => assembly
                .GetType("Logic")
                .GetMethod("Init")
                .Invoke(null, new object[] { dataBinding });

        private static Node CreateNode(Assembly assembly, int nodeId, int nodeCount)
        {
            var dataBinding = new DataBinding();
            InitDataBinding(assembly, dataBinding);
            return new Node(dataBinding, null, null, nodeId, nodeCount);
        }

        public static void RunNode(
            Assembly assembly, TextReader reader, int nodeId, int nodeCount)
        {
            var node = CreateNode(assembly, nodeId, nodeCount);
            while(true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    Console.Error.WriteLine("unexpected end of stream");
                }
                var split = line.Split(' ');
                var command = split[0];
                if (command == "exit")
                {
                    break;
                }
                Console.Error.WriteLine("invalid command: " + line);
            } 
        }

        public static void RunServer(Assembly assembly, int nodesCount)
        {
            var nodes = Enumerable
                .Range(0, nodesCount)
                .Select(i => CreateNode(assembly, i, nodesCount));
            var server = new Server(nodes);
            InitDataBinding(assembly, server);
            server.Run();
        }

        public static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.Error.WriteLine("error: no arguments");
                    return -1;
                }
                var command = args[0];
                var dllPath = args[1];
                var nodeCount = int.Parse(args[2]);
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                switch (command)
                {
                    case "node":
                        var nodeId = int.Parse(args[3]);
                        RunNode(assembly, Console.In, nodeId, nodeCount);
                        break;
                    case "server":
                        RunServer(assembly, nodeCount);
                        break;
                    default:
                        Console.Error.WriteLine("unknown command: " + command);
                        break;
                }                                               
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }
        }
    }
}
