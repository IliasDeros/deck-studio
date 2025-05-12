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

// Add controller support
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AiController).Assembly)
    .AddApplicationPart(typeof(JobController).Assembly)
    .AddApplicationPart(typeof(LatestController).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();

public record UserMessageWithId(string Message, string? Id);
public record ConversationState(string Step, string? Bank = null, object? PendingJobSpec = null);