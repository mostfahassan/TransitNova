using FluentValidation;
using MediatR;
using System.Windows.Input;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
namespace TransitNovaPayment.Busieness.Common.Behaviour;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsCommand())
            return await next(cancellationToken);

        var validatorList = validators.ToList();
        if (validatorList.Count == 0)
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validatorList.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .GroupBy(e => new { e.PropertyName, e.ErrorMessage })
            .Select(g => g.First())
            .Select(e => Errors.Validation(e.ErrorMessage))
            .ToList();

        if (errors.Count == 0)
            return await next(cancellationToken);

        return ResultFactory.Validation<TResponse>(errors);
    }

    private static bool IsCommand()
    {
        var requestType = typeof(TRequest);

        return typeof(ICommand).IsAssignableFrom(requestType)
            || requestType.GetInterfaces()
                .Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

}