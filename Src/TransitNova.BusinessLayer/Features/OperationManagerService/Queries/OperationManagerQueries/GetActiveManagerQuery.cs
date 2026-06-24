using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetActiveManagerQuery : IQuery<Result<List<OperationManagerProfileDto>>>;
   
}
