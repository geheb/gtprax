namespace GtPrax.WebApp.Tests.Models;

using GtPrax.Application.Models;
using GtPrax.WebApp.Models;
using Xunit;

public sealed class TherapyTimesInputTests
{
    [Fact]
    public void Map_NoDaysSelected_ShouldReturnEmptyArray()
    {
        var input = new TherapyTimesInput();
        var result = input.Map();
        Assert.Empty(result);
    }

    [Fact]
    public void Map_MondayMorningSelected_ShouldReturnMondayEntry()
    {
        var input = new TherapyTimesInput
        {
            Monday = [true, false, false]
        };
        var result = input.Map();
        Assert.Single(result);
        Assert.Equal(DayOfWeek.Monday, result[0].DayOfWeek);
        Assert.True(result[0].IsMorning);
        Assert.False(result[0].IsAfternoon);
        Assert.False(result[0].IsHomeVisit);
    }

    [Fact]
    public void Map_MultipleSelectedDays_ShouldReturnMultipleEntries()
    {
        var input = new TherapyTimesInput
        {
            Monday = [true, false, false],
            Wednesday = [false, true, false],
            Friday = [false, false, true]
        };
        var result = input.Map();
        Assert.Equal(3, result.Length);
        Assert.Equal(DayOfWeek.Monday, result[0].DayOfWeek);
        Assert.Equal(DayOfWeek.Wednesday, result[1].DayOfWeek);
        Assert.Equal(DayOfWeek.Friday, result[2].DayOfWeek);
    }

    [Fact]
    public void Map_WithTimeOnly_ShouldIncludeDay()
    {
        var input = new TherapyTimesInput
        {
            TuesdayTime = "09:30"
        };
        var result = input.Map();
        Assert.Single(result);
        Assert.Equal(DayOfWeek.Tuesday, result[0].DayOfWeek);
        Assert.Equal("09:30", result[0].Time);
    }

    [Fact]
    public void Map_InvalidTime_ShouldSetTimeToNull()
    {
        var input = new TherapyTimesInput
        {
            MondayTime = "invalid"
        };
        var result = input.Map();
        Assert.Single(result);
        Assert.Null(result[0].Time);
    }

    [Fact]
    public void MapFromDto_NullDto_ShouldResetAll()
    {
        var input = new TherapyTimesInput
        {
            Monday = [true, true, true],
            MondayTime = "10:00"
        };
        input.Map((TherapyTimesDto[]?)null);

        Assert.All(input.Monday, b => Assert.False(b));
        Assert.Null(input.MondayTime);
    }

    [Fact]
    public void MapFromDto_WithEntries_ShouldSetCorrectly()
    {
        var dto = new TherapyTimesDto[]
        {
            new() { DayOfWeek = DayOfWeek.Monday, IsMorning = true, IsAfternoon = false, IsHomeVisit = true, Time = "08:00" },
            new() { DayOfWeek = DayOfWeek.Thursday, IsMorning = false, IsAfternoon = true, IsHomeVisit = false, Time = "14:00" }
        };

        var input = new TherapyTimesInput();
        input.Map(dto);

        Assert.True(input.Monday[0]);
        Assert.False(input.Monday[1]);
        Assert.True(input.Monday[2]);
        Assert.Equal("08:00", input.MondayTime);

        Assert.False(input.Thursday[0]);
        Assert.True(input.Thursday[1]);
        Assert.False(input.Thursday[2]);
        Assert.Equal("14:00", input.ThursdayTime);
    }

    [Fact]
    public void MapFromDto_EmptyArray_ShouldResetAll()
    {
        var input = new TherapyTimesInput
        {
            Friday = [true, false, true]
        };
        input.Map([]);

        Assert.All(input.Friday, b => Assert.False(b));
    }
}
