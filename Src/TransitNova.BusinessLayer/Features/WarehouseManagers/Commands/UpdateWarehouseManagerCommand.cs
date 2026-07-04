using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Commands
{
    public sealed record UpdateWarehouseManagerCommand(UpdateWarehouseManagerProfile Dto)
       : ICommand<BaseResult>,ICacheInvalidator;
}
