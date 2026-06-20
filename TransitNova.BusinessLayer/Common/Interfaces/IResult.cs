
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Common.Interfaces
{
    public interface IResult
    {
        ResultStatus Status { get; }

        Error? Error { get; }

        IReadOnlyList<Error> Errors { get; }

    }
}
