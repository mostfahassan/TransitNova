namespace TransitNova.Domain.Contracts.Constants
{
    public static class Constant
    {

        public static class InvoiceReportConstants
        {
            public const string PaymentIdPrefix = "PaymentId";
        }
        public static class ShipmentReportConstants
        {
            public const string ShipmentIdPrefix = "ShipmentId";
        }

        public static class PaymentReferenceConstants
        {
            public const string Bundle = "Bundle";
            public const string Shipment = "Shipment";
        }

        public static class PaymentServiceEndpointConstants
        {
            public const string Pay = "pay";
            public const string Subscribe = "subscribe";
        }

    }
}
