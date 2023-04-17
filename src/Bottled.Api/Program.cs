using Bottled.Api.Dtos;
using Bottled.Api.Infrastructure.Data;
using Bottled.Api.Infrastructure.Filters;
using Bottled.Api.Models;
using Bottled.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IValidator<MessageDto>, MessageDtoValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<BottledContext>();

    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var minApi = app.MapGroup("/api")
    .AddEndpointFilter<AddApiKeyAuthFilter>()
    .WithOpenApi();

minApi.MapGet("/", async (BottledContext context) =>
{
    var rand = new Random();

    int skipper = rand.Next(0, context.Messages.Count());

    var randomMessage = await context.Messages
        .Skip(skipper)
        .Take(1)
        .SingleOrDefaultAsync();

    if (randomMessage == null)
    {
        return Results.NotFound();
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
.Produces(401)
.Produces(404)
.Produces<MessageDto>();

minApi.MapPost("/write", async (IValidator<MessageDto> validator, BottledContext context, MessageDto messageDto) =>
{
    var validationResult = await validator.ValidateAsync(messageDto);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

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
.Produces(400)
.Produces(401)
.Produces<int>();

app.Run();

public partial class Program { }