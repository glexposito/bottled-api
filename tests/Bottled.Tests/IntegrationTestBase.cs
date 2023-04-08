using Bottled.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly BottledContext DbContext;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();

        DbContext = factory.Services.CreateScope().ServiceProvider
            .GetRequiredService<BottledContext>();
    }
}