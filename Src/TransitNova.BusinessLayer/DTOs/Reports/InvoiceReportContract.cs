namespace TransitNova.BusinessLayer.DTOs.Reports
{
    public sealed class InvoiceReportContract
    {
        public const string ReportKey = "invoice";

        public Guid PaymentId { get; set; }
    }
}
