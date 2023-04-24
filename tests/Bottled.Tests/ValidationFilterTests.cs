using System.Net;
using System.Net.Http.Json;
using Bottled.Api.Filters;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bottled.Tests;

public class ValidationFilterTests
{
    private const string BaseUrl = "http://localhost:3046";
    private readonly HttpClient _client;
    
    public ValidationFilterTests()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped<IValidator<Person>, PersonValidator>();
        builder.Services.AddHttpClient();
        var app = builder.Build();
        
        var factory = builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri(BaseUrl);
        
        app.MapPost("/", (Person person) => Results.Ok(person))
            .AddEndpointFilter<ValidationFilter<Person>>();
        
        _ = app.RunAsync(BaseUrl);
    }
    
    [Fact]
    public async void ValidationFilter_WhenPersonNameEmpty_And_ValidationErrors_ShouldReturn400BadRequest()
    {
        var response = await _client.PostAsJsonAsync("/", new Person());
        
        var jsonString = await response.Content.ReadAsStringAsync();
        
        jsonString.Should().Contain("One or more validation errors occurred.");
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async void ValidationFilter_WhenPersonNameFilled_And_NoValidationErrors_ShouldReturn200OK()
    {
        var response = await _client.PostAsJsonAsync("/", new Person() {Name = "Ryu"});

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