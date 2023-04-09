using Bottled.Api.Infrastructure.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Bottled.Tests;
public class AddDummyHeaderFilterTest
{
    [Fact]
    public async Task AddDummyHeaderFilter_ShouldAddDummyHeader()
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();
        app.MapGet("/", () => "Hello World!")
            .AddEndpointFilter<AddDummyHeaderFilter>();

        var baseUrl = "http://localhost:3045";

        _ = Task.Factory.StartNew(() => app.Run(baseUrl));

        using var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);

        var response = await client.GetAsync("/");

        response.Headers.Should().ContainEquivalentOf(new KeyValuePair<string, IEnumerable<string>>("Dummy-Header", new List<string>() { "I'm useless!" }));
    }
}
