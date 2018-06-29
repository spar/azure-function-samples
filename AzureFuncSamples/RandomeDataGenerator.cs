using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFuncSamples
{
    public static class RandomeDataGenerator
    {
        private static readonly Random Rand = new Random();

        [FunctionName("RandomeDataGenerator")]
        public static IActionResult Random([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "random/{type=type}")]HttpRequest req, string type, TraceWriter log)
        {
            switch (type)
            {
                case "int":
                    return new OkObjectResult(RandomInt());

                case "double":
                    return new OkObjectResult(RandomDouble());

                case "char":
                    return new OkObjectResult(RandomChar());

                default:
                    return new BadRequestObjectResult("Provide a proper data type");
            }
        }

        private static int RandomInt()
        {
            return Rand.Next(int.MinValue, int.MaxValue);
        }

        private static double RandomDouble()
        {
            return Rand.NextDouble();
        }

        private static char RandomChar()
        {
            return (char)Rand.Next(0, 127);
        }
    }
}