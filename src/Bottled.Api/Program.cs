using Bottled.Api.Dtos;
using Bottled.Api.Infrastructure.Data;
using Bottled.Api.Infrastructure.Filters;
using Bottled.Api.Models;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var _connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");

if (_connectionString != null)
{
    builder.Services.AddEntityFrameworkMySQL().AddDbContext<BottledContext>(options =>
    {
        options.UseMySQL(_connectionString);
    });
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<BottledContext>();

    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", async (BottledContext context) =>
{
    var rand = new Random();

    int skipper = rand.Next(0, context.Messages.Count());

    var randomMessage = await context.Messages
        .Skip(skipper)
        .Take(1)
        .SingleOrDefaultAsync();

    if (randomMessage == null)
    {
        return Results.NoContent();
    }

    var messageDto = new MessageDto()
    {
        Author = randomMessage?.Author,
        Content = randomMessage?.Content
    };

    return Results.Ok(messageDto);
})
.WithName("GetRandomMessage")
.WithSummary("Break a bottle and read the message")
.WithOpenApi()
.AddEndpointFilter<AddDummyHeaderFilter>();

app.MapPost("/write", async (MessageDto messageDto, BottledContext context) =>
{

    var message = await context.AddAsync(new Message()
    {
        Author = messageDto.Author,
        Content = messageDto.Content
    });

    await context.SaveChangesAsync();

    return Results.Ok(message.Entity.Id);
})
.WithName("WriteMessage")
.WithSummary("Write a message and dispatch into the ocean")
.WithOpenApi()
.AddEndpointFilter<AddDummyHeaderFilter>();

app.Run();

public partial class Program { }