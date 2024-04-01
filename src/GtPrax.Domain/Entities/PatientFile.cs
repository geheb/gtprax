namespace GtPrax.Domain.Entities;

using System;

public sealed class PatientFile : IEquatable<PatientFile>
{
    public string Id { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public Person Person { get; private set; }

    public PatientFile(string id, string issuer, DateTimeOffset now, Person person)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        Id = id;
        Audit = new(now, issuer);
        Person = person;
    }

    public bool Equals(PatientFile? other) => other is not null && Id == other.Id;

    public override bool Equals(object? obj) => Equals(obj as PatientFile);

    public override int GetHashCode() => Id.GetHashCode();
}
