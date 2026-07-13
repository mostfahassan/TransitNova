using System.Collections;
using System.Globalization;

using TransitNova.UI.ViewModels;

namespace TransitNova.UI.Infrastructure.Mvc.Common;

public static class AdminDisplay
{
    public static string Format(object? value, string propertyPath = "", string kind = "text")
    {
        if (value is null)
            return "-";

        if (value is string text)
            return string.IsNullOrWhiteSpace(text) ? "-" : text;

        if (value is bool boolean)
            return boolean ? "Yes" : "No";

        if (value is DateTime dateTime)
            return dateTime == default ? "-" : dateTime.ToString("MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture);

        if (value is DateTimeOffset dateTimeOffset)
            return dateTimeOffset.ToString("MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture);

        if (value is decimal decimalValue)
            return IsCurrency(propertyPath, kind)
                ? decimalValue.ToString("C0", CultureInfo.CreateSpecificCulture("en-US"))
                : decimalValue.ToString("N2", CultureInfo.InvariantCulture);

        if (value is double doubleValue)
            return doubleValue.ToString("N2", CultureInfo.InvariantCulture);

        if (value is float floatValue)
            return floatValue.ToString("N2", CultureInfo.InvariantCulture);

        if (value is Guid guid)
            return guid == Guid.Empty ? "-" : guid.ToString()[..8].ToUpperInvariant();

        if (value is IEnumerable enumerable && value is not string)
        {
            var values = enumerable.Cast<object?>()
                .Where(item => item is not null)
                .Select(item => Format(item, propertyPath, kind))
                .Where(item => !string.IsNullOrWhiteSpace(item) && item != "-")
                .Take(4)
                .ToArray();
            return values.Length == 0 ? "-" : string.Join(", ", values);
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "-";
    }


    public static AdminDisplayCellViewModel AdminCell(string header, object? value, string kind = "text") =>
        new(header, Format(value, header, kind), kind);

    public static AdminTableRowViewModel AdminRow(string routeIdName, object? routeId, params AdminDisplayCellViewModel[] cells) =>
        new()
        {
            Cells = cells,
            RouteValues = new Dictionary<string, object?> { [routeIdName] = routeId }
        };

    public static OpsDisplayCellViewModel OpsCell(string header, object? value, string kind = "text") =>
        new(header, Format(value, header, kind), kind);

    public static OpsTableRowViewModel OpsRow(string detailsRouteIdName, object? detailsRouteId, params OpsDisplayCellViewModel[] cells) =>
        new()
        {
            Cells = cells,
            DetailsRouteValues = new Dictionary<string, object?> { [detailsRouteIdName] = detailsRouteId },
            PrimaryRouteValues = new Dictionary<string, object?> { [detailsRouteIdName] = detailsRouteId }
        };

    public static OpsTableRowViewModel OpsRow(string detailsRouteIdName, object? detailsRouteId, string primaryRouteIdName, object? primaryRouteId, params OpsDisplayCellViewModel[] cells) =>
        new()
        {
            Cells = cells,
            DetailsRouteValues = new Dictionary<string, object?> { [detailsRouteIdName] = detailsRouteId },
            PrimaryRouteValues = new Dictionary<string, object?> { [primaryRouteIdName] = primaryRouteId }
        };

    public static WarehouseDisplayCellViewModel WarehouseCell(string header, object? value, string kind = "text") =>
        new(header, Format(value, header, kind), kind);

    public static WarehouseTableRowViewModel WarehouseRow(string routeIdName, object? routeId, params WarehouseDisplayCellViewModel[] cells) =>
        new()
        {
            Cells = cells,
            RouteValues = new Dictionary<string, object?> { [routeIdName] = routeId }
        };
    public static string BadgeClass(string value)
    {
        var text = value.ToLowerInvariant();

        if (text.Contains("active") || text.Contains("delivered") || text.Contains("approved") || text.Contains("success") || text.Contains("available") || text.Contains("picked") || text.Contains("warehouse") || text == "yes")
            return "admin-badge admin-badge-success";

        if (text.Contains("pending") || text.Contains("transit") || text.Contains("review") || text.Contains("busy") || text.Contains("assigned") || text.Contains("pickup") || text.Contains("delivery"))
            return "admin-badge admin-badge-warning";

        if (text.Contains("cancel") || text.Contains("reject") || text.Contains("delete") || text.Contains("inactive") || text.Contains("locked") || text.Contains("delay") || text == "no")
            return "admin-badge admin-badge-danger";

        return "admin-badge admin-badge-neutral";
    }

    private static bool IsCurrency(string propertyPath, string kind)
    {
        if (string.Equals(kind, "currency", StringComparison.OrdinalIgnoreCase))
            return true;

        return propertyPath.Contains("Price", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Revenue", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Cost", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Amount", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Commission", StringComparison.OrdinalIgnoreCase);
    }
}

