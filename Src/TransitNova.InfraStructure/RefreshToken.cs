namespace TransitNova.InfraStructure
{
    public class RefreshToken
    {
        public Guid Id { get; }
        public string Token { get; set; } = null!;
        public Guid UserId { get; set; } 
        public virtual AppUser User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string? ReplacedByToken { get; set; }
    }
}
