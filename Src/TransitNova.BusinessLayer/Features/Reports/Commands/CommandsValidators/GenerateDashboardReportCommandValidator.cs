using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Features.Reports.Commands.CommandsValidators
{
    public sealed class GenerateDashboardReportCommandValidator : AbstractValidator<GenerateDashboardReportCommand>
    {
        public GenerateDashboardReportCommandValidator(IValidator<DashboardReportContract> contractValidator)
        {
            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.RequestedBy)
                .NotEmpty();

            RuleFor(x => x.Contract)
                .NotNull()
                .SetValidator(contractValidator);
        }
    }
}
