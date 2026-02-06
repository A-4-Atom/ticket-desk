using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Ticket_Based_Request_System.Models;
using Ticket_Based_Request_System.Services;
using Ticket_Based_Request_System.Helpers;

namespace Ticket_Based_Request_System.Functions.Auth
{
    public class Signup
    {
        private readonly CosmosDbService _cosmos;

        public Signup(CosmosDbService cosmos)
        {
            _cosmos = cosmos;
        }

        [Function("Signup")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/signup")]
            HttpRequestData req)
        {
            try
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(req.Body);

                string name = body.GetProperty("name").GetString();
                string email = body.GetProperty("email").GetString();
                string password = body.GetProperty("password").GetString();
                string role = body.GetProperty("role").GetString();

                string rolePrefix = role switch
                {
                    "Sales Rep" => "W",
                    "SVP" => "X",
                    "IT Manager" => "Y",
                    _ => null
                };

                if (rolePrefix == null)
                {
                    var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                    await bad.WriteStringAsync("Invalid role");
                    return bad;
                }

                var counter = await _cosmos.Counters.ReadItemAsync<dynamic>(
                    "employeeCode",
                    new Microsoft.Azure.Cosmos.PartitionKey("employee"));

                int nextCode = counter.Resource.currentValue + 1;
                counter.Resource.currentValue = nextCode;

                await _cosmos.Counters.ReplaceItemAsync(
                    counter.Resource,
                    "employeeCode",
                    new Microsoft.Azure.Cosmos.PartitionKey("employee"));

                var user = new User
                {
                    name = name,
                    email = email,
                    employeeCode = nextCode.ToString("D3"),
                    role = role,
                    rolePrefix = rolePrefix,
                    passwordHash = PasswordHelper.HashPassword(password)
                };

                await _cosmos.Users.CreateItemAsync(
                    user,
                    new Microsoft.Azure.Cosmos.PartitionKey(email));

                var res = req.CreateResponse(HttpStatusCode.Created);
                await res.WriteAsJsonAsync(new
                {
                    user.id,
                    user.employeeCode,
                    user.name,
                    user.email,
                    user.role
                });

                return res;
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
