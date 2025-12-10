namespace GtPrax.Infrastructure.AspNetCore;

using System.Globalization;
using System.Text;
using GtPrax.Application.Models;
using GtPrax.Application.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class HtmlHelperExtensions
{
    private const string MODAL_START = @"
<div id=""{0}"" class=""modal"" aria-hidden=""true"">
    <div class=""modal-background""></div>
    <div class=""modal-card"">
        <header class=""modal-card-head"">
            <p class=""modal-card-title"">{1}</p>
            <button class=""delete close-modal"" aria-label=""close""></button>
        </header>
        <section class=""modal-card-body"">
";

    private const string MODAL_OK_END = @"
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

    private const string MODAL_OK_CANCEL_END = @"
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

    public static IHtmlContent AjaxCsrfToken(this IHtmlHelper helper)
    {
        var antiForgery = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
        var tokens = antiForgery.GetTokens(helper.ViewContext.HttpContext);
        return new HtmlString($"{tokens.FormFieldName}:'{tokens.RequestToken}'");
    }

    public static string AppendVersion(this IHtmlHelper helper, string path) =>
        helper.ViewContext.HttpContext
            .RequestServices
            .GetRequiredService<IFileVersionProvider>()
            .AddFileVersionToPath(helper.ViewContext.HttpContext.Request.PathBase, path);

    public static IHtmlContent CreateBreadcrumb(this IHtmlHelper helper, params object[] routeValues)
    {
        var nav = new TagBuilder("nav");
        nav.AddCssClass("breadcrumb");
        nav.Attributes.Add("aria-label", "breadcrumbs");

        var breadcrumbGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<INodeGenerator>();
        var linkGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

        if (helper.ViewContext.ActionDescriptor is not CompiledPageActionDescriptor actionDescriptor)
        {
            throw new InvalidOperationException($"Current route isn't a Razor page");
        }

        var currentNode = breadcrumbGenerator.GetNode(actionDescriptor.HandlerTypeInfo);
        var routeEnum = routeValues.GetEnumerator();
        var nodes = new List<(NodeDto, string?)>();

        while (currentNode != null)
        {
            var path = linkGenerator.GetPathByPage(currentNode.Page, null, routeEnum.MoveNext() ? routeEnum.Current : null);
            nodes.Add((currentNode, path));
            currentNode = currentNode.Parent;
        }

        nodes.Reverse();
        var htmlContent = new StringBuilder("<ul>");

        for (var i = 0; i < nodes.Count; i++)
        {
            var (node, path) = nodes[i];

            if (nodes.Count > 2 && i > 0 && i < nodes.Count - 1)
            {
                htmlContent.AppendFormat(CultureInfo.InvariantCulture, "<li><a href=\"{0}\">", path);
                htmlContent.AppendFormat(CultureInfo.InvariantCulture, "<span class=\"is-hidden-mobile\">{0}</span>", node.Title);
                htmlContent.Append("<span class=\"is-hidden-tablet\">...</span>");
                htmlContent.Append("</a></li>");
            }
            else
            {
                if (i == nodes.Count - 1)
                {
                    htmlContent.Append("<li class=\"is-active\">");
                    htmlContent.AppendFormat(CultureInfo.InvariantCulture, "<a href=\"{0}\" aria-current=\"page\">{1}</a>", path, node.Title);
                    htmlContent.Append("</li>");
                }
                else
                {
                    htmlContent.AppendFormat(CultureInfo.InvariantCulture, "<li><a href=\"{0}\">{1}</a></li>", path, node.Title);
                }
            }
        }
        htmlContent.Append("</ul>");

        nav.InnerHtml.AppendHtml(htmlContent.ToString());

        return nav;
    }

    public static string RandomFontawesomeFace(this IHtmlHelper helper)
    {
        var values = new[]
        {
                "fa-face-smile",
                "fa-face-smile-wink",
                "fa-face-smile-beam",
                "fa-face-grin-wink",
                "fa-face-grin-wide",
                "fa-face-grin-hearts",
                "fa-face-grin-stars"
            };
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    public static async Task<IHtmlContent> ReadTextContent(this IHtmlHelper helper, string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return helper.Raw(string.Empty);
        }
        var env = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var fullPath = path.StartsWith("~/", StringComparison.OrdinalIgnoreCase) ? Path.Combine(env.WebRootPath, path.Substring(2)) : path;
        if (!File.Exists(fullPath))
        {
            return helper.Raw(string.Empty);
        }
        return helper.Raw(await File.ReadAllTextAsync(fullPath));
    }


    public static void BuildHeaderMenu(this PageModel model, object? routeValues = null)
    {
        if (model.User.Identity == null ||
            !model.User.Identity.IsAuthenticated)
        {
            return;
        }

        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<INodeGenerator>();
        var linkGenerator = model.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

        var node = breadcrumbGenerator.GetNode(model.GetType());

        var path = linkGenerator.GetPathByPage(node.Page, null, routeValues);
        if (path == null)
        {
            return;
        }

        var menu = new MenuItem(node.Title, path);

        model.ViewData["HeaderMenu"] = menu;
    }

#pragma warning disable CA1863
    public static IHtmlContent CreateModalTemplateStart(this IHtmlHelper htmlHelper, string id, string title) =>
        htmlHelper.Raw(string.Format(CultureInfo.InvariantCulture, MODAL_START, id, title));

    public static IHtmlContent CreateModalTemplateEnd(this IHtmlHelper htmlHelper, bool showOkAndCancel) =>
        htmlHelper.Raw(showOkAndCancel ? MODAL_OK_CANCEL_END : MODAL_OK_END);

    public static IHtmlContent CreateModalTemplateWithOkCancel(this IHtmlHelper htmlHelper, string id, string title, string body) =>
        htmlHelper.Raw(string.Format(CultureInfo.InvariantCulture, MODAL_START, id, title) + body + MODAL_OK_CANCEL_END);
#pragma warning restore CA1863
}

