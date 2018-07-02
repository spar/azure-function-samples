using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFuncSamples
{
    public static class WikipediaSearch
    {
        private static readonly HttpClient HttpClient;

        private static readonly string WikiUrl =
            "https://en.wikipedia.org/w/api.php?format=json&action=opensearch&limit=10&profile=fuzzy&search=";

        static WikipediaSearch()
        {
            HttpClient = new HttpClient();
        }

        [FunctionName("WikipediaSearch")]
        public static IActionResult Search([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "wikipedia/{q=q}")]HttpRequest req, string q, TraceWriter log)
        {
            if (string.IsNullOrEmpty(q))
                return new BadRequestObjectResult("Please provide a search term");
            var url = $"{WikiUrl}{q}";
            return new OkObjectResult(HttpClient.GetAsync(url)?.Result?.Content?.ReadAsStringAsync());
        }
    }
}