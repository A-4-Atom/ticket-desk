using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using Ticket_Based_Request_System.Models;
using Ticket_Based_Request_System.Services;

namespace Ticket_Based_Request_System.Functions.Tickets
{
    public class CreateTicket
    {
        private readonly CosmosDbService _cosmos;

        public CreateTicket(CosmosDbService cosmos)
        {
            _cosmos = cosmos;
        }

        [Function("CreateTicket")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tickets")]
            HttpRequestData req)
        {
            try
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(req.Body);

                string userId = body.GetProperty("userId").GetString();
                string employeeCode = body.GetProperty("employeeCode").GetString();
                string role = body.GetProperty("role").GetString();
                string rolePrefix = body.GetProperty("rolePrefix").GetString();

                string title = body.GetProperty("title").GetString();
                string description = body.GetProperty("description").GetString();
                string category = body.GetProperty("category").GetString();

                var counterResponse = await _cosmos.Counters.ReadItemAsync<dynamic>(
                    rolePrefix,
                    new PartitionKey("ticket"));

                int nextNumber = counterResponse.Resource.currentValue + 1;
                counterResponse.Resource.currentValue = nextNumber;

                await _cosmos.Counters.ReplaceItemAsync(
                    counterResponse.Resource,
                    rolePrefix,
                    new PartitionKey("ticket"));

                string confirmationNumber = $"{rolePrefix}-{nextNumber:D5}";

                var ticket = new Ticket
                {
                    confirmationNumber = confirmationNumber,
                    userId = userId,
                    employeeCode = employeeCode,
                    role = role,
                    title = title,
                    description = description,
                    category = category,
                    status = "Open"
                };

                await _cosmos.Tickets.CreateItemAsync(
                    ticket,
                    new PartitionKey(userId));

                var res = req.CreateResponse(HttpStatusCode.Created);
                await res.WriteAsJsonAsync(ticket);
                return res;
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
