
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace SoftServerless
{
    public static class Ratings
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<RatingDto>(requestBody);


            var isValid = await Validate(data);
            if (!isValid)
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

        private static async Task<bool> Validate(RatingDto data)
        {
            string getProductUri = $"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={data.ProductId}";
            string getUserUri = $"https://serverlessohlondonuser.azurewebsites.net/api/GetUser?userId={data.UserId}";
            return await ValidateUri(getProductUri) && await ValidateUri(getUserUri);

        }

        private static async Task<bool> ValidateUri(string requestUri)
        {
            var response = await httpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }
    }
}
