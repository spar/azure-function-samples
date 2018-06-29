using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace AzureFuncSampels
{
    public static class ImageResize
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [Obsolete]
        [FunctionName("ImageResize")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "image-resize/{imageUrl=imageUrl}")]HttpRequest req, string imageUrl, TraceWriter log)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return new BadRequestObjectResult("No Image Url provided");
            log.Info($"Trying to resize {imageUrl}");

            using (var result = HttpClient.GetAsync(imageUrl).Result)
            {
                if (!result.IsSuccessStatusCode) return new BadRequestErrorMessageResult("Something went wrong!");
                var image = Image.Load(result.Content.ReadAsByteArrayAsync().Result);
                image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
                var returnImageStream = new MemoryStream();
                image.Save(returnImageStream, new JpegEncoder());
                return new FileContentResult(returnImageStream.ToArray(), "image/jpeg");
            }
        }
    }
}