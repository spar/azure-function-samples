using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFuncSamples
{
    public static class EpochConverter
    {
        private static readonly DateTime Epoch;

        static EpochConverter()
        {
            Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        [FunctionName("EpochConverter")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "epoch-convert/{value=value}")]HttpRequest req, double value, TraceWriter log)
        {
            var result = new List<KeyValue>();
            var utcTime = Epoch.AddMilliseconds(value);
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                result.Add(new KeyValue
                {
                    Timezone = tz.DisplayName,
                    Value = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz).ToString(CultureInfo.InvariantCulture)
                });
            }
            return new OkObjectResult(result.OrderBy(x => x.Timezone));
        }

        private class KeyValue
        {
            public string Timezone { get; set; }
            public string Value { get; set; }
        }
    }
}