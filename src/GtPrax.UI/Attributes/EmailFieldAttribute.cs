namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class EmailFieldAttribute : DataTypeAttribute
{
    private readonly EmailAddressAttribute _emailAddress = new();

    public EmailFieldAttribute()
        : base(DataType.EmailAddress)
    {
        ErrorMessage = "Das Feld '{0}' ist keine gültige E-Mail-Adresse.";
    }

    public override bool IsValid(object? value) => _emailAddress.IsValid(value);
}
