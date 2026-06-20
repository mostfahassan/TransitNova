using System;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetOperationManagerDetailsQuery(Guid OperationManagerId) : IQuery<Result<OperationManagerProfileDto>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagerDetails(OperationManagerId);
    }

}

