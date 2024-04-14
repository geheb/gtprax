namespace GtPrax.Domain.Entities;

using System;

public sealed class AuditMetadata
{
    public DateTimeOffset CreatedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }

    internal AuditMetadata(string createdBy, DateTimeOffset createdDate, string? lastModifiedBy, DateTimeOffset? lastModifiedDate)
    {
        CreatedBy = createdBy;
        CreatedDate = createdDate;
        LastModifiedBy = lastModifiedBy;
        LastModifiedDate = lastModifiedDate;
    }
}
