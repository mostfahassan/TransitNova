using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransitNova.BusinessLayer.Common.Reports
{
    public static class ReportPayloadSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

        public static string Serialize<TContract>(TContract contract)
        {
            ArgumentNullException.ThrowIfNull(contract);
            return JsonSerializer.Serialize(contract, SerializerOptions);
        }

        public static TContract Deserialize<TContract>(string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                throw new InvalidOperationException("Report payload cannot be null or empty.");
            }

            var contract = JsonSerializer.Deserialize<TContract>(payloadJson, SerializerOptions);
            return contract ?? throw new InvalidOperationException($"Failed to deserialize report payload for '{typeof(TContract).Name}'.");
        }

        private static JsonSerializerOptions CreateSerializerOptions()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
