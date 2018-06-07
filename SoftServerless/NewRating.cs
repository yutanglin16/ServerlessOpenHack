
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

namespace SoftServerless
{
    public static class NewRating
    {
        [FunctionName("NewRating")]

        public static void Run(
            [CosmosDBTrigger("SoftServerless", "RatingDb", ConnectionStringSetting = "RatingsDb", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [EventHub(
                "asarelay",
                Connection = "SalesEventHub")] out string[] events,
            TraceWriter log)
        {
            var lineItems = documents.Select(d => JObject.Parse(d.ToString())).Select(j => new
            {
                Type = "sentiment",
                productId = j["productId"],
                score = j["sentimentScore"]
            }).Select(l => JsonConvert.SerializeObject(l));

            events = lineItems.ToArray();
        }
    }
}
