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
            Program.RunNode(assembly, new StringReader("exit"), 0, 1);
        }

        
        [Fact]
        void TestDataWriterString()
        {
            string path = "MikeLiuStringTest.txt";

            using (FileDataWriter<string> s = new FileDataWriter<string>(path))
            {
                s.Append("Hello World");
                Assert.True(File.Exists(path));
                s.Append("Goodbye World");
            }

            string sample = File.ReadAllText(path);
            Assert.Equal("\"Hello World\"\0\"Goodbye World\"\0", sample);
            File.Delete(path);
        }
        
        [Fact]
        void TestDataWriterNumber()
        {
            string path = "MikeLiuNumberTest.txt";

            using (FileDataWriter<int> s = new FileDataWriter<int>(path))
            {
                s.Append(1);
                s.Append(2);
                s.Append(3);
                s.Append(4);
                s.Append(5);
                Assert.True(File.Exists(path));
            }

            string sample = File.ReadAllText(path);
            Assert.Equal("1\02\03\04\05\0", sample);
            File.Delete(path);
        }

        [Fact]
        void WriteOutReadIn()
        {
            string path = "MikeLiuBidirectional.txt";

            using (FileDataWriter<string> w = new FileDataWriter<string>(path))
            {
                w.Append("{First: 'Mike', Last: 'Liu'}");
                w.Append("{First: 'Sergey', Last: 'Shandar'}");
                Assert.True(File.Exists(path));
            }

            string sampleText = File.ReadAllText(path);
            Assert.Equal("\"{First: 'Mike', Last: 'Liu'}\"\0\"{First: 'Sergey', Last: 'Shandar'}\"\0", sampleText);

            //now read using the functionality for FileDatareader

            using (FileDataReader<string> r = new FileDataReader<string>(path))
            {
                (bool, string) currentTuple = r.Read();
                Assert.Equal(true, currentTuple.Item1);
                Assert.Equal("{First: 'Mike', Last: 'Liu'}", currentTuple.Item2);
                
                currentTuple = r.Read();
                Assert.Equal(true, currentTuple.Item1);
                Assert.Equal("{First: 'Sergey', Last: 'Shandar'}", currentTuple.Item2);
                
                currentTuple = r.Read();
                Assert.Equal(false, currentTuple.Item1);
                Assert.Equal(null, currentTuple.Item2);
                
            }

            File.Delete(path);
        }
    }
}
