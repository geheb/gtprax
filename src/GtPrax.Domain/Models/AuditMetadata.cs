namespace GtPrax.Domain.Models;

using System;

public sealed class AuditMetadata
{
    public DateTimeOffset CreatedDate { get; private set; }
    public string CreatedById { get; private set; }
    public DateTimeOffset? LastModifiedDate { get; private set; }
    public string? LastModifiedById { get; private set; }

    internal AuditMetadata(string createdById, DateTimeOffset createdDate, string? lastModifiedById = null, DateTimeOffset? lastModifiedDate = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdById);
        if (lastModifiedDate is not null && lastModifiedDate < createdDate)
        {
            throw new ArgumentException("Invalid value", nameof(lastModifiedDate));
        }

        CreatedById = createdById;
        CreatedDate = createdDate;
        LastModifiedById = lastModifiedById;
        LastModifiedDate = lastModifiedDate;
    }
}
