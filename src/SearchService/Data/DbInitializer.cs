using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data
{
    public class DbInitializer
    {

        public static async Task InitDb(WebApplication app)
        {
            // Initialize the connection to the MongoDB database
            await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            // Create the indexes for the Make,Model,Color columns so we could use them for the search functionality
            await DB.Index<Item>()
                .Key(a => a.Make, KeyType.Text)
                .Key(a => a.Model, KeyType.Text)
                .Key(a => a.Color, KeyType.Text)
                .CreateAsync();

            // Check if the database is empty or already populated
            var count = await DB.CountAsync<Item>();

            using var scope = app.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionsServiceHttpClient>();

            // Call the AuctionsService to get the items and insert them into the search database
            var items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine($"Returned Items from the Auction service 😎🤘. Items count: {items.Count}");

            if (items.Count > 0)
            {
                await DB.SaveAsync(items);
                Console.WriteLine("Items inserted successfully into the SearchDb 🚀");
            }
            else
            {
                Console.WriteLine("The SearchDb is already populated with up to date items 🤷‍♂️");
            }
            
        }

    }
}