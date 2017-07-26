using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace RemoteBackEnd
{
    public class Program
    {
        public static void Run(
            Assembly assembly, TextReader reader, int nodeId, int nodeCount)
        {
            var logicType = assembly.GetType("Logic");
            var dataBinding = new DataBinding();
            logicType.GetMethod("Init").Invoke(null, new object[] { dataBinding });
            var node = new Node(dataBinding, nodeId, nodeCount);
            while(true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    var split = line.Split(' ');
                    var command = split[0];
                    if (command == "exit")
                    {
                        break;
                    }
                    Console.Error.WriteLine("invalid command: " + line);
                }
            } 
        }

        public static void RunServer(Assembly assembly)
        {
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
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                switch (command)
                {
                    case "node":
                        var nodeId = int.Parse(args[2]);
                        var nodeCount = int.Parse(args[3]);
                        Run(assembly, Console.In, nodeId, nodeCount);
                        break;
                    case "server":
                        RunServer(assembly);
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
