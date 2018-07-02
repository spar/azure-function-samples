using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFuncSamples
{
    public static class OpenWeatherMap
    {
        private static readonly HttpClient HttpClient;

        private static readonly string GetUrl =
            $"http://api.openweathermap.org/data/2.5/weather?APPID={System.Environment.GetEnvironmentVariable("OpenWeatherMapKey")}&q=";

        static OpenWeatherMap()
        {
            HttpClient = new HttpClient();
        }

        [FunctionName("OpenWeatherMap")]
        public static IActionResult Search([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "weather/{q=q}")]HttpRequest req, string q, TraceWriter log)
        {
            if (string.IsNullOrEmpty(q))
                return new BadRequestObjectResult("Please provide a valid city name");
            var url = $"{GetUrl}{q}";
            return new OkObjectResult(HttpClient.GetAsync(url)?.Result?.Content.ReadAsAsync(typeof(object)));
        }
    }
}