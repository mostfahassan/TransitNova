using System;
namespace TransitNova.Domain.DomainExceptions
{
    public class ReusedRefreshTokenException : DomainException
    {
        public ReusedRefreshTokenException(Guid userId, string token)
           : base($"Refresh token reuse detected. UserId: {userId}","REUSED_REFRESHTOKEN_EXCEPTION") { }
    }

}
