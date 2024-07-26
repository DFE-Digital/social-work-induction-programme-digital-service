using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators;

public abstract class ValidatorTestBase<TValidator, TModel, TFaker>(TValidator sut, TFaker faker)
    where TValidator : AbstractValidator<TModel>
{
    private protected TFaker Faker { get; } = faker;

    private protected TValidator Sut { get; } = sut;
}
