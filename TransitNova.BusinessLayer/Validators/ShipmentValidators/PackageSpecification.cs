using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
public sealed class PackageSpecificationValidator
    : AbstractValidator<PackageSpecificationDto>
{
    public PackageSpecificationValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0);

        RuleFor(x => x.Width)
            .GreaterThan(0);

        RuleFor(x => x.Height)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0);
    }
}