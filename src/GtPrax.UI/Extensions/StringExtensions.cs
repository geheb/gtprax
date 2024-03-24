namespace GtPrax.UI.Extensions;

using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;

public static class StringExtensions
{
    public static string AnonymizeEmail(this string email)
    {
        var emailSplit = email.Split('@');
        if (emailSplit.Length != 2)
        {
            return string.Empty;
        }

        var emailUser = emailSplit[0];
        var idn = new IdnMapping();
        var emailDomain = idn.GetUnicode(emailSplit[1]);
        return emailUser[0] + "***@" + emailDomain;
    }

    public static string PageLinkName<T>() where T : PageModel => typeof(T).Name[..^5]; // remove suffix "Model"
}
