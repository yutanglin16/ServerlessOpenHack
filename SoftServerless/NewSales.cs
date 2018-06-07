
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Linq;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;

namespace SoftServerless
{
    public static class NewSales
    {
        [FunctionName("NewSales")]

        public static void Run(
            [CosmosDBTrigger("SoftServerless", "Sales", ConnectionStringSetting = "RatingsDb", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [EventHub(
                "asarelay",
                Connection = "SalesEventHub")] out string[] events,
            TraceWriter log)
        {
            var lineItems = documents.Select(d => JObject.Parse(d.ToString())).SelectMany(d => d["details"].Children()).Select(j =>
            {
                return new
                {
                    Type = "sales",
                    Cost = j["totalCost"].Value<string>(),
                    Name = j["productName"].Value<string>()
                };
            }).Select(l => JsonConvert.SerializeObject(l));

            events = lineItems.ToArray();
        }
    }
}
