using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AzureFuncSamples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFuncSamples.FakeRestFunction
{
    public static class FakeRest
    {
        private static IEnumerable<User> _users;

        static FakeRest()
        {
            _users = new List<User>
            {
                new User
                {
                    Id= 1,
                    FirstName= "Jaye",
                    LastName="Tumility",
                    Email="jtumility0@nature.com",
                    Gender= "Male",
                    Avatar= "https://robohash.org/velcommodisoluta.jpg?size=150x150&set=set1"
                },
                new User
                {
                    Id= 2,
                    FirstName= "Winifield",
                    LastName="Donat",
                    Email="wdonat1@behance.net",
                    Gender= "Male",
                    Avatar= "https://robohash.org/velcommodisoluta.jpg?size=150x150&set=set1"
                },
                new User
                {
                    Id= 3,
                    FirstName= "Deana",
                    LastName="Ritmeier",
                    Email="dritmeier2@booking.com",
                    Gender= "Female",
                    Avatar= "https://robohash.org/velcommodisoluta.jpg?size=150x150&set=set1"
                },
                new User
                {
                    Id= 4,
                    FirstName= "Brade",
                    LastName="Westwick",
                    Email="bwestwick3@npr.org",
                    Gender= "Male",
                    Avatar= "https://robohash.org/velcommodisoluta.jpg?size=150x150&set=set1"
                },
                new User
                {
                    Id= 5,
                    FirstName= "Marcille",
                    LastName="Cinnamond",
                    Email="mcinnamond4@icio.us",
                    Gender= "Female",
                    Avatar= "https://robohash.org/velcommodisoluta.jpg?size=150x150&set=set1"
                }
            };
        }

        [FunctionName("FakeRestUserGetAll")]
        public static IActionResult UserGetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/users")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            return new OkObjectResult(_users);
        }

        [FunctionName("FakeRestUserGet")]
        public static IActionResult UserGet([HttpTrigger(AuthorizationLevel.Anonymous, "get"
            , Route = "fakerest/user/{id=id}")]HttpRequest req, int id, TraceWriter log, ExecutionContext context)
        {
            return new OkObjectResult(_users?.FirstOrDefault(x => x.Id == id));
        }

        [FunctionName("FakeRestUserCreate")]
        public static IActionResult UserCreate([HttpTrigger(AuthorizationLevel.Anonymous, "post"
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log, ExecutionContext context)
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
            , Route = "fakerest/user")]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            try
            {
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

        [FunctionName("FakeRestUserDelete")]
        public static IActionResult UserDelete([HttpTrigger(AuthorizationLevel.Anonymous, "delete"
            , Route = "fakerest/user/{id=id}")]HttpRequest req, int id, TraceWriter log, ExecutionContext context)
        {
            return new OkResult();
        }
    }
}