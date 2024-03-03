using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GtPrax.UI.Extensions;

public static class ModalExtensions
{
    const string MODAL_START = @"
<div id=""{0}"" class=""modal"" aria-hidden=""true"">
    <div class=""modal-background""></div>
    <div class=""modal-card"">
        <header class=""modal-card-head"">
            <p class=""modal-card-title"">{1}</p>
            <button class=""delete close-modal"" aria-label=""close""></button>
        </header>
        <section class=""modal-card-body"">
";

    const string MODAL_OK_END = @"
            <div class=""fa-3x loading-value has-text-centered is-hidden"">
                <i class=""fas fa-spinner fa-spin""></i>
            </div>
        </section>
        <footer class=""modal-card-foot"">
            <button class=""button close-modal"" aria-label=""close"">OK</button>
        </footer>
    </div>
</div>
";

    const string MODAL_OK_CANCEL_END = @"
            <div class=""fa-3x loading-value has-text-centered is-hidden"">
                <i class=""fas fa-spinner fa-spin""></i>
            </div>
        </section>
        <footer class=""modal-card-foot"">
            <button class=""button is-danger confirm"" disabled>OK</button>
            <button class=""button close-modal"" aria-label=""close"">Abbrechen</button>
        </footer>
    </div>
</div>
";

    public static IHtmlContent CreateModalTemplateStart(this IHtmlHelper htmlHelper, string id, string title)
    {
        return htmlHelper.Raw(string.Format(MODAL_START, id, title));
    }

    public static IHtmlContent CreateModalTemplateEnd(this IHtmlHelper htmlHelper, bool showOkAndCancel)
    {
        return htmlHelper.Raw(showOkAndCancel ? MODAL_OK_CANCEL_END : MODAL_OK_END);
    }

    public static IHtmlContent CreateModalTemplateWithOkCancel(this IHtmlHelper htmlHelper, string id, string title, string body)
    {
        return htmlHelper.Raw(string.Format(MODAL_START, id, title) + body + MODAL_OK_CANCEL_END);
    }
}
