namespace TransitNova.BusinessLayer.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        DateTime? DeletedOn { get; }
     
    }
}
