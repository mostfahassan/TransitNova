
namespace TransitNova.Domain.DomainExceptions
{  
    public class RefreshTokenNotFoundException : DomainException
    {
        public RefreshTokenNotFoundException()
            : base("Refresh token not found.", "REFRESH_TOKEN_NOT_FOUND") { }
    }

}
