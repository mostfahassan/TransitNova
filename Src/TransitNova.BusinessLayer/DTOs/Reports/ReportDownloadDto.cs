using TransitNova.Domain.Enums.Reports;
namespace TransitNova.BusinessLayer.DTOs.Reports
{
    public sealed record ReportDownloadDto(string FilePath, string FileName);
    public sealed record ReportClanableData(Guid Id , string FilePath);
}
