namespace GtPrax.Domain.Entities;

using System;

public sealed class PatientFile
{
    public string Id { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public Person Person { get; private set; }

    public PatientFile(string id, string issuer, DateTimeOffset now, Person person)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        Id = id;
        Audit = new(now, issuer);
        Person = person;
    }
}
