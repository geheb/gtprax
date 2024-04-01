namespace GtPrax.Domain.Entities;

using System;

public sealed class PersonIdentity
{
    public string Name { get; private set; }
    public DateOnly BirthDate { get; private set; }

    public PersonIdentity(string name, DateOnly birthDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (birthDate == DateOnly.MinValue || birthDate == DateOnly.MaxValue)
        {
            throw new ArgumentException("Invalid birth date");
        }
        Name = name;
        BirthDate = birthDate;
    }
}
