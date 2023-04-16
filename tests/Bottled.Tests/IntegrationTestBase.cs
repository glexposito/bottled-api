using Bottled.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly BottledContext DbContext;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (env == null ) 
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Integration");
        }

        Client = factory.CreateClient();

        DbContext = factory.Services.CreateScope().ServiceProvider
            .GetRequiredService<BottledContext>();
    }
}