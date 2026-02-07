using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Ticket_Based_Request_System.Services
{
    public class CosmosDbService
    {
        private readonly CosmosClient _client;
        private readonly Database _database;

        public CosmosDbService(IConfiguration configuration)
        {
            try
            {
                _client = new CosmosClient(
                    configuration["CosmosDb:Endpoint"],
                    configuration["CosmosDb:Key"]
                );

                _database = _client.GetDatabase(
                    configuration["CosmosDb:DatabaseName"]
                );
            }
            catch
            {
                throw;
            }
        }

        public Container Users =>
            _database.GetContainer("Users");

        public Container Tickets =>
            _database.GetContainer("Tickets");

        public Container Counters =>
            _database.GetContainer("Counters");
    }
}
