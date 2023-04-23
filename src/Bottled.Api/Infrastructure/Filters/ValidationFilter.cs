using FluentValidation;

namespace Bottled.Api.Infrastructure.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var contextObj = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T));

        if (contextObj == null)
        {
            return Results.BadRequest();
        }

        var validationResult = await _validator.ValidateAsync((T)contextObj);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}