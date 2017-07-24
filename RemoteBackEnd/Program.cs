using System;
using System.Runtime.Loader;

namespace RemoteBackEnd
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("no arguments");
                return -1;
            }
            var dllName = args[0];
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllName);
            return 0;
        }
    }
}