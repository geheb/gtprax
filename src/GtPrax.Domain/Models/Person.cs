namespace GtPrax.Domain.Models;

using System.Text.RegularExpressions;

public sealed class Person
{
    public string Name { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string PhoneNumber { get; private set; } = null!;

    public Person(string name, DateOnly birthDate, string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        if (birthDate == DateOnly.MinValue || birthDate == DateOnly.MaxValue)
        {
            throw new ArgumentException("Invalid value", nameof(birthDate));
        }

        Name = name;
        BirthDate = birthDate;

        if (!Regex.IsMatch(phoneNumber, "^(\\d{4,16})$"))
        {
            throw new ArgumentException("Invalid value", nameof(phoneNumber));
        }

        PhoneNumber = phoneNumber;
    }

    public Person SetPhoneNumber(string phoneNumber) => new(Name, BirthDate, phoneNumber);
}
