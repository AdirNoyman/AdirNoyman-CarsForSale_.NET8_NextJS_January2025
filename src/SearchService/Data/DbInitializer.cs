using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

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

            if (count == 0)
            {
                Console.WriteLine("Database is empty. Populating the database with sample data 🤓...");

                var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                // Deserialize the JSON data into a list of Item objects (POCO - Plain Old C# Object)
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                // Save the items to the database
                await DB.SaveAsync(items);
            }
        }

    }
}