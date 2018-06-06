
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace SoftServerless
{
    public static class OrderFileParse
    {
        [FunctionName("OrderFileParse")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, 
            [Blob("orders/{Query.timestamp}-OrderHeaderDetails.csv", FileAccess.Read, Connection = "OrdersDb")] string orderHeaderDetailsStream,
            [Blob("orders/{Query.timestamp}-OrderLineItems.csv", FileAccess.Read, Connection = "OrdersDb")] string orderLineItemsStream,
            [Blob("orders/{Query.timestamp}-ProductInformation.csv", FileAccess.Read, Connection = "OrdersDb")] string productInformationStream,

            TraceWriter log)
        {
            log.Info(orderHeaderDetailsStream);
            log.Info(orderLineItemsStream);
            log.Info(productInformationStream);

            return new OkResult();
        }
    }
}
