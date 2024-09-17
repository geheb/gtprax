namespace GtPrax.Domain.Models;

using System.Text.RegularExpressions;

public sealed class Person
{
    public string Name { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string PhoneNumber { get; private set; } = null!;

    public int CalcAge(DateTime today)
    {
        var age = today.Year - BirthDate.Year;
        var birthdayTime = BirthDate.ToDateTime(TimeOnly.MinValue);
        // Go back to the year in which the person was born in case of a leap year
        return birthdayTime > today.AddYears(-age) ? age - 1 : age;
    }

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

        if (!Regex.IsMatch(phoneNumber, "^(\\d{4,16})$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500)))
        {
            throw new ArgumentException("Invalid value", nameof(phoneNumber));
        }

        PhoneNumber = phoneNumber;
    }

    public Person SetPhoneNumber(string phoneNumber) => new(Name, BirthDate, phoneNumber);
}
