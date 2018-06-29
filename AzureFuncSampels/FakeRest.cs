using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AzureFuncSampels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFuncSampels
{
    public static class FakeRest
    {
        private static readonly IEnumerable<User> Users;

        static FakeRest()
        {
            Users = JsonConvert.DeserializeObject<IEnumerable<User>>(File.ReadAllText("Users.json"));
        }

        [FunctionName("FakeRestUserGetAll")]
        public static IActionResult UserGetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/users")]HttpRequest req, TraceWriter log)
        {
            return new OkObjectResult(Users);
        }

        [FunctionName("FakeRestUserGet")]
        public static IActionResult UserGet([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/user/{id=id}")]HttpRequest req, int id, TraceWriter log)
        {
            return new OkObjectResult(Users?.FirstOrDefault(x => x.Id == id));
        }

        [FunctionName("FakeRestUserCreate")]
        public static IActionResult UserCreate([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log)
        {
            try
            {
                var reader = new StreamReader(req.Body, Encoding.UTF8);
                var user = JsonConvert.DeserializeObject<User>(reader.ReadToEnd());
                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("FakeRestUserUpdate")]
        public static IActionResult UserUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "put"
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log)
        {
            try
            {
                var reader = new StreamReader(req.Body, Encoding.UTF8);
                var user = JsonConvert.DeserializeObject<User>(reader.ReadToEnd());
                if (Users.Any(x => x.Id == user?.Id))
                    return new OkObjectResult(user);
                return new NotFoundObjectResult(user);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}