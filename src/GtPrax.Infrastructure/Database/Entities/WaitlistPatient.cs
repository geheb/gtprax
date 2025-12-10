namespace GtPrax.Infrastructure.Database.Entities;

using System.Text.Json;
using GtPrax.Application.Converter;
using GtPrax.Application.Models;
using GtPrax.Infrastructure.Extensions;

internal sealed class WaitlistPatient
{
    public Guid Id { get; set; }
    public Waitlist? Waitlist { get; set; }
    public Guid? WaitlistId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset? Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public string? Name { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Reason { get; set; }
    public string? Doctor { get; set; }
    public string? TherapyTimes { get; set; }
    public string? Remark { get; set; }
    public string? Tags { get; set; }

    public WaitlistPatientDto ToDto(GermanDateTimeConverter dc)
    {
        TherapyTimesDto[]? therapyTimes = null;
        if (!string.IsNullOrEmpty(TherapyTimes))
        {
            var model = JsonSerializer.Deserialize<JsonListVersioned<TherapyTimesDto>>(TherapyTimes);
            therapyTimes = model?.Items;
        }

        TagsDto? tags = null;
        if (!string.IsNullOrEmpty(Tags))
        {
            var model = JsonSerializer.Deserialize<JsonItemVersioned<TagsDto>>(Tags);
            tags = model?.Item;
        }

        return new()
        {
            Id = Id,
            WaitlistId = WaitlistId,
            UserId = UserId,
            User = User?.Name ?? User?.UserName,
            Created = Created != null ? dc.ToLocal(Created.Value) : null,
            Updated = Updated != null ? dc.ToLocal(Updated.Value) : null,
            Name = Name,
            Birthday = Birthday,
            PhoneNumber = PhoneNumber,
            Reason = Reason,
            Doctor = Doctor,
            Remark = Remark,
            TherapyTimes = therapyTimes,
            Tags = tags
        };
    }

    internal bool FromDto(WaitlistPatientDto dto)
    {
        var count = 0;

        if (this.SetValue(m => m.WaitlistId, dto.WaitlistId))
        {
            count++;
        }

        if (this.SetValue(m => m.UserId, dto.UserId))
        {
            count++;
        }

        if (this.SetValue(m => m.Name, dto.Name))
        {
            count++;
        }

        if (this.SetValue(m => m.Birthday, dto.Birthday))
        {
            count++;
        }

        if (this.SetValue(m => m.PhoneNumber, dto.PhoneNumber))
        {
            count++;
        }

        if (this.SetValue(m => m.Reason, dto.Reason))
        {
            count++;
        }

        if (this.SetValue(m => m.Doctor, dto.Doctor))
        {
            count++;
        }

        if (this.SetValue(m => m.Remark, dto.Remark))
        {
            count++;
        }

        if (dto.TherapyTimes?.Length > 0)
        {
            var model = new JsonListVersioned<TherapyTimesDto>
            {
                Version = 1,
                Items = dto.TherapyTimes
            };
            var json = JsonSerializer.Serialize(model);
            if (this.SetValue(m => m.TherapyTimes, json))
            {
                count++;
            }
        }
        else
        {
            if (this.SetValue(m => m.TherapyTimes, null))
            {
                count++;
            }
        }

        if (dto.Tags != null)
        {
            var model = new JsonItemVersioned<TagsDto>
            {
                Version = 1,
                Item = dto.Tags
            };
            var json = JsonSerializer.Serialize(model);
            if (this.SetValue(m => m.Tags, json))
            {
                count++;
            }
        }
        else
        {
            if (this.SetValue(m => m.Tags, null))
            {
                count++;
            }
        }

        return count > 0;
    }
}
