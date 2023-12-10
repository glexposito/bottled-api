using Bottled.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Bottled.Tests;

public abstract class IntegrationTestBase(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client = factory.CreateClient();
    protected readonly BottledContext DbContext = factory.Services.CreateScope().ServiceProvider
        .GetRequiredService<BottledContext>();
}