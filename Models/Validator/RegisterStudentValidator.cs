﻿using CSharpFunctionalExtensions;
using FluentValidation;
using Models.Repositories;
using Models.ValueObjects;
using Models.ViewModels;

namespace Models.Validator;

public class RegisterStudentValidator : AbstractValidator<RegisterStudent>
{
    public RegisterStudentValidator(IUnitOfWork unitOfWork)
    {
        //RuleSet(ActionCrud.Add.ToString(), () =>
        //{
        //    //RuleFor(o => o.Phone).NotEmpty().Matches(@"^0(9\d{9})$");
        //});

        //CascadeMode = CascadeMode.Stop;

        RuleFor(o => o.NationalCode)/*.Cascade(CascadeMode.Stop)*/
            .NotNull()
            .Length(10)
            .Must(o => unitOfWork.StudentRepository.ExistByNationalCode(o) == false).WithMessage(Errors.Student.NationalCodeIsTaken().Serialize());

        //Transform(o => o.FirstName, o => (o ?? "").Trim());


        //RuleFor(o => o.FirstName)
        //    .NotEmpty()
        //    .Must(o => o != null && o.StartsWith("h") && o.EndsWith("a"));

        //RuleFor(o => o.LastName)
        //    .Must(o => o != null && o.StartsWith("h") && o.EndsWith("a"))
        //    .NotEmpty();

        //RuleFor(o => o.FirstName)
        //    .StartAndEndControl("h", "a");

        //RuleFor(o => o.LastName)
        //    .StartAndEndControl("z", "b");

        //RuleFor(o => o.RegisterAddress).NotNull()
        //    .Must(o => o?.Count > 0 && o.Count <= 2);

        //RuleForEach(o => o.RegisterAddress)
        //    .SetValidator(new RegisterAddressValidator());

        //RuleFor(o => o.Email)
        //    .NotEmpty()
        //    .EmailAddress();

        //RuleFor(o => o.Age)
        //    .GreaterThan(18)
        //    .LessThan(40);

        RuleFor(o => o.FirstName)
            .NotEmpty()
            .MustBeValueObject(FirstName.Create);

        //When(o => !string.IsNullOrEmpty(o.Phone), () =>
        //{
        //    RuleFor(o => o.Phone).NotEmpty().Matches(@"^0(9\d{9})$");

        //    RuleFor(o => o.Email).Null();

        //}).Otherwise(() =>
        //{
        //    RuleFor(o => o.Email).NotEmpty().EmailAddress();

        //    RuleFor(o => o.Phone).Null();
        //});

    }
}

public class RegisterAddressValidator : AbstractValidator<RegisterAddress>
{
    public RegisterAddressValidator()
    {
        RuleFor(o => o.City).NotEmpty();
        RuleFor(o => o.State).NotEmpty();
        RuleFor(o => o.PostalCode).NotEmpty();
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.CompleteAddress).NotEmpty();
    }
}


public static class CustomeValidator
{
    public static IRuleBuilder<T, string?> StartAndEndControl<T>
        (this IRuleBuilder<T, string?> ruleBuilder, string? start = null, string? end = null)
        => ruleBuilder.Custom((input, Context) =>
        {
            if (start != null && input != null && !input.StartsWith(start))
                Context.AddFailure(Errors.General.ValueIsInvalid().Serialize());

            if (end != null && input != null && !input.EndsWith(end))
                Context.AddFailure(Errors.General.ValueIsInvalid().Serialize());
        });

    public static IRuleBuilderOptions<T, string?>
        MustBeValueObject<T, TValueObject>
        (this IRuleBuilder<T, string?> ruleBuilder, Func<string, Result<TValueObject, Error>> factoryMethod)
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, string>)ruleBuilder.Custom((value, context) =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Result<TValueObject, Error> result = factoryMethod(value);

            if (result.IsFailure)
                context.AddFailure(result.Error.Serialize());
        });
    }

    public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return DefaultValidatorExtensions.NotEmpty(ruleBuilder)
            .WithMessage(Errors.General.ValueIsRequired().Serialize());
    }

    public static IRuleBuilderOptions<T, TProperty> NotNull<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return DefaultValidatorExtensions.NotNull(ruleBuilder)
            .WithMessage(Errors.General.ValueIsRequired().Serialize());
    }

    public static IRuleBuilderOptions<T, TProperty> Length<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, int min, int max)
    {
        return DefaultValidatorExtensions.NotNull(ruleBuilder)
            .WithMessage(Errors.General.InvalidLength().Serialize());
    }
}