using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Services.ShipmentServices
{
    public class ShipmentPricingServices : IShipmentPricingServices
    {
        private const decimal VolumetricDivisor = 5000m;
        private readonly decimal _baseRatePerKilogram = 25m;
        static decimal ApplyShipmentTypeMultiplier(decimal cost, enShipmentType type)
        {
            return type switch
            {
                enShipmentType.Standard => cost * 1.0m,
                enShipmentType.Express => cost * 1.5m,
                enShipmentType.Fragile => cost * 1.2m,
                _ => cost
            };
        }
        static decimal ApplyShipmentTypeMultiplier(decimal cost, TransportationMode? mode)
        {
            return mode switch
            {
                TransportationMode.Air => cost * 1.0m,
                TransportationMode.Sea => cost * 1.5m,
                TransportationMode.Land => cost * 1.2m,
                _ => cost
            };
        }
        private decimal Calculate(PackageSpecification packageSpecification, enShipmentType shipmentType, TransportationMode? mode)
        {

            decimal volumetricWeight = (packageSpecification.Length * packageSpecification.Width * packageSpecification.Height) / VolumetricDivisor;
            decimal chargeableWeight = Math.Max(packageSpecification.Weight, volumetricWeight);
            decimal baseCost = chargeableWeight * _baseRatePerKilogram;
            decimal prefinalCost = ApplyShipmentTypeMultiplier(baseCost, shipmentType);
            decimal finalCost = ApplyShipmentTypeMultiplier(prefinalCost, mode!);
            decimal minimumCharge = 50m;

            return Math.Max(finalCost, minimumCharge);
        }

        static DateTime CalculateEstimatedDeliveryDate(decimal cost, enShipmentType shipmentType, TransportationMode mode)
        { 
            var now = DateTime.UtcNow;
            int baseDays = GetBaseDays(shipmentType);
            int modeDays = GetTransportationDays(mode);
            int costDays = GetCostImpactDays(cost);
            int totalDays = baseDays + modeDays + costDays;
            var estimated = now.AddDays(totalDays);
            estimated = AdjustForWeekend(estimated);

            return estimated;
        }
        static DateTime AdjustForWeekend(DateTime estimated)
        {
            if (estimated.DayOfWeek == DayOfWeek.Friday)
                return estimated.AddDays(1);

            if (estimated.DayOfWeek == DayOfWeek.Saturday)
                return estimated.AddDays(2);

            return estimated;
        }
        static int GetCostImpactDays(decimal shipmentCost)
        {
            if (shipmentCost < 100) return 0;
            if (shipmentCost < 300) return 1;
            if (shipmentCost < 600) return 2;
            return 3;
        }
        static int GetTransportationDays(TransportationMode transportationMode)
        {
            return transportationMode switch
            {
                TransportationMode.Air => 0,
                TransportationMode.Land => 2,
                TransportationMode.Sea => 5,
                _ => 2
            };
        }
        static int GetBaseDays(enShipmentType shipmentType)
        {
            return shipmentType switch
            {
                enShipmentType.Express => 1,
                enShipmentType.Standard => 3,
                enShipmentType.Fragile => 5,
                _ => 3
            };
        }

        public (decimal, DateTime) CalculateShipment(PackageSpecification packageSpecification, enShipmentType shipmentType, TransportationMode mode)
        {
            var cost = Calculate(packageSpecification, shipmentType, mode);
            var date = CalculateEstimatedDeliveryDate(cost , shipmentType , mode);
            return (cost, date);
        }
      
    }
}
