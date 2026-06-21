
namespace TransitNova.BusinessLayer.Common.Exceptions
{
    public sealed class ReusedRefreshTokenException : Exception
    {
        public ReusedRefreshTokenException(Guid userId, string token)
            : base($"Refresh token reuse detected. UserId: {userId}, Token: {token}") { }
      
        public sealed class RevokingRefreshTokenException : Exception
        {
            public RevokingRefreshTokenException()
                : base("Refresh token revoking failed") { }  
        }
        public sealed class IdempotentConflicExceptionException : Exception
        {
            public IdempotentConflicExceptionException()
                : base("Request already processed") { }  
        }
    }
}
