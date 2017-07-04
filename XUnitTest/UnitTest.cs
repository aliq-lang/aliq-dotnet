using System;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class UnitTest
    {
        [Fact]
        public void AverageTest()
        {
            var x = (new[] { 1.0, 3.0 }).Average();
            Assert.Equal(2.0, x);
            Assert.Throws<InvalidOperationException>(
                () => (new double[0]).Average());
            var n = (new double?[] { }).Average();
            Assert.Null(n);
        }
    }
}
