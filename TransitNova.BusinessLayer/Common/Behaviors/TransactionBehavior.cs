using MediatR;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public class TransactionPipelineBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : BaseResult
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            if (request is not ITransactional)
                return await next(ct);

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var result = await next(ct);

                if (result.IsSuccess)
                    await unitOfWork.CommitAsync(ct);
                else
                    await unitOfWork.RollbackAsync(ct);

                return result;
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
