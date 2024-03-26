namespace GtPrax.UI.Extensions;

using Microsoft.AspNetCore.Mvc.RazorPages;

public static class PageModelExtensions
{
    public static string PageLinkName<T>(this PageModel _) where T : PageModel
    {
        var target = typeof(T).FullName;
        var index = target!.IndexOf(".Pages.", StringComparison.Ordinal);
        var absolutePath = target[(index + 6)..].Replace('.', '/');
        return absolutePath[..^5]; // remove Model
    }
}
