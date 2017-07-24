using Aliq;
using System.Collections.Generic;
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
        public void TestGroupByIntegersSum()
        {
            // logic
            var a = new ExternalInput<int>();
            var g = a.Aggregate((ai, bi) => ai + bi);

            // data
            var aTable = new[] { 8, 30, 4};

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput(a, aTable);

            // get
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            var item = gNew.First();
            Assert.Equal(42, item);
        }

        [Fact]
        public void TestGroupByDoubleMean()
        {
            // logic
            var a = new ExternalInput<double>();
            var g = a.Average();

            // data
            double[] aTable = (new[] { 8.0, 30.0, 4.0, 18.0 });
            var avgWithLINQ = aTable.Average();
            Assert.Equal(15, avgWithLINQ);

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput<double>(a, aTable);

            // get
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            var item = gNew.First();
            Assert.Equal(14, item);
        }

        [Fact]
        public void MapReduceTest()
        {


            int[] numbers = { 10, 15, 20, 25, 30, 35 };
            //List<int>

            foreach (int number in numbers)
            {
                int curNumber = number + 3;
            }

            // logic
            var a = new ExternalInput<int>();
            var g = a.Aggregate((ai, bi) => ai + bi);



            var result = numbers.GroupBy(n => (n % 10 == 0));
            var max = result.ElementAt(0);
            Assert.Equal(6, numbers.Length);
        }

    }
}
