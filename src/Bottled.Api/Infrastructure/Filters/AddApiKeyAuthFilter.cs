using Microsoft.Extensions.Primitives;

namespace Bottled.Api.Infrastructure.Filters;

public class AddApiKeyAuthFilter : IEndpointFilter
{
    const string HeaderKeyName = "X-API-Key";
    const string Secret = "Hadoken";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Request.Headers.TryGetValue(HeaderKeyName, out StringValues headerKeyValue);

        if (headerKeyValue != Secret)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}