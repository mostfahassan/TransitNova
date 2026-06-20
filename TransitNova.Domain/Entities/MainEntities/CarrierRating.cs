
namespace TransitNova.Domain.Entities.MainEntities
{
    public class CarrierRating
    {
        public Guid Id { get; private set; } = Guid.CreateVersion7();
        public Guid CarrierId { get; private set; }
        public Guid ShipmentId { get; private set; }
        public Guid CustomerId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private CarrierRating()
        {
            
        }
        private CarrierRating(Guid carrierId, Guid shipmentId, Guid customerId, int rating, string? comment)
        {
            CarrierId = carrierId;
            ShipmentId = shipmentId;
            CustomerId = customerId;
            Rating = rating;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }


        public static CarrierRating Create(Guid carrierId, Guid shipmentId, Guid customerId, int rating, string? comment)
         => new(carrierId, shipmentId, customerId, rating, comment);
         
    }
}
