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

    public void SetLastModified(DateTimeOffset modifiedDate, string modifiedBy)
    {
        if (modifiedDate == DateTimeOffset.MinValue || modifiedDate == DateTimeOffset.MaxValue || modifiedDate < CreatedDate)
        {
            throw new ArgumentException("Invalid date");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(modifiedBy);
        LastModifiedDate = modifiedDate;
        LastModifiedBy = modifiedBy;
    }
}
