using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Linq;
using System.Threading.Tasks;

namespace SoftServerless
{
    public static class Sales
    {
        // [FunctionName("Sales")]
        public static async Task Run([EventHubTrigger("sales", Connection = "SalesEventHub")]string[] myEventHubMessages, 
            [CosmosDB(
                databaseName: "SoftServerless",
                collectionName: "Sales",
                ConnectionStringSetting = "RatingsDb")] IAsyncCollector<string> docs, 
            TraceWriter log)
        {
            foreach (var message in myEventHubMessages)
            {
                await docs.AddAsync(message);
            }
        }
    }
}
    