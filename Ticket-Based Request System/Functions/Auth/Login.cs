using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Ticket_Based_Request_System.Helpers;
using Ticket_Based_Request_System.Services;

namespace Ticket_Based_Request_System.Functions.Auth
{
    public class Login
    {
        private readonly CosmosDbService _cosmos;

        public Login(CosmosDbService cosmos)
        {
            _cosmos = cosmos;
        }

        [Function("Login")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")]
            HttpRequestData req)
        {
            try
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(req.Body);

                string email = body.GetProperty("email").GetString();
                string password = body.GetProperty("password").GetString();

                var query = new Microsoft.Azure.Cosmos.QueryDefinition(
                    "SELECT * FROM c WHERE c.email = @email")
                    .WithParameter("@email", email);

                var iterator = _cosmos.Users.GetItemQueryIterator<dynamic>(query);
                if (!iterator.HasMoreResults)
                    return req.CreateResponse(HttpStatusCode.Unauthorized);

                var user = (await iterator.ReadNextAsync()).FirstOrDefault();
                if (user == null)
                    return req.CreateResponse(HttpStatusCode.Unauthorized);

                if (!PasswordHelper.Verify(password, user.passwordHash.ToString()))
                    return req.CreateResponse(HttpStatusCode.Unauthorized);

                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteAsJsonAsync(new
                {
                    userId = user.id?.ToString(),
                    employeeCode = user.employeeCode?.ToString(),
                    role = user.role?.ToString(),
                    rolePrefix = user.rolePrefix?.ToString(),
                    name = user.name?.ToString(),
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
