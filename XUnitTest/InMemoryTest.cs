using Aliq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class InMemoryTest
    {
        public class Store
        {
            public string Name;
            public List<decimal> inventoryTotals;
        }


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
            Assert.Equal(15, item);
        }

        [Fact]
        public void MapReduceTest()
        {
            //initialize stores

            Store[] chain =
            {
                new Store
                {
                    Name= "originalStore",
                    inventoryTotals = new List<decimal>()
                },
                new Store()
                {
                    Name = "secondStore",
                    inventoryTotals = new List<decimal>()
        }
            };



            var result = chain.SelectMany(c => c.inventoryTotals);

            // logic
            var a = new ExternalInput<decimal>();
            var g = a.Aggregate((ai, bi) => ai + bi);

            // data - populate
            chain[0].inventoryTotals.Add((decimal)4.80);
            chain[0].inventoryTotals.Add((decimal)8.80);
            chain[0].inventoryTotals.Add((decimal)9.40);

            chain[1].inventoryTotals.Add((decimal)5.80);
            chain[1].inventoryTotals.Add((decimal)9.80);
            chain[1].inventoryTotals.Add((decimal)12.40);

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput<decimal>(a, chain.SelectMany(c=>c.inventoryTotals));

            // get
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            var item = gNew.First();
            Assert.Equal(51, item);


            Assert.True(true);
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
        public void WhereTest()
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
        }
    }
}
