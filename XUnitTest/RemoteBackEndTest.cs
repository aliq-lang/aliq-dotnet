using RemoteBackEnd;
using System.IO;
using System.Reflection;
using Xunit;

namespace XUnitTest
{
    public class RemoteBackEndTest
    {
        [Fact]
        void TestNoArguments()
        {
            var exitCode = Program.Main(new string[0]);
            Assert.Equal(exitCode, -1);
        }

        [Fact]
        void TestBusinessLogic()
        {
            var assembly = typeof(Logic).GetTypeInfo().Assembly;
            Program.Run(assembly, new StringReader("exit"), 0, 1);
        }
    }
}
