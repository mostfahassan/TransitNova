using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.Validators.UserProfile.User
{
    public class CreateReceiverValidator :AbstractValidator<CreateReceiverDto>
    {
        public CreateReceiverValidator()
        {

            // Phone Number validation rules:
            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .MaximumLength(15)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Please enter a valid phone number.");

            // First Name and Last Name validation rules:
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .MaximumLength(50)
                .MinimumLength(2)
                .Matches(@"^[\p{L}\s]+$")
                .WithMessage("First Name must be between 2 and 50 characters long.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name Cant Be Empty")
                .MinimumLength(2)
                .MaximumLength(50)
                .Matches(@"^[\p{L}\s]+$").WithMessage("Last Name Cant Be Contain Small And Capital Letters");

            // CityId validation rules:
            RuleFor(u => u.CityId)
                .NotEmpty()
                .WithMessage("Please select a valid City.");


            // Email validation rules:
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");


            // Address validation rules:

            RuleFor(u => u.Address)
                .NotEmpty()
                .WithMessage("Address Field Is Required");

        }
    }
}
