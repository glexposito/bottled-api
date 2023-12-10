using FluentValidation;

namespace Bottled.Api.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var contextObj = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T));

        if (contextObj == null)
        {
            return Results.BadRequest();
        }

        var validationResult = await validator.ValidateAsync((T)contextObj);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}