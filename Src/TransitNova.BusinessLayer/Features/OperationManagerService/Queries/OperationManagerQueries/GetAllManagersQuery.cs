using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetAllManagersQuery : IQuery<Result<IEnumerable<OperationManagerProfileDto>>>;
}

