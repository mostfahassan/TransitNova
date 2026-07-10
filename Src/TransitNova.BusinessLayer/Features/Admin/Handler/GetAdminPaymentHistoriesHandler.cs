using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;

namespace TransitNova.BusinessLayer.Features.Admin.Handler
{
    public sealed class GetAdminPaymentHistoriesHandler(
        IPaymentHistoryService paymentHistoryService,
        ILogger<GetAdminPaymentHistoriesHandler> logger)
        : IQueryHandler<GetAdminPaymentHistoriesQuery, Result<PagedResult<PaymentHistoryDetailsDto>>>
    {
        public Task<Result<PagedResult<PaymentHistoryDetailsDto>>> Handle(
            GetAdminPaymentHistoriesQuery request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Retrieving admin payment histories. PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.Filter.PageNumber,
                request.Filter.PageSize);

            return paymentHistoryService.GetPaymentHistoriesAsync(request.Filter, cancellationToken);
        }
    }
}
