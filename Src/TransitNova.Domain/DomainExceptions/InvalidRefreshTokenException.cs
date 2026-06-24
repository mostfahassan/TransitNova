using System;
namespace TransitNova.Domain.DomainExceptions
{

    public class InvalidRefreshTokenException : DomainException
    {
        public InvalidRefreshTokenException()
            : base("The refresh token is invalid.", "REFRESH_TOKEN_NOT_VALID") { }
    }

}
