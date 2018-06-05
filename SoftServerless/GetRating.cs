using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SoftServerless
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "SoftServerless",
                collectionName: "RatingDb",
                ConnectionStringSetting = "RatingsDb", Id = "{Query.ratingId}")]
            RatingDto ratingDocs,
            TraceWriter log)
        {
            return new JsonResult(ratingDocs);
        }
    }
}