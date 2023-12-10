using Bottled.Api.Dtos;
using Bottled.Api.Validators;
using FluentValidation.TestHelper;

namespace Bottled.Tests;
public class MessageDtoValidatorTests
{
    private readonly MessageDtoValidator _validator = new();

    [Fact]
    public void WhenAuthorIsNull_ShouldHaveError()
    {
        var model = new MessageDto(null, null);
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Author)
            .WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public void WhenAuthorExceedsMaxCharacterLimit_ShouldHaveError()
    {
        var model = new MessageDto("Conde Alessandro Leonardo de la Cruz Alta Rosa de la Manchu", null);
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Author)
            .WithErrorCode("MaximumLengthValidator");
    }

    [Fact]
    public void WhenContentIsNull_ShouldHaveError()
    {
        var model = new MessageDto(null, null);
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public void WhenContentExceedsMaxCharacterLimit_ShouldHaveError()
    {
        // ReSharper disable once StringLiteralTypo
        var model = new MessageDto(null,
            "In Street Fighter, Ryu's fighting stance was similar to that of full-contact karate. However, Ryu was given a different stance in Street Fighter II, which was inspired by Bruce Lee's fighting stance in Enter the Dragon. Akiman made this change as he wanted Ryu to have a stance that wasn't traditional karate.");
        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorCode("MaximumLengthValidator");
    }
}