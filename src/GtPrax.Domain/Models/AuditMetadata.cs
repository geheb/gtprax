namespace GtPrax.Domain.Models;

using System;

public sealed class AuditMetadata
{
    public DateTimeOffset CreatedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }

    internal AuditMetadata(string createdBy, DateTimeOffset createdDate, string? lastModifiedBy = null, DateTimeOffset? lastModifiedDate = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        if (lastModifiedDate is not null && lastModifiedDate < createdDate)
        {
            throw new ArgumentException("Invalid value", nameof(lastModifiedDate));
        }

        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
    }
}
