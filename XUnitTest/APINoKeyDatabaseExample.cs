using Aliq;
using System.Linq;
using System;
using Xunit;
using Aliq.Linq;
using Aliq.Adapters;
using Aliq.Bags;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XUnitTest
{
    public class AliqAPIAdapterTest
    {



        /// <summary>
        /// This is API data obtained from the Lottery Mega Millions Winning Numbers by the New York State Gaming Commission.
        /// 
        /// This data is freely obtainable from the website:  https://catalog.data.gov/dataset/lottery-mega-millions-winning-numbers-beginning-2002
        /// as part of https://www.data.gov/ .
        /// </summary>
        [Fact]
        public async Task LottoryExampleTest ()
        {
            // ExternalInput allows for late binding
            var lotteryExternalInput = new ExternalInput<Lottos>();

            // Get a list of lottery items and the API resource page
            List<Lottos> lotteryList = new List<Lottos>();
            string page = "https://data.ny.gov/api/views/5xaw-6ayf/rows.json?accessType=DOWNLOAD";

            // Use HTTP Client to retrieve data
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                if (result != null)
                {
                    //Get the JSON data you need
                    JObject m = JObject.Parse(result);
                    var array = m["data"];

                    //Take in and process JSON data
                    foreach (JToken token in array)
                    {
                        Lottos temp = new Lottos(token[8].ToString(), token[9].ToString(), token[10].ToString());
                        lotteryList.Add(temp);
                    }

                    var azureAliqAdapter = new EnumerableAdapter();

                    //Select drawings where the Mega Ball number is 49, as 49 is the square of 7 (a lucky number).
                    var doublyLuckyLogic = lotteryExternalInput.Where(lot => lot.megaBall==49);
                    
                    //Your can use other System.Collection items for data as well, such as Lists, to set to external input.  Apply logic here.
                    azureAliqAdapter.SetInput(lotteryExternalInput, lotteryList);

                    //Get the list matching the logic of having MegaBall number 49.
                    var luckyWinnerCombo = azureAliqAdapter.Get(doublyLuckyLogic).ToArray();
                    Assert.Equal(luckyWinnerCombo.Length, 7);
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2003, 8, 5) && Array.IndexOf(v.winningNumbers, 7) == 0 && Array.IndexOf(v.winningNumbers, 32) == 1 && Array.IndexOf(v.winningNumbers, 34) == 2 && Array.IndexOf(v.winningNumbers, 38) == 3 && Array.IndexOf(v.winningNumbers, 44) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2003, 9, 30) && Array.IndexOf(v.winningNumbers, 2) == 0 && Array.IndexOf(v.winningNumbers, 26) == 1 && Array.IndexOf(v.winningNumbers, 37) == 2 && Array.IndexOf(v.winningNumbers, 40) == 3 && Array.IndexOf(v.winningNumbers, 46) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2003, 12, 30) && Array.IndexOf(v.winningNumbers, 12) == 0 && Array.IndexOf(v.winningNumbers, 18) == 1 && Array.IndexOf(v.winningNumbers, 21) == 2 && Array.IndexOf(v.winningNumbers, 32) == 3 && Array.IndexOf(v.winningNumbers, 46) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2004, 3, 9) && Array.IndexOf(v.winningNumbers, 16) == 0 && Array.IndexOf(v.winningNumbers, 23) == 1 && Array.IndexOf(v.winningNumbers, 29) == 2 && Array.IndexOf(v.winningNumbers, 36) == 3 && Array.IndexOf(v.winningNumbers, 51) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2004, 4, 6) && Array.IndexOf(v.winningNumbers, 8) == 0 && Array.IndexOf(v.winningNumbers, 17) == 1 && Array.IndexOf(v.winningNumbers, 29) == 2 && Array.IndexOf(v.winningNumbers, 32) == 3 && Array.IndexOf(v.winningNumbers, 39) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2004, 4, 20) && Array.IndexOf(v.winningNumbers, 12) == 0 && Array.IndexOf(v.winningNumbers, 22) == 1 && Array.IndexOf(v.winningNumbers, 37) == 2 && Array.IndexOf(v.winningNumbers, 46) == 3 && Array.IndexOf(v.winningNumbers, 48) == 4));
                    Assert.True(luckyWinnerCombo.Any(v => v.megaBall == 49 && v.drawDate == new DateTime(2004, 9, 14) && Array.IndexOf(v.winningNumbers, 12) == 0 && Array.IndexOf(v.winningNumbers, 35) == 1 && Array.IndexOf(v.winningNumbers, 37) == 2 && Array.IndexOf(v.winningNumbers, 38) == 3 && Array.IndexOf(v.winningNumbers, 50) == 4));

                    //Get the 5 oldest entries, since more recent entries will be updated live.  Set logic here accordinly.
                    var oldestDrawLogic = lotteryExternalInput.Where(lot => lot.drawDate < new DateTime(2002, 6, 1));

                    //Apply logic to dataset.
                    var oldestEntries = azureAliqAdapter.Get(oldestDrawLogic).ToArray();

                    Assert.Equal(oldestEntries.Length, 5);
                    Assert.True(oldestEntries.Any(v => v.megaBall == 30 && v.drawDate == new DateTime(2002, 5, 17) && Array.IndexOf(v.winningNumbers, 15) == 0 && Array.IndexOf(v.winningNumbers, 18) == 1 && Array.IndexOf(v.winningNumbers, 25) == 2 && Array.IndexOf(v.winningNumbers, 33) == 3 && Array.IndexOf(v.winningNumbers, 47) == 4));
                    Assert.True(oldestEntries.Any(v => v.megaBall == 9 && v.drawDate == new DateTime(2002, 5, 21) && Array.IndexOf(v.winningNumbers, 4) == 0 && Array.IndexOf(v.winningNumbers, 28) == 1 && Array.IndexOf(v.winningNumbers, 39) == 2 && Array.IndexOf(v.winningNumbers, 41) == 3 && Array.IndexOf(v.winningNumbers, 44) == 4));
                    Assert.True(oldestEntries.Any(v => v.megaBall == 36 && v.drawDate == new DateTime(2002, 5, 24) && Array.IndexOf(v.winningNumbers, 2) == 0 && Array.IndexOf(v.winningNumbers, 4) == 1 && Array.IndexOf(v.winningNumbers, 32) == 2 && Array.IndexOf(v.winningNumbers, 44) == 3 && Array.IndexOf(v.winningNumbers, 52) == 4));
                    Assert.True(oldestEntries.Any(v => v.megaBall == 24 && v.drawDate == new DateTime(2002, 5, 28) && Array.IndexOf(v.winningNumbers, 6) == 0 && Array.IndexOf(v.winningNumbers, 21) == 1 && Array.IndexOf(v.winningNumbers, 22) == 2 && Array.IndexOf(v.winningNumbers, 29) == 3 && Array.IndexOf(v.winningNumbers, 32) == 4));
                    Assert.True(oldestEntries.Any(v => v.megaBall == 47 && v.drawDate == new DateTime(2002, 5, 31) && Array.IndexOf(v.winningNumbers, 12) == 0 && Array.IndexOf(v.winningNumbers, 28) == 1 && Array.IndexOf(v.winningNumbers, 45) == 2 && Array.IndexOf(v.winningNumbers, 46) == 3 && Array.IndexOf(v.winningNumbers, 52) == 4));
                }
                else
                {
                    Assert.False(true);
                }

            }
        }

        public class Lottos
        {
            public DateTime drawDate;
            public int[] winningNumbers;
            public int megaBall;

            public Lottos(string drawDateString, string winningNumbersString, string megaBallString)
            {
                this.drawDate = DateTime.Parse(drawDateString);

                List<string> stringList = winningNumbersString.Split(' ').ToList();
                winningNumbers = stringList.Select(i => Convert.ToInt32(i)).ToArray(); ;

                this.megaBall = Convert.ToInt32(megaBallString);
            }

        }

    }

}
