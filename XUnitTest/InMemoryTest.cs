using Aliq;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class InMemoryTest
    {
        [Fact]
        public void TestSetInput()
        {
            // logic
            var a = new ExternalInput<string>();

            // data
            var aTable = new[] { "Hello", "world", "!" };

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput(a, aTable);

            // get
            var newA = inMemory.Get(a);
            Assert.Equal(aTable, newA);
        }

        [Fact]
        public void TestGroupBy()
        {
            // logic
            var a = new ExternalInput<string>();
            var g = a.Aggregate((ai, bi) => ai + bi);

            // data
            var aTable = new[] { "Hello", "world", "!" };

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput(a, aTable);

            // get
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            var item = gNew.First();
            Assert.Equal("Helloworld!".Length, item.Length);
        }

        [Fact]
        public void TestAverage()
        {
            var a = new ExternalInput<double>();
            var g = a.Average();

            //
            var aData = new double[] { 3, 4, 5 };

            //
            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(4.0, gNew.First());
        }
    }
}
