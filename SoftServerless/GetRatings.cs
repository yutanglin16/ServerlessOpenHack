using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SoftServerless
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "SoftServerless",
                collectionName: "RatingDb",
                ConnectionStringSetting = "RatingsDb")]
            DocumentClient docClient,
            TraceWriter log)
        {
            var userId = req.Query["userId"];

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("SoftServerless", "RatingDb");
            
            IDocumentQuery<RatingDto> query = docClient.CreateDocumentQuery<RatingDto>(collectionUri)
                .Where(r => r.UserId.ToString().Equals(userId))
                .AsDocumentQuery();

            var ratingsResult = new List<RatingDto>();
            while (query.HasMoreResults)
            {
                foreach (RatingDto result in await query.ExecuteNextAsync())
                {
                    ratingsResult.Add(result);
                }
            }
            
            return new JsonResult(ratingsResult);
        }
    }
}