using System.Collections;
using System.Globalization;
using System.Reflection;

namespace TransitNova.UI.Infrastructure.Mvc.Common;

public static class AdminDisplay
{
    public static IReadOnlyList<object> Rows(object? source)
    {
        if (source is null)
            return [];

        if (source is string)
            return [source];

        var data = Value(source, "Data|Items|Users|RecentShipments|RecentActivities|TopCarriers|TopOperationManagers");
        if (data is not null && !ReferenceEquals(data, source))
            return Rows(data);

        if (source is IEnumerable enumerable)
            return enumerable.Cast<object>().ToList();

        return [source];
    }

    public static object? Value(object? source, string propertyPath)
    {
        if (source is null || string.IsNullOrWhiteSpace(propertyPath))
            return null;

        foreach (var alternative in propertyPath.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var value = ReadPath(source, alternative);
            if (HasDisplayValue(value))
                return value;
        }

        return null;
    }

    public static string Text(object? source, string propertyPath, string kind = "text") =>
        Format(Value(source, propertyPath), propertyPath, kind);

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
            var values = enumerable.Cast<object>().Select(ShortObjectText).Where(item => !string.IsNullOrWhiteSpace(item)).Take(4).ToArray();
            return values.Length == 0 ? "-" : string.Join(", ", values);
        }

        return ShortObjectText(value);
    }

    public static bool Boolean(object? source, string propertyPath)
    {
        var value = Value(source, propertyPath);

        return value switch
        {
            bool boolean => boolean,
            string text => bool.TryParse(text, out var parsed) && parsed,
            int number => number != 0,
            _ => false
        };
    }

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

    public static string PrimaryLabel(object? source) =>
        Text(source, "Name|FullName|BundleName|RoleName|UserName|PlateNumber|TrackingNumber|Title|Email|Code|Id");

    public static object? RouteValue(object? source, string idPropertyPath) =>
        Value(source, idPropertyPath);

    public static int TotalCount(object? source)
    {
        var value = Value(source, "TotalCount|TotalUsers|UsersCount|Count");
        return value is int count ? count : Rows(source).Count;
    }

    public static int PageNumber(object? source)
    {
        var value = Value(source, "PageNumber");
        return value is int page && page > 0 ? page : 1;
    }

    public static int TotalPages(object? source)
    {
        var value = Value(source, "TotalPages");
        if (value is int pages && pages > 0)
            return pages;

        var pageSizeValue = Value(source, "PageSize");
        var pageSize = pageSizeValue is int size && size > 0 ? size : Math.Max(Rows(source).Count, 1);
        return Math.Max(1, (int)Math.Ceiling(TotalCount(source) / (double)pageSize));
    }

    private static object? ReadPath(object source, string propertyPath)
    {
        object? current = source;
        foreach (var segment in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (current is null)
                return null;

            current = ReadProperty(current, segment);
        }

        return current;
    }

    private static object? ReadProperty(object source, string propertyName)
    {
        if (source is IDictionary dictionary && dictionary.Contains(propertyName))
            return dictionary[propertyName];

        var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        return property?.GetValue(source);
    }

    private static bool HasDisplayValue(object? value)
    {
        if (value is null)
            return false;

        if (value is string text)
            return !string.IsNullOrWhiteSpace(text);

        return true;
    }

    private static bool IsCurrency(string propertyPath, string kind)
    {
        if (string.Equals(kind, "currency", StringComparison.OrdinalIgnoreCase))
            return true;

        return propertyPath.Contains("Price", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Revenue", StringComparison.OrdinalIgnoreCase)
            || propertyPath.Contains("Cost", StringComparison.OrdinalIgnoreCase);
    }

    private static string ShortObjectText(object value)
    {
        if (value is Guid guid)
            return guid.ToString()[..8].ToUpperInvariant();

        var label = Value(value, "Name|FullName|BundleName|RoleName|UserName|TrackingNumber|PlateNumber|Title|Email|Code");
        if (label is not null && !ReferenceEquals(label, value))
            return Format(label);

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "-";
    }
}
