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

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput<double>(a, aTable);

            // get
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            var item = gNew.First();
            Assert.Equal(15, item);
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


        [Fact]
        public void WhereTestString()
        {
            var a = new ExternalInput<string>();
            var g = a.Where(v => (v.StartsWith("M")));

            //
            var aData = new string[] { "Mike", "Sergey", "Mandar" };

            //
            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(2, gNew.Count());
            Assert.True(gNew.Contains("Mike"));
            Assert.True(gNew.Contains("Mandar"));
            Assert.False(gNew.Contains("Sergey"));
        }

        public void WhereTestNumber()
        {
            var a = new ExternalInput<int>();
            var g = a.Where(v => (v > 7  && v % 4 == 0));

            //
            var aData = new int[] { 12, 9, 3, 4, 8 };

            //
            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(2, gNew.Count());
            Assert.False(gNew.Contains(12));
            Assert.True(gNew.Contains(8));
        }


        [Fact]
        public void GroupingTest()
        {
            var a = new ExternalInput<double>();
            var g = a.GroupBy(v => v.ToString(), _ => 1, (ai, bi) => ai + bi);
 

            //data
            var aData = new double[] { 6, 6, 5, 7,7,7 };

            //backend
            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(3, gNew.Count());
            Assert.True(gNew.Any(v=> v.Item1 == "5" && v.Item2 == 1));
            Assert.True(gNew.Any(v => v.Item1 == "6" && v.Item2 == 2));
            Assert.True(gNew.Any(v => v.Item1 == "7" && v.Item2 == 3));
        }
        

    }
}
