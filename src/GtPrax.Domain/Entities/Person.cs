namespace GtPrax.Domain.Entities;

using System;
using System.Text.RegularExpressions;
using FluentResults;

public sealed class Person
{
    public PersonIdentity Identity { get; private set; }

    public string PhoneNumber { get; private set; }

    public Person(PersonIdentity identity, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(identity);
        Identity = identity;
        PhoneNumber = phoneNumber;
    }

    public Result<Person> Create(string name, DateOnly birthDate, string phoneNumber, DateTimeOffset now, PersonIdentity[] identities)
    {
        if (birthDate > DateOnly.FromDateTime(now.DateTime))
        {
            return Result.Fail("Das Geburtsdatum ist ungültig.");
        }

        if (!Regex.IsMatch(phoneNumber, "^(\\d{4,16})$"))
        {
            return Result.Fail("Die Telefonnummer ist ungültig. Es werden 4-16 Zahlen benötigt.");
        }

        var exists = identities.Any(p => p.Name == name && p.BirthDate == birthDate);
        if (exists)
        {
            return Result.Fail("Die Person existiert bereits.");
        }

        return Result.Ok(new Person(new PersonIdentity(name, birthDate), phoneNumber));
    }
}
