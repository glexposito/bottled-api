using System.Net;
using System.Net.Http.Json;
using Bottled.Api.Infrastructure.Filters;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bottled.Tests;

public class ValidationFilterTests
{
    [Fact]
    public async void ValidationFilter_WhenValidationErrors_ShouldReturn400BadRequest()
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

        var app = builder.Build();
        app.MapPost("/", (Person person) => Results.Ok())
            .AddEndpointFilter<ValidationFilter<Person>>();

        const string baseUrl = "http://localhost:3046";

        _ = app.RunAsync(baseUrl);

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);

        var response = await client.PostAsJsonAsync("/", new Person());

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async void ValidationFilter_WhenNoValidationErrors_ShouldReturn200OK()
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

        var app = builder.Build();
        app.MapPost("/", (Person person) => Results.Ok())
            .AddEndpointFilter<ValidationFilter<Person>>();

        const string baseUrl = "http://localhost:3046";

        _ = app.RunAsync(baseUrl);

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);

        var response = await client.PostAsJsonAsync("/", new Person() {Name = "Ryu"});

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    private class Person
    {
        public string? Name { get; init; }
    }

    private class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}