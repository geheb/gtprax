namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class EmailFieldAttribute : DataTypeAttribute
{
    private readonly EmailAddressAttribute _emailAddress = new();

    public EmailFieldAttribute() : base(DataType.EmailAddress)
    {
        ErrorMessage = Messages.FieldIsInvalidEmail;
    }

    public override bool IsValid(object? value) => _emailAddress.IsValid(value);
}
