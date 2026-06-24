namespace TransitNova.BusinessLayer.DTOs.Country
{
    public sealed class UpdateGovernmentDto
    {
        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }
    }
}
