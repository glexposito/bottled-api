using Bottled.Api.Dtos;
using Bottled.Api.Filters;
using Bottled.Api.Infrastructure.Data;
using Bottled.Api.Models;
using Bottled.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddDbContext<BottledContext>(options => { options.UseInMemoryDatabase("BottledDatabase"); });

builder.Services.AddScoped<IValidator<MessageDto>, MessageDtoValidator>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

var minApi = app.MapGroup("/api")
    .WithOpenApi()
    .WithTags("Message in a bottle");


minApi.MapGet("/", GetRandomMessage)
    .WithName("GetRandomMessage")
    .WithSummary("Break a bottle and read the message")
    .Produces(404)
    .Produces<MessageDto>();

minApi.MapPost("/write", WriteMessage)
    .AddEndpointFilter<ValidationFilter<MessageDto>>()
    .WithName("WriteMessage")
    .WithSummary("Write a message and dispatch it into the ocean")
    .Produces(400)
    .Produces(200);

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

    var messageDto = new MessageDto(randomMessage.Author, randomMessage.Content);

    return TypedResults.Ok(messageDto);
}

static async Task<IResult> WriteMessage(IValidator<MessageDto> validator, BottledContext context, MessageDto messageDto)
{
    await context.AddAsync
    (
        new Message()
        {
            Author = messageDto.Author,
            Content = messageDto.Content
        }
    );

    await context.SaveChangesAsync();

    return TypedResults.Ok();
}

public abstract partial class Program;