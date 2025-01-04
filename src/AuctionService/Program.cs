using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapControllers();

// Seed the database
try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{

    Console.WriteLine("Failed to seed the database 😫", e);
}

app.Run();
