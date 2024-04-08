namespace GtPrax.Domain.Entities;

using System;
using FluentResults;

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

    public static Result<PersonIdentity> Create(string name, DateOnly birthDate, DateTimeOffset now)
    {
        if (birthDate > DateOnly.FromDateTime(now.DateTime))
        {
            return Result.Fail("Das Geburtsdatum ist ungültig.");
        }

        return new PersonIdentity(name, birthDate);
    }
}
