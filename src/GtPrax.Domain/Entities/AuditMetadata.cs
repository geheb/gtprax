namespace GtPrax.Domain.Entities;

using System;

public sealed class AuditMetadata
{
    public DateTimeOffset CreatedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }

    public AuditMetadata(DateTimeOffset createdDate, string createdBy)
    {
        if (createdDate == DateTimeOffset.MinValue || createdDate == DateTimeOffset.MaxValue)
        {
            throw new ArgumentException("Invalid date");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        CreatedDate = createdDate;
        CreatedBy = createdBy;
    }

    public AuditMetadata(DateTimeOffset createdDate, string createdBy, DateTimeOffset? modifiedDate, string? modifiedBy)
        : this(createdDate, createdBy)
    {
        if (modifiedDate is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);
            SetLastModified(modifiedDate.Value, modifiedBy);
        }
    }

    public void SetLastModified(DateTimeOffset modifiedDate, string modifiedBy)
    {
        if (modifiedDate == DateTimeOffset.MinValue || modifiedDate == DateTimeOffset.MaxValue || modifiedDate < CreatedDate)
        {
            throw new ArgumentException("Invalid date");
        }
        LastModifiedDate = modifiedDate;
        LastModifiedBy = modifiedBy;
    }
}
