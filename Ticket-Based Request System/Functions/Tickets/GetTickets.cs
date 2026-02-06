using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Cosmos;
using Ticket_Based_Request_System.Services;
using Ticket_Based_Request_System.Models;

namespace Ticket_Based_Request_System.Functions.Tickets
{
    public class GetTickets
    {
        private readonly CosmosDbService _cosmos;

        public GetTickets(CosmosDbService cosmos)
        {
            _cosmos = cosmos;
        }

        [Function("GetTickets")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tickets")]
            HttpRequestData req)
        {
            try
            {
                string userId = req.Query["userId"];

                var query = new QueryDefinition(
                    "SELECT * FROM c WHERE c.userId = @uid ORDER BY c.createdAt DESC")
                    .WithParameter("@uid", userId);

                var iterator = _cosmos.Tickets.GetItemQueryIterator<Ticket>(query);

                var results = new List<Ticket>();

                while (iterator.HasMoreResults && results.Count < 10)
                {
                    var page = await iterator.ReadNextAsync();
                    results.AddRange(page);
                }

                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteAsJsonAsync(results.Take(10));
                return res;
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
