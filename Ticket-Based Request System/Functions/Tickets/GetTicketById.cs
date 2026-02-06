using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Concurrent;
using System.Net;
using Ticket_Based_Request_System.Models;
using Ticket_Based_Request_System.Services;

namespace Ticket_Based_Request_System.Functions.Tickets
{
    public class GetTicketById
    {
        private readonly CosmosDbService _cosmos;

        public GetTicketById(CosmosDbService cosmos)
        {
            _cosmos = cosmos;
        }

        [Function("GetTicketById")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tickets/{id}")]
            HttpRequestData req,
            string id)
        {
            string userId = req.Query["userId"];

            try
            {
                var response = await _cosmos.Tickets.ReadItemAsync<Ticket>(
                    id,
                    new PartitionKey(userId));

                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteAsJsonAsync(response.Resource);
                return res;
            }
            catch (CosmosException)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
