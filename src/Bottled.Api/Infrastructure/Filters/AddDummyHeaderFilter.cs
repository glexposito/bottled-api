namespace Bottled.Api.Infrastructure.Filters;

public class AddDummyHeaderFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Response.Headers.Add("Dummy-Header", "I'm useless!");
        var result = await next(context);
        return result;
    }
}