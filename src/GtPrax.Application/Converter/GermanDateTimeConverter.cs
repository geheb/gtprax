namespace GtPrax.Application.Converter;

using System.Globalization;
using System.Runtime.InteropServices;

public sealed class GermanDateTimeConverter
{
    private const string IsoDateTimeFormat = "yyyy-MM-ddTHH:mm";
    private readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("de-DE");
    private readonly TimeZoneInfo _westEuropeTimeZone;

    public GermanDateTimeConverter()
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var timezone = isWindows ? "Central European Standard Time" : "Europe/Berlin";
        _westEuropeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
    }

    public string? ToDateTime(DateTimeOffset date) =>
        date.ToString("dd.MM.yyyy HH\\:mm", _culture);

    public string ToDateTimeShort(DateTimeOffset date) =>
        date.ToString("dd.MM. HH\\:mm", _culture);

    public string ToDate(DateTimeOffset date) =>
        date.ToString("dd.MM.yyyy", _culture);

    public string ToTime(DateTimeOffset date) =>
        date.ToString("HH\\:mm", _culture);

    public DateTimeOffset ToUtc(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var offset = new DateTimeOffset(dateTime, _westEuropeTimeZone.GetUtcOffset(dateTime));
        return offset.ToUniversalTime();
    }

    public DateTimeOffset ToLocal(DateTimeOffset date) =>
        TimeZoneInfo.ConvertTime(date, _westEuropeTimeZone);

    public DateTimeOffset? ParseIsoToUtc(string? isoDate)
    {
        if (string.IsNullOrEmpty(isoDate))
        {
            return default;
        }
        if (!DateTime.TryParseExact(isoDate, IsoDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return default;
        }
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, _westEuropeTimeZone);
    }

    public string ToIso(DateTimeOffset date) =>
        date.ToString(IsoDateTimeFormat, CultureInfo.InvariantCulture);

    public string Format(DateTimeOffset start, DateTimeOffset end)
    {
        var nowYear = ToLocal(DateTimeOffset.UtcNow).Year;

        if (start.Date == end.Date)
        {
            if (nowYear == start.Year)
            {
                return ToDateTimeShort(start) + "-" + ToTime(end);
            }
            return ToDateTime(start) + "-" + ToTime(end);
        }

        if (start.Year == end.Year && nowYear == start.Year)
        {
            return ToDateTimeShort(start) + " - " + ToDateTimeShort(end);
        }

        return ToDateTime(start) + " - " + ToDateTime(end);
    }
}
