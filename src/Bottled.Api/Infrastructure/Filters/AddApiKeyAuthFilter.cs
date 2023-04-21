using Microsoft.Extensions.Primitives;

namespace Bottled.Api.Infrastructure.Filters;

public class AddApiKeyAuthFilter : IEndpointFilter
{
    private const string HeaderKeyName = "X-API-Key";
    private const string Secret = "Hadoken";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Request.Headers.TryGetValue(HeaderKeyName, out var headerKeyValue);

        if (headerKeyValue != Secret)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}