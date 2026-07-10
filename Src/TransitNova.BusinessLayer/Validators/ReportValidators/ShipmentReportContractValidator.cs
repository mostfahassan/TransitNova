using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Validators.ReportValidators
{
    public sealed class ShipmentReportContractValidator : AbstractValidator<ShipmentReportContract>
    {
        public ShipmentReportContractValidator()
        {
            RuleFor(x => x.ShipmentId)
                .NotEmpty();
        }
    }
}
