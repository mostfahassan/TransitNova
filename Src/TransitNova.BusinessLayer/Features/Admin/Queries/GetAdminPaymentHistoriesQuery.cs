using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;

namespace TransitNova.BusinessLayer.Features.Admin.Queries
{
    public sealed record GetAdminPaymentHistoriesQuery(PaymentHistoryFilterDto Filter)
        : IQuery<Result<PagedResult<PaymentHistoryDetailsDto>>>;
}
