using Microsoft.EntityFrameworkCore;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.PagedResults;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;
using TransitNovaPayment.Infrastructure.Context;
namespace TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo
{
    internal sealed class PaymentQueryRepository(AppDbContext context) : IPaymentQueryRepository
    {
        public async Task<PagedResult<PaymentHistoryDetailsDto>> FilterPaymentHistoryAsync(
            FilterPaymentHistoryDto filterDto,
            CancellationToken cancellationToken)
        {
            IQueryable<PaymentHistory> query = context.PaymentHistory.AsNoTracking();

            if (filterDto.PaymentId.HasValue)
            {
                query = query.Where(history => history.PaymentId == filterDto.PaymentId.Value);
            }

            if (filterDto.PaymentStatus.HasValue)
            {
                query = query.Where(history => history.NewStatus == filterDto.PaymentStatus.Value);
            }
            if (filterDto.PaymentMethod.HasValue)
            {
                query = query.Where(history => history.Payment.PaymentMethod == filterDto.PaymentMethod.Value);
            }

            if (filterDto.CreatedAt.HasValue)
            {
                var createdAt = filterDto.CreatedAt.Value.Date;
                query = query.Where(history => history.CreatedAt.Date == createdAt);
            }

            if (filterDto.CreatedAtFrom.HasValue)
            {
                query = query.Where(history => history.CreatedAt >= filterDto.CreatedAtFrom.Value);
            }

            if (filterDto.CreatedAtTo.HasValue)
            {
                query = query.Where(history => history.CreatedAt <= filterDto.CreatedAtTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterDto.CreatedBy))
            {
                query = query.Where(history => history.CreatedBy == filterDto.CreatedBy);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var pageNumber = filterDto.PageNumber <= 0 ? 1 : filterDto.PageNumber;
            var pageSize = filterDto.PageSize <= 0 ? 10 : filterDto.PageSize;

            var paymentHistories = await query
                .OrderByDescending(history => history.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(history => new PaymentHistoryDetailsDto
                {
                    Id = history.Id,
                    PaymentId = history.PaymentId,
                    OldStatus = history.OldStatus.ToString(),
                    NewStatus = history.NewStatus.ToString(),
                    ChangedAt = history.ChangedAt,
                    CreatedAt = history.CreatedAt,
                    CreatedBy = history.CreatedBy
                })
                .ToListAsync(cancellationToken);

            return PagedResult<PaymentHistoryDetailsDto>.Page(paymentHistories, totalCount, pageNumber, pageSize);
        }
    }
}
