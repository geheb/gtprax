namespace GtPrax.Application.Tests.Models;

using GtPrax.Application.Models;
using Xunit;

public sealed class WaitlistPatientDtoTests
{
    [Fact]
    public void CalcAge_StandardCase_ShouldReturnCorrectAge()
    {
        var dto = new WaitlistPatientDto { Birthday = new DateOnly(1990, 5, 15) };
        var age = dto.CalcAge(new DateTime(2024, 12, 1));
        Assert.Equal(34, age);
    }

    [Fact]
    public void CalcAge_BirthdayNotYetPassed_ShouldReturnOneLess()
    {
        var dto = new WaitlistPatientDto { Birthday = new DateOnly(1990, 12, 25) };
        var age = dto.CalcAge(new DateTime(2024, 12, 1));
        Assert.Equal(33, age);
    }

    [Fact]
    public void CalcAge_LeapYearBirthday_ShouldHandleCorrectly()
    {
        var dto = new WaitlistPatientDto { Birthday = new DateOnly(2000, 2, 29) };
        var age = dto.CalcAge(new DateTime(2024, 2, 28));
        Assert.Equal(23, age);
    }

    [Fact]
    public void CalcAge_OnBirthday_ShouldReturnCorrectAge()
    {
        var dto = new WaitlistPatientDto { Birthday = new DateOnly(1990, 5, 15) };
        var age = dto.CalcAge(new DateTime(2024, 5, 15));
        Assert.Equal(34, age);
    }

    [Fact]
    public void HasTherapyTimesMorning_NullArray_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto { TherapyTimes = null };
        Assert.True(dto.HasTherapyTimesMorning);
    }

    [Fact]
    public void HasTherapyTimesMorning_WithMorningEntry_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsMorning = true }]
        };
        Assert.True(dto.HasTherapyTimesMorning);
    }

    [Fact]
    public void HasTherapyTimesMorning_WithMorningTime_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { Time = "09:00" }]
        };
        Assert.True(dto.HasTherapyTimesMorning);
    }

    [Fact]
    public void HasTherapyTimesMorning_OnlyAfternoon_ShouldReturnFalse()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsAfternoon = true, Time = "14:00" }]
        };
        Assert.False(dto.HasTherapyTimesMorning);
    }

    [Fact]
    public void HasTherapyTimesAfternoon_NullArray_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto { TherapyTimes = null };
        Assert.True(dto.HasTherapyTimesAfternoon);
    }

    [Fact]
    public void HasTherapyTimesAfternoon_WithAfternoonEntry_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsAfternoon = true }]
        };
        Assert.True(dto.HasTherapyTimesAfternoon);
    }

    [Fact]
    public void HasTherapyTimesAfternoon_OnlyMorning_ShouldReturnFalse()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsMorning = true, Time = "09:00" }]
        };
        Assert.False(dto.HasTherapyTimesAfternoon);
    }

    [Fact]
    public void HasTherapyTimesHomeVisit_NullArray_ShouldReturnFalse()
    {
        var dto = new WaitlistPatientDto { TherapyTimes = null };
        Assert.False(dto.HasTherapyTimesHomeVisit);
    }

    [Fact]
    public void HasTherapyTimesHomeVisit_WithHomeVisit_ShouldReturnTrue()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsHomeVisit = true }]
        };
        Assert.True(dto.HasTherapyTimesHomeVisit);
    }

    [Fact]
    public void HasTherapyTimesHomeVisit_WithoutHomeVisit_ShouldReturnFalse()
    {
        var dto = new WaitlistPatientDto
        {
            TherapyTimes = [new TherapyTimesDto { IsMorning = true }]
        };
        Assert.False(dto.HasTherapyTimesHomeVisit);
    }
}
