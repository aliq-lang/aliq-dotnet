using Aliq;
using Aliq.Adapters;
using Aliq.Bags;
using Aliq.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class EnumerableAdapterTest
    {
        [Fact]
        public void TestSetInput()
        {
            // logic
            var a = new ExternalInput<string>();

            // data
            var aTable = new[] { "Hello", "world", "!" };

            // back end
            var inMemory = new EnumerableAdapter();

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
            var inMemory = new EnumerableAdapter();

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
            var aTable = new[] { 8, 30, 4 };

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
            var inMemory = new EnumerableAdapter();

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
            var g = a.Where(v => (v > 7 && v % 4 == 0));

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
            var aData = new double[] { 6, 6, 5, 7, 7, 7 };

            //backend
            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(3, gNew.Count());
            Assert.True(gNew.Any(v => v.Item1 == "5" && v.Item2 == 1));
            Assert.True(gNew.Any(v => v.Item1 == "6" && v.Item2 == 2));
            Assert.True(gNew.Any(v => v.Item1 == "7" && v.Item2 == 3));
        }


        [Fact]
        public void SelectManyTest()
        {
            char[][] multiNumbers = new char[][] { new[] { 'a', 'a' }, new[] { 'b', 'c' }, new[] { 'd', 'd' } };

            var a = new ExternalInput<char[]>();
            var g = a.SelectMany(v => v).Where(character => character < 'd');

            var aData = new char[][] { new[] { 'a', 'a' }, new[] { 'b', 'c' }, new[] { 'd', 'd' } };

            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(4, gNew.Count());
            Assert.True(gNew.Contains('a'));
            Assert.True(gNew.Contains('b'));
            Assert.True(gNew.Contains('c'));
            Assert.False(gNew.Contains('d'));
        }

        class PetOwner
        {
            public string Name;
            public List<string> PetNames;
        }


        [Fact]
        public void SelectManyFilterGroupTest()
        {
            PetOwner[] owners =
            { new PetOwner { Name = "Mike", PetNames = new List<string>{"Fluffy", "Mittens" } },
              new PetOwner { Name = "Ramit", PetNames = new List<string>{ "Roger","Muffy", "Fluffy"} }
            };

            var a = new ExternalInput<List<string>>();
            var g = a.SelectMany(v => v).GroupBy(v => (string.IsNullOrEmpty(v) ? "" : v.Substring(0, 1)), _ => 1, (ai, bi) => ai + bi).Where(v => v.Item1 == "M");

            var aData = owners.Select(v => v.PetNames);

            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);
            Assert.Equal(1, gNew.Count());
            Assert.True(gNew.Any(v => v.Item1 == "M" && v.Item2 == 2));
        }



        class InventoryItem
        {
            public string ItemName;
            public int NumberOfItems;
            public string StoreName;
            public string City;
        }


        [Fact]
        public void InventoryTotalByItem()
        {

            InventoryItem[] inventory =
            {
                new InventoryItem { ItemName = "Socks", NumberOfItems = 5, StoreName = "Walmart", City = "Bellevue"},
                new InventoryItem { ItemName = "Cheezits", NumberOfItems = 2, StoreName = "Walmart", City = "Kirkland"},
                new InventoryItem { ItemName = "Socks", NumberOfItems = 9, StoreName = "Walgreen", City = "Bellevue" },
                new InventoryItem { ItemName = "Shirts", NumberOfItems = 3, StoreName = "Walgreen", City = "Kirkland"},
                new InventoryItem { ItemName = "Beer", NumberOfItems = 8, StoreName = "Walgreen", City = "Kirkland"},
            };


            var a = new ExternalInput<InventoryItem>();
            var g = a.GroupBy(v => (v.ItemName.ToString()), ai => ai.NumberOfItems, (ai, bi) => ai + bi);

            var aData = inventory;

            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);

            Assert.Equal(4, gNew.Count());
            Assert.True(gNew.Any(v => v.Item1 == "Beer" && v.Item2 == 8));
            Assert.True(gNew.Any(v => v.Item1 == "Socks" && v.Item2 == 14));
            Assert.True(gNew.Any(v => v.Item1 == "Shirts" && v.Item2 == 3));
            Assert.True(gNew.Any(v => v.Item1 == "Cheezits" && v.Item2 == 2));
        }

        [Fact]
        public void InventoryTotalByStoreAndCity()
        {

            InventoryItem[] inventory =
            {
                new InventoryItem { ItemName = "Socks", NumberOfItems = 5, StoreName = "Walmart", City = "Bellevue"},
                new InventoryItem { ItemName = "Cheezits", NumberOfItems = 2, StoreName = "Walmart", City = "Kirkland"},
                new InventoryItem { ItemName = "Socks", NumberOfItems = 9, StoreName = "Walgreen", City = "Bellevue" },
                new InventoryItem { ItemName = "Shirts", NumberOfItems = 3, StoreName = "Walgreen", City = "Kirkland"},
                new InventoryItem { ItemName = "Beer", NumberOfItems = 8, StoreName = "Walgreen", City = "Kirkland"},
            };


            var a = new ExternalInput<InventoryItem>();
            var g = a.GroupBy(v => (v.StoreName.ToString() + ";" + v.City.ToString()), ai => ai.NumberOfItems, (ai, bi) => ai + bi);

            var aData = inventory;

            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);

            Assert.Equal(4, gNew.Count());
            Assert.True(gNew.Any(v => v.Item1 == "Walmart;Bellevue" && v.Item2 == 5));
            Assert.True(gNew.Any(v => v.Item1 == "Walmart;Kirkland" && v.Item2 == 2));
            Assert.True(gNew.Any(v => v.Item1 == "Walgreen;Bellevue" && v.Item2 == 9));
            Assert.True(gNew.Any(v => v.Item1 == "Walgreen;Kirkland" && v.Item2 == 11));
        }




        class BankAccount
        {
            public string BankName;
            public decimal CashAmount;
            public string City;
            public string State;
        }


        [Fact]
        public void BankTotals()
        {
            BankAccount[] banks =
{
                new BankAccount { BankName = "Mike", CashAmount=(decimal)380.8, City="Chicago", State="IL"},
                new BankAccount { BankName = "Sergey", CashAmount=(decimal)480.8, City="Seattle", State="WA"},
                new BankAccount { BankName = "BankOfAmerica", CashAmount=(decimal)1480.8, City="Chicago", State="IL"},
                new BankAccount { BankName = "CapitalOne", CashAmount=(decimal)1280.8, City="Fairfax", State="VA"},
                new BankAccount { BankName = "JPMorgan", CashAmount=(decimal)980.8, City="Seattle", State="WA"},
                new BankAccount { BankName = "WellsFargo", CashAmount=(decimal)180.8, City="LosAngeles", State="CA"}
            };

            var a = new ExternalInput<BankAccount>();
            var g = a.GroupBy(v => (v.City.ToString() + ";" + v.State.ToString()), ai => ai.CashAmount, (ai, bi) => ai + bi);

            var aData = banks;

            var inMemory = new InMemory();

            inMemory.SetInput(a, aData);
            var gNew = inMemory.Get(g);

            Assert.Equal(4, gNew.Count());
            Assert.True(true);
            Assert.True(gNew.Any(v => v.Item1 == "Chicago;IL" && v.Item2 == (decimal)1861.6));
            Assert.True(gNew.Any(v => v.Item1 == "Seattle;WA" && v.Item2 == (decimal)1461.6));
            Assert.True(gNew.Any(v => v.Item1 == "Fairfax;VA" && v.Item2 == (decimal)1280.8));
            Assert.True(gNew.Any(v => v.Item1 == "LosAngeles;CA" && v.Item2 == (decimal)180.8));
        }

        public class UniversityYearData
        {
            public int Year;
            public string YearString;
            public int OverallStudents;
            public int HighIncome;
            public int LowIncome;
            public int Independent;
            public int Dependent;
        }
    }
}
