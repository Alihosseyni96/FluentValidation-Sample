﻿using FluentValidation;
using Models.ViewModels;

namespace Models.Validator;

public class RegisterStudentValidator : AbstractValidator<RegisterStudent>
{
    public RegisterStudentValidator()
    {
        RuleSet(ActionCrud.Add.ToString(), () =>
        {
            RuleFor(o => o.Phone).NotEmpty().Matches(@"^0(9\d{9})$");
        });

        RuleFor(o => o.NationalCode)
            .NotNull().WithMessage("لطفا کد ملی را وارد کنید")
            .Length(10).WithMessage("تعداد کاراکترها صحیح نیست");

        RuleFor(o => o.FirstName)
            .NotEmpty();

        RuleFor(o => o.RegisterAddress).NotNull()
            .Must(o=> o?.Count > 0 && o.Count <=2);

        RuleForEach(o => o.RegisterAddress)
            .SetValidator(new RegisterAddressValidator());

        RuleFor(o => o.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(o => o.Age)
            .GreaterThan(18)
            .LessThan(40);

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

