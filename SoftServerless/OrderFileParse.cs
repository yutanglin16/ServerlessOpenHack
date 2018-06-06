
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Linq;
using System.Threading.Tasks;

namespace SoftServerless
{
    public static class OrderFileParse
    {
        [FunctionName("OrderFileParse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, 
            [Blob("orders/{Query.timestamp}-OrderHeaderDetails.csv", FileAccess.Read, Connection = "OrdersDb")] string orderHeaderDetails,
            [Blob("orders/{Query.timestamp}-OrderLineItems.csv", FileAccess.Read, Connection = "OrdersDb")] string orderLineItems,
            [Blob("orders/{Query.timestamp}-ProductInformation.csv", FileAccess.Read, Connection = "OrdersDb")] string productInformation,
            [CosmosDB(
                databaseName: "SoftServerless",
                collectionName: "Orders",
                ConnectionStringSetting = "RatingsDb")] IAsyncCollector<Order> docs,

            TraceWriter log)
        {
            log.Info(orderHeaderDetails);
            log.Info(orderLineItems);
            log.Info(productInformation);

            var orderHeaders = CsvSerializer.DeserializeFromString<IEnumerable<OrderHeaderDetails>>(orderHeaderDetails);
            var orderItems = CsvSerializer.DeserializeFromString<IEnumerable<OrderLineItems>>(orderLineItems);
            var products = CsvSerializer.DeserializeFromString<IEnumerable<ProductInformation>>(productInformation);

            foreach (var item in orderItems)
            {
                item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            }

            var orders = orderHeaders.Select(o => new Order
            {
                id = o.PoNumber,
                Header = o,
                LineItems = orderItems.Where(i => i.PoNumber == o.PoNumber)
            });

            foreach (var order in orders)
            {
                await docs.AddAsync(order);
            }
            
            return new OkResult();
        }
    }
}
