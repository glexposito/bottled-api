using Bottled.Api.Infrastructure.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Bottled.Tests;
public class AddApiKeyAuthFilterTests
{
    [Fact]
    public async Task AddApiKeyAuthFilter_WhenNoApiKey_ShouldReturn401Unauthorized()
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();
        app.MapGet("/", () => "Hello World!")
            .AddEndpointFilter<AddApiKeyAuthFilter>();

        const string baseUrl = "http://localhost:3045";

        _ = Task.Factory.StartNew(() => app.Run(baseUrl));

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);

        var response = await client.GetAsync("/");

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddApiKeyAuthFilter_WhenIncorrectApiKeyProvided_ShouldReturn401Unauthorized()
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();
        app.MapGet("/", () => "Hello World!")
            .AddEndpointFilter<AddApiKeyAuthFilter>();

        const string baseUrl = "http://localhost:3045";

        _ = Task.Factory.StartNew(() => app.Run(baseUrl));

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("X-API-Key", "Shoryuken");

        var response = await client.GetAsync("/");

        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddApiKeyAuthFilter_WhenCorrectApiKeyProvided_ShouldReturn200OK()
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();
        app.MapGet("/", () => "Hello World!")
            .AddEndpointFilter<AddApiKeyAuthFilter>();

        const string baseUrl = "http://localhost:3045";

        _ = Task.Factory.StartNew(() => app.Run(baseUrl));

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("X-API-Key", "Hadoken");

        var response = await client.GetAsync("/");

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }
}
