using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SixLabors.ImageSharp;
using Path = System.IO.Path;

namespace AzureFuncSamples.ImageResize
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizeByPercentage")]
        public static IActionResult ImageResizeByPercentage([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "imageresize/{percent=percent}")]HttpRequest req, int percent, TraceWriter log)
        {
            if (percent >= 100 | percent < 1)
                return new BadRequestObjectResult("Percentage value must be less than 100 and greater than 0");
            return new FileContentResult(ImageResizeFunctions.GenerateThumbnailByPercentage(req.Body, percent), "image/jpeg");
        }

        [FunctionName("ImageResizeByWidthHeight")]
        public static IActionResult ImageResizeByWidthHeight([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "imageresize/{w=w}/{h=h}")]HttpRequest req, int w, int h, TraceWriter log)
        {
            using (var imageStream = Image.Load(req.Body))
            {
                if ((w <= 0 || w > imageStream.Width) || (h <= 0 || h > imageStream.Height))
                    return new BadRequestObjectResult("Provide proper width and height");
                return new FileContentResult(ImageResizeFunctions.GenerateThumbnailByWidthHeight(imageStream, w, h), "image/jpeg");
            }
        }

        [FunctionName("ImageResizeOriginalSample")]
        public static IActionResult ImageResizeOriginalSample([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "imageresize/orig-sample")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            var sample = File.ReadAllBytes(Path.Combine(context.FunctionAppDirectory, "Images", "Whale_shark_Georgia_aquarium.jpg"));
            return new FileContentResult(sample, "image/jpeg");
        }

        [FunctionName("ImageResizeThumbSample")]
        public static IActionResult ImageResizeThumbSample([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "imageresize/thumb-sample/{percent=percent}")]HttpRequest req, int percent, TraceWriter log, ExecutionContext context)
        {
            if (percent >= 100 | percent < 1)
                return new BadRequestObjectResult("Percentage value must be less than 100 and greater than 0");
            var sample = File.ReadAllBytes(Path.Combine(context.FunctionAppDirectory, "Images", "Whale_shark_Georgia_aquarium.jpg"));
            return new FileContentResult(ImageResizeFunctions.GenerateThumbnailByPercentage(new MemoryStream(sample), percent), "image/jpeg");
        }

        [FunctionName("ImageResizeAvatar")]
        public static IActionResult ImageResizeAvatar([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "imageresize/avatar/{w=w}/{h=h}/{r=r}")]HttpRequest req, int w, int h, float r, TraceWriter log)
        {
            using (var imageStream = Image.Load(req.Body))
            {
                if ((w <= 0 || w > imageStream.Width) || (h <= 0 || h > imageStream.Height))
                    return new BadRequestObjectResult("Provide proper width and height");
                return new FileContentResult(ImageResizeFunctions.GenerateAvatar(imageStream, w, h, r), "image/png");
            }
        }
    }
}