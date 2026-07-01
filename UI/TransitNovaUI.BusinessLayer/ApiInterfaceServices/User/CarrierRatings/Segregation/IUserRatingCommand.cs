using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Segregation
{
    internal interface IUserRatingCommand: IRateDeliveryCarrierCommandService, IRatePickupCarrierCommandService
    {
    }
}
