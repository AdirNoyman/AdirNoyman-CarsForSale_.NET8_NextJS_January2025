using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

// Initialize the MongoDB database including seeding if the database is empty
try
{
    await DbInitializer.InitDb(app);
}
catch (Exception error)
{
    
    Console.WriteLine("An error occurred while initializing the database 😫", error);
}

app.Run();
