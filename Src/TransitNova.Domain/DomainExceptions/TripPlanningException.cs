using System;
namespace TransitNova.Domain.DomainExceptions
{

    public class TripPlanningException : DomainException
    {
        public TripPlanningException(string message, Guid? tripId = null)
            : base(message, "TRIP_PLANNING_ERROR", "Trip", tripId) { }
    }

}
