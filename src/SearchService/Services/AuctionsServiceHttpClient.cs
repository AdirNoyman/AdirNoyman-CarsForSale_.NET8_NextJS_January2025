using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionsServiceHttpClient(HttpClient httpClient, IConfiguration config)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;


        // Get auctions from the Auctions service, that are newer than the last updated date in the Search service
        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdatedDateInSearchSvc = await DB.Find<Item, string>()
            .Sort(x => x.Descending(y => y.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

            return await _httpClient.GetFromJsonAsync<List<Item>>($"{_config["AuctionsServiceUrl"]}/api/auctions?date={lastUpdatedDateInSearchSvc}");
        }
    }
}