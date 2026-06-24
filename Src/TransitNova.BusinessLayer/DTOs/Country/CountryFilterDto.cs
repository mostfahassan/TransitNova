namespace TransitNova.BusinessLayer.DTOs.Country
{
    public class CountryFilterDto
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool SortDescending { get; set; }
    }
}

