using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Validators.AddressValidator;

public sealed class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(address => address.MainAddress)
            .NotEmpty()
            .MaximumLength(Address.MainAddressMaxLength);

        RuleFor(address => address.SecondaryAddress)
            .MaximumLength(Address.SecondaryAddressMaxLength)
            .When(address => !string.IsNullOrWhiteSpace(address.SecondaryAddress));

        RuleFor(address => address.Street)
            .NotEmpty()
            .MaximumLength(Address.StreetMaxLength);
    }
}
