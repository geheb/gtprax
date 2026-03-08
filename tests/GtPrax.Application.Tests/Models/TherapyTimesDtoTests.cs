namespace GtPrax.Application.Tests.Models;

using GtPrax.Application.Models;
using Xunit;

public sealed class TherapyTimesDtoTests
{
    [Theory]
    [InlineData("08:00", true)]
    [InlineData("11:59", true)]
    [InlineData("00:00", true)]
    public void IsTimeMorning_BeforeNoon_ShouldReturnTrue(string time, bool expected)
    {
        var dto = new TherapyTimesDto { Time = time };
        Assert.Equal(expected, dto.IsTimeMorning);
    }

    [Theory]
    [InlineData("12:00", false)]
    [InlineData("14:30", false)]
    public void IsTimeMorning_AtOrAfterNoon_ShouldReturnFalse(string time, bool expected)
    {
        var dto = new TherapyTimesDto { Time = time };
        Assert.Equal(expected, dto.IsTimeMorning);
    }

    [Fact]
    public void IsTimeMorning_NullTime_ShouldReturnFalse()
    {
        var dto = new TherapyTimesDto { Time = null };
        Assert.False(dto.IsTimeMorning);
    }

    [Fact]
    public void IsTimeMorning_InvalidTime_ShouldReturnFalse()
    {
        var dto = new TherapyTimesDto { Time = "invalid" };
        Assert.False(dto.IsTimeMorning);
    }

    [Theory]
    [InlineData("12:00", true)]
    [InlineData("14:30", true)]
    [InlineData("23:59", true)]
    public void IsTimeAfternoon_AtOrAfterNoon_ShouldReturnTrue(string time, bool expected)
    {
        var dto = new TherapyTimesDto { Time = time };
        Assert.Equal(expected, dto.IsTimeAfternoon);
    }

    [Theory]
    [InlineData("08:00", false)]
    [InlineData("11:59", false)]
    public void IsTimeAfternoon_BeforeNoon_ShouldReturnFalse(string time, bool expected)
    {
        var dto = new TherapyTimesDto { Time = time };
        Assert.Equal(expected, dto.IsTimeAfternoon);
    }

    [Fact]
    public void IsTimeAfternoon_NullTime_ShouldReturnFalse()
    {
        var dto = new TherapyTimesDto { Time = null };
        Assert.False(dto.IsTimeAfternoon);
    }

    [Fact]
    public void IsTimeAfternoon_InvalidTime_ShouldReturnFalse()
    {
        var dto = new TherapyTimesDto { Time = "invalid" };
        Assert.False(dto.IsTimeAfternoon);
    }
}
