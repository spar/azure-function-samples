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
        private static IEnumerable<User> _users;

        private static void InitUsers(ExecutionContext context)
        {
            if (_users != null) return;
            var path = Path.Combine(context.FunctionAppDirectory, "Users.json");
            _users = JsonConvert.DeserializeObject<IEnumerable<User>>(File.ReadAllText(path));
        }

        [FunctionName("FakeRestUserGetAll")]
        public static IActionResult UserGetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/users")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            InitUsers(context);
            return new OkObjectResult(_users);
        }

        [FunctionName("FakeRestUserGet")]
        public static IActionResult UserGet([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/user/{id=id}")]HttpRequest req, int id, TraceWriter log, ExecutionContext context)
        {
            InitUsers(context);
            return new OkObjectResult(_users?.FirstOrDefault(x => x.Id == id));
        }

        [FunctionName("FakeRestUserCreate")]
        public static IActionResult UserCreate([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            try
            {
                InitUsers(context);
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
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            try
            {
                InitUsers(context);
                var reader = new StreamReader(req.Body, Encoding.UTF8);
                var user = JsonConvert.DeserializeObject<User>(reader.ReadToEnd());
                if (_users.Any(x => x.Id == user?.Id))
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