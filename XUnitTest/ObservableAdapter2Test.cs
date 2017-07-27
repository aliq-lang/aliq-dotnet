using Aliq;
using Aliq.Adapters;
using Aliq.Bags;
using Aliq.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Xunit;

namespace XUnitTest
{
    public class ObservableAdapter2Test
    {
        public IEnumerable<string> Get()
        {
            yield return "a";
            yield return "b";
            yield return "c";
        }

        [Fact]
        public void Test()
        {
            // logic
            var a = new ExternalInput<string>();
            var b = a.Select(v => v + v);
            var r = b.Select(v => v.Length).Average();

            // data
            var aTable = Get().ToArray();

            // back end
            var inMemory = new HotObservableAdapter();

            // binding
            inMemory.SetInput(a, Get().ToObservable());

            // get
            var x = new List<string>();
            var y = new List<string>();
            var newA = inMemory.Get(a);
            newA.Subscribe(Observer.Create<string>(v => x.Add(v)));
            newA.Subscribe(Observer.Create<string>(v => y.Add(v)));

            var z = new List<string>();
            var d = new List<string>();
            var newB = inMemory.Get(b);
            newB.Subscribe(Observer.Create<string>(v => z.Add(v)));
            newB.Subscribe(Observer.Create<string>(v => d.Add(v)));

            var rr = new List<int>();
            var newR = inMemory.Get(r);
            newR.Subscribe(Observer.Create<int>(v => rr.Add(v)));

            inMemory.Start();

            Assert.Equal(z, new[] { "aa", "bb", "cc" });
            Assert.Equal(d, new[] { "aa", "bb", "cc" });
            Assert.Equal(aTable, x);
            Assert.Equal(rr, new[] { 2 });
        }
    }
}
