namespace GtPrax.Application.Tests.Converter;

using GtPrax.Application.Converter;
using Xunit;

public sealed class GermanDateTimeConverterTests
{
    private readonly GermanDateTimeConverter _sut = new();

    [Fact]
    public void ToDateTime_ShouldFormatCorrectly()
    {
        var date = new DateTimeOffset(2024, 3, 15, 14, 30, 0, TimeSpan.Zero);
        var result = _sut.ToDateTime(date);
        Assert.Equal("15.03.2024 14:30", result);
    }

    [Fact]
    public void ToDateTimeShort_ShouldFormatWithoutYear()
    {
        var date = new DateTimeOffset(2024, 3, 15, 14, 30, 0, TimeSpan.Zero);
        var result = _sut.ToDateTimeShort(date);
        Assert.Equal("15.03. 14:30", result);
    }

    [Fact]
    public void ToDate_DateTimeOffset_ShouldFormatCorrectly()
    {
        var date = new DateTimeOffset(2024, 12, 1, 0, 0, 0, TimeSpan.Zero);
        var result = _sut.ToDate(date);
        Assert.Equal("01.12.2024", result);
    }

    [Fact]
    public void ToDate_DateOnly_ShouldFormatCorrectly()
    {
        var date = new DateOnly(2024, 1, 5);
        var result = _sut.ToDate(date);
        Assert.Equal("05.01.2024", result);
    }

    [Fact]
    public void ToTime_ShouldFormatHoursAndMinutes()
    {
        var date = new DateTimeOffset(2024, 1, 1, 9, 5, 0, TimeSpan.Zero);
        var result = _sut.ToTime(date);
        Assert.Equal("09:05", result);
    }

    [Fact]
    public void FromIsoDateTime_ValidInput_ShouldReturnUtcOffset()
    {
        var result = _sut.FromIsoDateTime("2024-06-15T14:30");
        Assert.NotNull(result);
        Assert.Equal(TimeSpan.Zero, result.Value.Offset);
    }

    [Fact]
    public void FromIsoDateTime_InvalidInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoDateTime("not-a-date"));
    }

    [Fact]
    public void FromIsoDateTime_NullInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoDateTime(null));
    }

    [Fact]
    public void FromIsoDate_ValidInput_ShouldReturnDate()
    {
        var result = _sut.FromIsoDate("2024-03-15");
        Assert.NotNull(result);
        Assert.Equal(new DateOnly(2024, 3, 15), result.Value);
    }

    [Fact]
    public void FromIsoDate_InvalidInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoDate("invalid"));
    }

    [Fact]
    public void FromIsoDate_NullInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoDate(null));
    }

    [Fact]
    public void FromIsoTime_ValidInput_ShouldReturnTime()
    {
        var result = _sut.FromIsoTime("14:30");
        Assert.NotNull(result);
        Assert.Equal(new TimeOnly(14, 30), result.Value);
    }

    [Fact]
    public void FromIsoTime_InvalidInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoTime("invalid"));
    }

    [Fact]
    public void FromIsoTime_NullInput_ShouldReturnNull()
    {
        Assert.Null(_sut.FromIsoTime(null));
    }

    [Fact]
    public void ToIso_DateTimeOffset_ShouldFormatCorrectly()
    {
        var date = new DateTimeOffset(2024, 3, 15, 14, 30, 0, TimeSpan.Zero);
        Assert.Equal("2024-03-15T14:30", _sut.ToIso(date));
    }

    [Fact]
    public void ToIso_DateOnly_ShouldFormatCorrectly()
    {
        var date = new DateOnly(2024, 3, 15);
        Assert.Equal("2024-03-15", _sut.ToIso(date));
    }

    [Fact]
    public void ToIso_TimeOnly_ShouldFormatCorrectly()
    {
        var time = new TimeOnly(14, 30);
        Assert.Equal("14:30", _sut.ToIso(time));
    }

    [Fact]
    public void FromIsoDateTime_RoundTrip_ShouldPreserveLocalTime()
    {
        var original = "2024-06-15T14:30";
        var parsed = _sut.FromIsoDateTime(original);
        Assert.NotNull(parsed);
        var local = _sut.ToLocal(parsed.Value);
        var result = _sut.ToIso(local);
        Assert.Equal(original, result);
    }

    [Fact]
    public void ToUtc_ShouldConvertGermanDateToUtc()
    {
        var date = new DateOnly(2024, 6, 15);
        var result = _sut.ToUtc(date);
        Assert.Equal(TimeSpan.Zero, result.Offset);
    }

    [Fact]
    public void ToLocal_ShouldConvertUtcToGermanTime()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var local = _sut.ToLocal(utc);
        Assert.Equal(14, local.Hour); // CEST = UTC+2
    }

    [Fact]
    public void Format_SameDay_SameYear_ShouldUseShortFormat()
    {
        var now = DateTimeOffset.UtcNow;
        var start = new DateTimeOffset(now.Year, 3, 15, 10, 0, 0, TimeSpan.FromHours(1));
        var end = new DateTimeOffset(now.Year, 3, 15, 11, 30, 0, TimeSpan.FromHours(1));
        var result = _sut.Format(start, end);
        Assert.Contains("15.03.", result);
        Assert.Contains("-", result);
        Assert.DoesNotContain(now.Year.ToString(), result);
    }

    [Fact]
    public void Format_SameDay_OtherYear_ShouldIncludeYear()
    {
        var start = new DateTimeOffset(2020, 3, 15, 10, 0, 0, TimeSpan.FromHours(1));
        var end = new DateTimeOffset(2020, 3, 15, 11, 30, 0, TimeSpan.FromHours(1));
        var result = _sut.Format(start, end);
        Assert.Contains("2020", result);
        Assert.Contains("-", result);
    }

    [Fact]
    public void Format_CrossDay_SameYear_ShouldUseDateTimeShortForBoth()
    {
        var now = DateTimeOffset.UtcNow;
        var start = new DateTimeOffset(now.Year, 3, 15, 10, 0, 0, TimeSpan.FromHours(1));
        var end = new DateTimeOffset(now.Year, 3, 16, 11, 30, 0, TimeSpan.FromHours(1));
        var result = _sut.Format(start, end);
        Assert.Contains("15.03.", result);
        Assert.Contains("16.03.", result);
        Assert.Contains(" - ", result);
    }

    [Fact]
    public void Format_CrossDay_OtherYear_ShouldIncludeFullDates()
    {
        var start = new DateTimeOffset(2020, 3, 15, 10, 0, 0, TimeSpan.FromHours(1));
        var end = new DateTimeOffset(2020, 3, 16, 11, 30, 0, TimeSpan.FromHours(1));
        var result = _sut.Format(start, end);
        Assert.Contains("2020", result);
        Assert.Contains(" - ", result);
    }
}
