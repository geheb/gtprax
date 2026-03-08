namespace GtPrax.Application.Converter;

using System.Globalization;

public class EmailConverter
{
    private readonly IdnMapping _idn = new();

    public string Anonymize(string email)
    {
        var emailSplit = email.Split('@');
        if (emailSplit.Length != 2)
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        var emailUser = emailSplit[0];
        var emailDomain = _idn.GetUnicode(emailSplit[1]);
        return emailUser[0] + "***@" + emailDomain;
    }

    public string Normalize(string email)
    {
        var emailSplit = email.Split('@');
        if (emailSplit.Length != 2)
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        return emailSplit[0] + "@" + _idn.GetUnicode(emailSplit[1]);
    }
}
