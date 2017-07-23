using Aliq;
using System;
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
            var g = a.GroupBy(key => 0, (ai, bi) => ai + bi);

            // data
            var aTable = new[] { "Hello", "world", "!" };

            // back end
            var inMemory = new InMemory();

            // binding
            inMemory.SetInput(a, aTable);

            // get
            var gNew = inMemory.Get(g);           
        }
    }
}
