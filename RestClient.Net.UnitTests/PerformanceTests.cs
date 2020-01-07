
#if (NETCOREAPP3_1)

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class PerformanceTests
    {
        private const int Repeats = 10;
        private const string RestCountriesUrl = "https://restcountries.eu/rest/v2/";

        [TestMethod]
        public async Task GetPerformanceTest()
        {
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(RestCountriesUrl));

            List<RestCountry> countryData = null;

            var startTime = DateTime.Now;

            for (var i = 0; i < Repeats; i++)
            {
                countryData = await countryCodeClient.GetAsync<List<RestCountry>>();
                Assert.IsTrue(countryData.Count > 0);
            }

            var restClientTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestClient Get : Total Milliseconds:{ restClientTotalMilliseconds}");

            startTime = DateTime.Now;
            var restSharpClient = new RestSharp.RestClient(RestCountriesUrl);

            for (var i = 0; i < Repeats; i++)
            {
                var response = await restSharpClient.ExecuteTaskAsync<List<RestCountry>>(new RestRequest(Method.GET));
                Assert.IsTrue(response.Data.Count > 0);
            }

            var restSharpTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestSharp Get : Total Milliseconds:{ restSharpTotalMilliseconds}");

            Assert.IsTrue(restClientTotalMilliseconds < restSharpTotalMilliseconds, "😞 RestSharp wins.");

            Console.WriteLine("🏆 RestClient Wins!!!");
        }

        [TestMethod]
        public async Task PatchPerformanceTest()
        {

            var restClient = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));

            UserPost userPost = null;

            var startTime = DateTime.Now;

            for (var i = 0; i < Repeats; i++)
                userPost = await restClient.PatchAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, "/posts/1");

            var restClientTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestClient Get : Total Milliseconds:{ restClientTotalMilliseconds}");


            startTime = DateTime.Now;
            var restSharpClient = new RestSharp.RestClient("https://jsonplaceholder.typicode.com");

            var request = new RestRequest(Method.PATCH)
            {
                Resource = "/posts/1"
            };

            for (var i = 0; i < Repeats; i++)
            {
                var response = await restSharpClient.ExecuteTaskAsync<UserPost>(request);
                userPost = response.Data;
            }

            var restSharpTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestSharp Get : Total Milliseconds:{ restSharpTotalMilliseconds}");

            Assert.IsTrue(restClientTotalMilliseconds < restSharpTotalMilliseconds, "😞 RestSharp wins.");

            Console.WriteLine("🏆 RestClient Wins!!!");
        }

    }
}

#endif