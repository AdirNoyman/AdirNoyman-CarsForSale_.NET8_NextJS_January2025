using System.Net;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionsServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

// Register the application started event. This makes sure that even if the db initialization fails, the application will still start, because it will skip this step.

app.Lifetime.ApplicationStarted.Register(async () =>
{
    // Initialize the MongoDB database including seeding if the database is empty
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception error)
    {
        Console.WriteLine("An error occurred while initializing the database 😫", error);
    }
});

app.Run();

// Handle transient HTTP failures
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        // Retry every 3 seconds until the call is successful
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}
