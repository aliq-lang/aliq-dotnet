using Aliq;
using System.Linq;
using System;
using Xunit;
using Aliq.Linq;

namespace XUnitTest
{
    public class AzureAliqAdapterTest
    {
        [Fact]
        public void Test()
        {
            // ExternalInput allows for late binding
            var msftEmployees = new ExternalInput<Employee>();

            // Logic to find msft youth, veterans and inBetweeners
            var youthLogic = msftEmployees.Where(emp => emp.Age < 30);
            var veteransLogic = msftEmployees.Where(emp => emp.Age > 40);
            var inBetweenersLogic = msftEmployees.Where(emp => emp.Age >= 30 && emp.Age <= 40);
            var averageAgeLogic = msftEmployees.Select(emp => emp.Age).Average();

            // Set up the adapter layer which performs operations on the data
            // var azureAliqAdapter = new AzureAliqAdapter();
            var azureAliqAdapter = new EnumerableAdapter();

            // Data can be anywhere!
            var dataStore = new Employee[] { }; // Azure blob container
            azureAliqAdapter.SetInput(msftEmployees, dataStore);

            // Calculate results!
            // logic to find msft youth, veterans and inBetweeners
            var youth = azureAliqAdapter.Get(youthLogic);
            var veterans = azureAliqAdapter.Get(veteransLogic);
            var inBetweeners = azureAliqAdapter.Get(inBetweenersLogic);
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