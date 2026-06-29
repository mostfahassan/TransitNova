namespace TransitNova.Domain.DomainExceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message, Guid? tripId = null)
            : base(message, "TRIP_PLANNING_ERROR", "Trip", tripId) { }
    }

}
