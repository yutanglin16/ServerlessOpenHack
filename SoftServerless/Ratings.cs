
using System.IO;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SoftServerless
{
    public static class Ratings
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, 
            [CosmosDB(
                databaseName: "SoftServerless",
                collectionName: "RatingDb",
                ConnectionStringSetting = "RatingsDb")] IAsyncCollector<RatingDto> docs,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<RatingDto>(requestBody);

            var isValid = await Validate(data);
            if (!isValid)
            {
                return new BadRequestObjectResult("Request not valid");
            }

            data.SentimentScore = await GetSentimentRating(data.UserNotes, log);

            var updatedData = GenerateMetadata(data);
            
            await docs.AddAsync(updatedData);

            log.LogInformation("ratingId={ratingId}, sentimentScore={sentimentScore}", updatedData.Id, updatedData.SentimentScore);

            return new JsonResult(updatedData);
        }

        private static async Task<double> GetSentimentRating(string userNotes, ILogger logger)
        {
            var value = new
            {
                documents = new object[]
                {
                    new {
                        language = "en",
                        id = "1",
                        text = userNotes
                    }
                }
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"),
            };

            request.Headers.Add("Ocp-Apim-Subscription-Key", "73e84a39f9c24babb1f30cd2a82a94f3");
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            logger.LogInformation(responseContent);

            return JObject.Parse(responseContent)["documents"][0]["score"].Value<double>();
        }

        private static RatingDto GenerateMetadata(RatingDto data)
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

    internal class SentimentRequest
    {
    }
}
