using System;
namespace TransitNova.Domain.DomainExceptions
{
    public class InvalidCarrierStatusException : DomainException
    {
        public InvalidCarrierStatusException()
            : base("The carrier status is invalid.", "INVALID_CARRIER_STATUS") { }
    }

}
