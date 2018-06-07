
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

namespace SoftServerless
{
    public static class NewOrder
    {
        [FunctionName("NewOrder")]
        
        public static void Run(
            [CosmosDBTrigger("SoftServerless", "Orders", ConnectionStringSetting = "RatingsDb", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [EventHub(
                "asarelay",
                Connection = "SalesEventHub")] out string[] events,
            TraceWriter log)
        {
            foreach (var document in documents)
            {
                var order = JsonConvert.DeserializeObject<Order>(document.ToString());
            }

            var lineItems = documents.Select(d => JsonConvert.DeserializeObject<Order>(d.ToString()).LineItems.Select(l => new
            {
                Type = "order",
                Cost = l.TotalCost,
                Name = l.Product.ProductName
            })).Select(l => JsonConvert.SerializeObject(l));

            events = lineItems.ToArray();
        }
    }
}
