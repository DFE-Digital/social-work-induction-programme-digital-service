using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators;

public abstract class ValidatorTestBase<TValidator, TModel, TFaker> where TValidator : AbstractValidator<TModel>
{
    private protected TFaker Faker { get; }

    private protected TValidator Sut { get; }

    protected ValidatorTestBase(TValidator sut, TFaker faker)
    {
        Faker = faker;

        Sut = sut;
    }
}
