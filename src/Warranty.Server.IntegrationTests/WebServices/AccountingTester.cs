using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Accounting.Client;
using NUnit.Framework;

namespace Warranty.Server.IntegrationTests.WebServices
{

    [TestFixture, Explicit]
    public class AccountingTester
    {
        [Test]
        public void ClientSmokeTest()
        {
            var client = new AccountingClient(new AccountingClientConfiguration("http://accountingtest.davidweekleyhomes.com"));
            //client.Get.Community("6010000");
            // This doesn't exist yet but leaving it here because we'll need to test it out later.
        }

        [Test]
        public void SmokeTest()
        {
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Og==");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.dwh.accounting-v1+json"));
                var result = client.GetAsync("http://accountingtest.davidweekleyhomes.com/api/community?communitynumber=60010000").Result;
                result.EnsureSuccessStatusCode();

                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            }
            
        }     
    }
}