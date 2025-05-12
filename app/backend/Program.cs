using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// In-memory storage for Job Specs
var jobSpecs = new ConcurrentQueue<object>();
object? latestJobSpec = null;

// Simple rule-based AI logic (mocked)
List<(string user, string ai)> aiFlow = new()
{
    ("I want bank data", "Which bank?"),
    ("RBC", "What do you need: transactions, balances, or both?"),
    ("Transactions", "Job Spec created."),
};

app.MapPost("/ai", ([FromBody] UserMessage message) =>
{
    // Simple rule-based response
    string response = aiFlow.FirstOrDefault(x => x.user.Equals(message.Message, StringComparison.OrdinalIgnoreCase)).ai
        ?? "Sorry, I didn't understand that.";
    return Results.Ok(new { response });
});

app.MapPost("/job", ([FromBody] object jobSpec) =>
{
    latestJobSpec = jobSpec;
    jobSpecs.Enqueue(jobSpec);
    return Results.Ok(new { status = "Job Spec received" });
});

app.MapGet("/latest", () =>
{
    if (latestJobSpec is null)
        return Results.NotFound();
    return Results.Ok(latestJobSpec);
});

app.MapGet("/", () => Results.Ok("Backend is running"));

app.Run();

record UserMessage(string Message);
