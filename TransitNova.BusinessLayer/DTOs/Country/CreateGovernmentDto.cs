namespace TransitNova.BusinessLayer.DTOs.Country
{
    public sealed class CreateGovernmentDto
    {
        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }
    }
}
