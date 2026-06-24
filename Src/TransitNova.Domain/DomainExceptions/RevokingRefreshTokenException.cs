namespace TransitNova.Domain.DomainExceptions
{
    public sealed class RevokingRefreshTokenException : Exception
    {
        public RevokingRefreshTokenException()
            : base("Failed to revoke the refresh token.") { }
    }
}
