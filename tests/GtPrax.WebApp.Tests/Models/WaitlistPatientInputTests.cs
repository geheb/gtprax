namespace GtPrax.WebApp.Tests.Models;

using GtPrax.Application.Models;
using GtPrax.WebApp.Models;
using Xunit;

public sealed class WaitlistPatientInputTests
{
    [Fact]
    public void ToDto_ShouldMapBasicFields()
    {
        var input = new WaitlistPatientInput
        {
            Name = " John Doe ",
            Birthday = "1990-05-15",
            PhoneNumber = " 1234567 ",
            Reason = " therapy ",
            Doctor = " Dr. Smith ",
            Remark = " some note "
        };

        var dto = input.ToDto();

        Assert.Equal("John Doe", dto.Name);
        Assert.Equal(new DateOnly(1990, 5, 15), dto.Birthday);
        Assert.Equal("1234567", dto.PhoneNumber);
        Assert.Equal("therapy", dto.Reason);
        Assert.Equal("Dr. Smith", dto.Doctor);
        Assert.Equal("some note", dto.Remark);
    }

    [Fact]
    public void ToDto_InvalidBirthday_ShouldSetNull()
    {
        var input = new WaitlistPatientInput { Birthday = "invalid" };
        var dto = input.ToDto();
        Assert.Null(dto.Birthday);
    }

    [Fact]
    public void ToDto_NoTags_ShouldSetTagsNull()
    {
        var input = new WaitlistPatientInput();
        var dto = input.ToDto();
        Assert.Null(dto.Tags);
    }

    [Fact]
    public void ToDto_WithTags_ShouldCreateTagsDto()
    {
        var input = new WaitlistPatientInput
        {
            IsPriority = true,
            IsNeurofeedback = true
        };

        var dto = input.ToDto();

        Assert.NotNull(dto.Tags);
        Assert.True(dto.Tags.IsPriority);
        Assert.True(dto.Tags.IsNeurofeedback);
        Assert.False(dto.Tags.IsJumper);
    }

    [Fact]
    public void ToDto_ShouldIncludeUserIdAndWaitlistId()
    {
        var userId = Guid.NewGuid();
        var waitlistId = Guid.NewGuid();
        var input = new WaitlistPatientInput
        {
            UserId = userId,
            WaitlistId = waitlistId
        };

        var dto = input.ToDto();

        Assert.Equal(userId, dto.UserId);
        Assert.Equal(waitlistId, dto.WaitlistId);
    }

    [Fact]
    public void FromDto_ShouldMapAllFields()
    {
        var dto = new WaitlistPatientDto
        {
            Name = "Jane",
            Birthday = new DateOnly(1985, 3, 20),
            PhoneNumber = "9876543",
            Reason = "reason",
            Doctor = "Dr. X",
            Remark = "remark",
            Tags = new TagsDto
            {
                IsPriority = true,
                IsSchool = true
            },
            TherapyTimes =
            [
                new TherapyTimesDto { DayOfWeek = DayOfWeek.Monday, IsMorning = true }
            ]
        };

        var input = new WaitlistPatientInput();
        input.FromDto(dto);

        Assert.Equal("Jane", input.Name);
        Assert.Equal("1985-03-20", input.Birthday);
        Assert.Equal("9876543", input.PhoneNumber);
        Assert.Equal("reason", input.Reason);
        Assert.Equal("Dr. X", input.Doctor);
        Assert.Equal("remark", input.Remark);
        Assert.True(input.IsPriority);
        Assert.True(input.IsSchool);
        Assert.False(input.IsJumper);
        Assert.True(input.TherapyTimes.Monday[0]);
    }

    [Fact]
    public void FromDto_NullTags_ShouldDefaultToFalse()
    {
        var dto = new WaitlistPatientDto { Tags = null };
        var input = new WaitlistPatientInput();
        input.FromDto(dto);

        Assert.False(input.IsPriority);
        Assert.False(input.IsJumper);
        Assert.False(input.IsNeurofeedback);
        Assert.False(input.IsSchool);
        Assert.False(input.IsDaycare);
        Assert.False(input.IsGroup);
    }
}
