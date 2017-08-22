using Aliq;
using System.Linq;
using System;
using Xunit;
using Aliq.Linq;
using Aliq.Adapters;
using Aliq.Bags;

namespace XUnitTest
{
    public class AzureAliqAdapterTest
    {
        [Fact]
        public void EmployeeTest()
        {
            // ExternalInput allows for late binding
            var msftEmployees = new ExternalInput<Employee>();

            // Logic to find msft youth, veterans and inBetweeners
            var youthLogic = msftEmployees.Where(emp => emp.Age < 30);
            var veteransLogic = msftEmployees.Where(emp => emp.Age > 40);
            var inBetweenersLogic = msftEmployees.Where(emp => emp.Age >= 30 && emp.Age <= 40);
            var averageAgeLogic = msftEmployees.Select(emp => (double)emp.Age).Average();

            // Set up the adapter layer which performs operations on the data
            // var azureAliqAdapter = new AzureAliqAdapter();
            var azureAliqAdapter = new EnumerableAdapter();

            // Data can be anywhere! For example, Azure blob container or an array
            var dataStore = new Employee[] 
            {
                new Employee { Name = "Deepak Shankargouda", Age = 20 },
                new Employee { Name = "Sergey Shandar", Age = 22 },
                new Employee { Name = "Mike Liu", Age = 18 },
                new Employee { Name = "Bob Smith", Age = 31 },
                new Employee { Name = "Mickey Mouse", Age = 89 },
            }; 
            
            azureAliqAdapter.SetInput(msftEmployees, dataStore);

            // Calculate results!
            // logic to find msft youth, veterans and inBetweeners
            var youth = azureAliqAdapter.Get(youthLogic).ToArray();
            var veterans = azureAliqAdapter.Get(veteransLogic).ToArray();
            var inBetweeners = azureAliqAdapter.Get(inBetweenersLogic).ToArray();
            var averageAge = azureAliqAdapter.Get(averageAgeLogic).ToArray();

            Assert.Equal(youth.Length, 3);
            Assert.True(youth.Any(v => v.Name == "Deepak Shankargouda" && v.Age == 20));
            Assert.True(youth.Any(v => v.Name == "Sergey Shandar" && v.Age == 22));
            Assert.True(youth.Any(v => v.Name == "Mike Liu" && v.Age == 18));
            Assert.False(youth.Any(v => v.Name == "Bob Smith" && v.Age == 31));
            Assert.False(youth.Any(v => v.Name == "Mickey Mouse" && v.Age == 89));

            Assert.Equal(veterans.Length, 1);
            Assert.True(veterans.Any(v => v.Name == "Mickey Mouse" && v.Age == 89));

            Assert.Equal(inBetweeners.Length, 1);
            Assert.True(inBetweeners.Any(v => v.Name == "Bob Smith" && v.Age == 31));

            Assert.Equal(averageAge, new[] { 36.0 });
        }
    }

    // Sample data struct
    class Employee
    {
        public String Name { get; set; }
        public String Id { get; set; }
        public int Age { get; set; }
        public String Org { get; set; }
    }

}