using Bottled.Api.Dtos;
using Bottled.Api.Infrastructure.Data;
using Bottled.Api.Infrastructure.Filters;
using Bottled.Api.Models;
using Bottled.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");

if (connectionString != null)
{
    builder.Services.AddDbContextPool<BottledContext>(options => options
        .UseMySql(
            connectionString,
            new MySqlServerVersion(ServerVersion.AutoDetect(connectionString))
        )
    );
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

minApi.MapGet("/", GetRandomMessage)
    .WithName("GetRandomMessage")
    .WithSummary("Break a bottle and read the message")
    .Produces(401)
    .Produces(404)
    .Produces<MessageDto>();

minApi.MapPost("/write", WriteMessage)
    .WithName("WriteMessage")
    .WithSummary("Write a message and dispatch into the ocean")
    .Produces(400)
    .Produces(401)
    .Produces<int>();

app.Run();

static async Task<IResult> GetRandomMessage(BottledContext context)
{
    var rand = new Random();

    var skipper = rand.Next(0, context.Messages.Count());

    var randomMessage = await context.Messages
        .OrderBy(x => x.Id)
        .Skip(skipper)
        .Take(1)
        .SingleOrDefaultAsync();

    if (randomMessage == null)
    {
        return TypedResults.NotFound();
    }

    var messageDto = new MessageDto()
    {
        Author = randomMessage.Author,
        Content = randomMessage.Content
    };

    return TypedResults.Ok(messageDto);
}

static async Task<IResult> WriteMessage(IValidator<MessageDto> validator, BottledContext context, MessageDto messageDto)
{
    var validationResult = await validator.ValidateAsync(messageDto);

    if (!validationResult.IsValid)
    {
        return TypedResults.ValidationProblem(validationResult.ToDictionary());
    }

    var message = await context.AddAsync(new Message()
    {
        Author = messageDto.Author,
        Content = messageDto.Content
    });

    await context.SaveChangesAsync();

    return TypedResults.Ok(message.Entity.Id);
}

public abstract partial class Program
{
}