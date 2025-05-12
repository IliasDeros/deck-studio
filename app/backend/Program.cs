using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

app.UseCors();
app.UseHttpsRedirection();

// In-memory storage for Job Specs and conversation state
object? latestJobSpec = null;
var conversationStates = new ConcurrentDictionary<string, ConversationState>();

app.MapPost("/ai", ([FromBody] UserMessageWithId message) =>
{
    // If no ID, generate one
    string convId = message.Id ?? Guid.NewGuid().ToString();
    var state = conversationStates.GetOrAdd(convId, _ => new ConversationState("start"));
    string response = "";
    object? jobSpec = null;

    switch (state.Step)
    {
        case "start":
            if (message.Message.Trim().Equals("I want bank data", StringComparison.OrdinalIgnoreCase))
            {
                conversationStates[convId] = state with { Step = "awaiting_bank" };
                response = "Which bank?";
            }
            else
            {
                response = "Please say 'I want bank data' to begin.";
            }
            break;
        case "awaiting_bank":
            conversationStates[convId] = state with { Step = "awaiting_data_type", Bank = message.Message.Trim() };
            response = "What do you need: transactions, balances, or both?";
            break;
        case "awaiting_data_type":
            if (string.IsNullOrEmpty(state.Bank))
            {
                response = "Please specify a bank first.";
                break;
            }
            if (message.Message.Trim().Equals("Transactions", StringComparison.OrdinalIgnoreCase))
            {
                jobSpec = new {
                    type = "bank-data-request",
                    bank = state.Bank,
                    data = "transactions",
                    date_range = new { from = "2024-01-01", to = "2024-01-31" },
                    user_id = "user-123"
                };
                conversationStates[convId] = state with { Step = "awaiting_confirm", PendingJobSpec = jobSpec };
                response = "Here is your job spec. Type 'confirm' to submit.";
            }
            else
            {
                response = "Sorry, only 'Transactions' is supported in this demo.";
            }
            break;
        case "awaiting_confirm":
            // Remove ability to confirm via message; only allow via /job endpoint
            response = "Review your job spec and use the Confirm button to submit.";
            jobSpec = state.PendingJobSpec;
            break;
        case "done":
            response = "This conversation is complete. Start a new one to create another job.";
            break;
        default:
            response = "Sorry, I didn't understand that.";
            break;
    }

    return Results.Ok(new { response, jobSpec, id = convId });
});

app.MapPost("/job", ([FromBody] object jobSpec) =>
{
    // Add current date and time to job spec
    var now = DateTime.UtcNow;
    var jobSpecWithTimestamp = new
    {
        jobSpec,
        submitted_at = now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
    };
    latestJobSpec = jobSpecWithTimestamp;
    return Results.Ok(new { status = "Job Spec received" });
});

app.MapGet("/latest", () =>
{
    if (latestJobSpec is null)
        return Results.NotFound();
    return Results.Ok(latestJobSpec);
});

app.Run();

record UserMessageWithId(string Message, string? Id);
record ConversationState(string Step, string? Bank = null, object? PendingJobSpec = null);