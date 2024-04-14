namespace GtPrax.Domain.Entities;

public sealed class Person
{
    public PersonIdentity Identity { get; private set; }

    public string PhoneNumber { get; private set; }

    internal Person(PersonIdentity identity, string phoneNumber)
    {
        Identity = identity;
        PhoneNumber = phoneNumber;
    }
}
