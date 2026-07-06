namespace TransitNovaUI.BusinessLayer.Common.APIHelper
{
    public static class ApiHelper
    {
        public static string BaseUrl =>
            Environment.GetEnvironmentVariable("TransitNovaApi__BaseUrl")
            ?? Environment.GetEnvironmentVariable("TRANSITNOVA_API_BASEURL")
            ?? "http://localhost:5200";

        public const string IdempotentHeader = "X-Idempotency-Key";
    }
}
