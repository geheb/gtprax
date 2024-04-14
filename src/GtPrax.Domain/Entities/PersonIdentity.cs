namespace GtPrax.Domain.Entities;

using System;

public sealed class PersonIdentity
{
    public string Name { get; private set; }
    public DateOnly BirthDate { get; private set; }

    public PersonIdentity(string name, DateOnly birthDate)
    {
        Name = name;
        BirthDate = birthDate;
    }
}
