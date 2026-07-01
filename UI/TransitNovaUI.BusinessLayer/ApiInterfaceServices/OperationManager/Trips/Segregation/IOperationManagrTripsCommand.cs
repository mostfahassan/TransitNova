using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation
{
    internal interface IOperationManagrTripsCommand : IStartDeliveryTripCommandService, IStartPickupTripCommandService
    {
    }
    internal interface IOperationManagrTripsQuery : IGetCarrierTripByIdQueryService, IGetCarrierTripsQueryService
    {
    }
}
