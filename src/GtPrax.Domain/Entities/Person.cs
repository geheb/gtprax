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
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        Identity = identity;
        PhoneNumber = phoneNumber;
    }

    public static Result<Person> Create(string name, DateOnly birthDate, string phoneNumber, DateTimeOffset now, PersonIdentity[] personIdentities)
    {
        var idResult = PersonIdentity.Create(name, birthDate, now);
        if (idResult.IsFailed)
        {
            return idResult.ToResult();
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, "^(\\d{4,16})$"))
        {
            return Result.Fail("Die Telefonnummer ist ungültig. Es werden 4-16 Zahlen benötigt.");
        }

        var exists = personIdentities.Any(p => p.Name == name && p.BirthDate == birthDate);
        if (exists)
        {
            return Result.Fail("Die Person existiert bereits.");
        }

        return Result.Ok(new Person(idResult.Value, phoneNumber));
    }
}
