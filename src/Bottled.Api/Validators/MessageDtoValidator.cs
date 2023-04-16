using Bottled.Api.Dtos;
using FluentValidation;

namespace Bottled.Api.Validators;

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(255);
    }
}