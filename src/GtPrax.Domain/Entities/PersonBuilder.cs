namespace GtPrax.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentResults;

public sealed class PersonBuilder
{
    private string? _name;
    private DateOnly? _birthDate;
    private string? _phoneNumber;

    public PersonBuilder()
    {
    }

    public PersonBuilder(Person person)
    {
        _name = person.Identity.Name;
        _birthDate = person.Identity.BirthDate;
        _phoneNumber = person.PhoneNumber;
    }

    public PersonBuilder SetName(string name) => Set(() => _name = name);
    public PersonBuilder SetBirthDate(DateOnly birthDate) => Set(() => _birthDate = birthDate);
    public PersonBuilder SetPhoneNumber(string phoneNumber) => Set(() => _phoneNumber = phoneNumber);

    public Result<Person> Build(PersonIdentity[] personIdentities, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(personIdentities);

        var error = new List<string>();
        if (string.IsNullOrWhiteSpace(_name))
        {
            error.Add("Der Name wird benötigt.");
        }
        if (_birthDate is null)
        {
            error.Add("Das Geburtsdatum wird benötigt.");
        }
        else if (_birthDate == DateOnly.MinValue || _birthDate == DateOnly.MaxValue || _birthDate.Value.ToDateTime(TimeOnly.MinValue) > now)
        {
            error.Add("Das Geburtsdatum ist ungültig.");
        }
        if (string.IsNullOrWhiteSpace(_phoneNumber))
        {
            error.Add("Die Telefonnummer wird benötigt.");
        }
        else if (!Regex.IsMatch(_phoneNumber, "^(\\d{4,16})$"))
        {
            error.Add("Die Telefonnummer ist ungültig. Es werden min. 4 und max. 16 Zahlen benötigt.");
        }

        if (error.Count > 0)
        {
            return Result.Fail(error);
        }

        var exists = personIdentities.Any(p => p.Name == _name && p.BirthDate == _birthDate);
        if (exists)
        {
            return Result.Fail("Die Person (Name & Geburtsadatum) existiert bereits.");
        }

        return Result.Ok(new Person(new PersonIdentity(_name!, _birthDate!.Value), _phoneNumber!));
    }

    private PersonBuilder Set(Action action)
    {
        action();
        return this;
    }
}
