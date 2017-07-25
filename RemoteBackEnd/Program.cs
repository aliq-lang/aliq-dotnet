using System;
using System.Reflection;
using System.Runtime.Loader;

namespace RemoteBackEnd
{
    public class Program
    {
        public static void Run(Assembly assembly)
        {
            var logicType = assembly.GetType("Logic");
            var dataBinding = new DataBinding();
            logicType.GetMethod("Init").Invoke(null, new object[] { dataBinding });
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
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                Run(assembly);
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