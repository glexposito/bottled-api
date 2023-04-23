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
    private readonly WebApplication _app;
    private const string BaseUrl = "http://localhost:3046";
    
    public ValidationFilterTests()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

        _app = builder.Build();
    }
    
    [Fact]
    public async void ValidationFilter_WhenValidationErrors_ShouldReturn400BadRequest()
    {
        _app.MapPost("/", (Person person) => Results.Ok())
            .AddEndpointFilter<ValidationFilter<Person>>();
        
        _ = _app.RunAsync(BaseUrl);

        using var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);

        var response = await client.PostAsJsonAsync("/", new Person());

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async void ValidationFilter_WhenNoValidationErrors_ShouldReturn200OK()
    {
        _app.MapPost("/", (Person person) => Results.Ok())
            .AddEndpointFilter<ValidationFilter<Person>>();
        
        _ = _app.RunAsync(BaseUrl);

        using var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);

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