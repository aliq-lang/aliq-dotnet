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

        public static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.Error.WriteLine("error: no arguments");
                    return -1;
                }
                var dllPath = args[0];
                var nodeId = int.Parse(args[1]);
                var nodeCount = int.Parse(args[2]);
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                Run(assembly, Console.In, nodeId, nodeCount);
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