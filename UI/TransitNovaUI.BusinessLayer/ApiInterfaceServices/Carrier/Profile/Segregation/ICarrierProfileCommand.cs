using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation
{
    internal interface ICarrierProfileCommand : IAddCarrierAdditionalInfoCommandService, IUpdateCarrierProfileCommandService
    {
    }
}
