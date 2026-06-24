namespace TransitNova.Domain
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        DateTime? DeletedOn { get; }
     
    }
}
