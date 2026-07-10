using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Reports;

namespace TransitNova.BusinessLayer.Validators.ReportValidators
{
    public sealed class InvoiceReportContractValidator : AbstractValidator<InvoiceReportContract>
    {
        public InvoiceReportContractValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty();
        }
    }
}
