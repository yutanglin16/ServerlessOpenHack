
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Serialization;

namespace SoftServerless
{
    public static class Ratings
    {
        [FunctionName("CreateRating")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<RatingDto>(requestBody);

            if (!Validate(data))
            {
                return new BadRequestObjectResult("Request not valid");
            }

            var updatedData = StoreRating(data);

            
            return new JsonResult(updatedData, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
            });
        }

        private static RatingDto StoreRating(RatingDto data)
        {
            data.Id = Guid.NewGuid();
            data.Timestamp = DateTime.UtcNow;
            return data;
        }

        private static bool Validate(RatingDto data)
        {
            return true;
        }
    }
}
