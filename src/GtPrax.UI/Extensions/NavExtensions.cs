using GtPrax.UI.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace GtPrax.UI.Extensions;

public static class NavExtensions
{
    public static IHtmlContent CreateBreadcrumb(this IHtmlHelper helper, params object[] routeValues)
    {
        var nav = new TagBuilder("nav");
        nav.AddCssClass("breadcrumb has-arrow-separator");
        nav.Attributes.Add("aria-label", "breadcrumbs");

        var breadcrumbGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<NodeGenerator>();
        var linkGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

        if (!helper.ViewContext.ActionDescriptor.RouteValues.TryGetValue("page", out var page)) throw new InvalidOperationException($"Current route isn't a Razor page");

        var node = breadcrumbGenerator.GetNode(page);
        var routeEnum = routeValues.GetEnumerator();
        var htmlContent = new StringBuilder();

        while (node != null)
        {
            var path = linkGenerator.GetPathByPage(node.Page, null, routeEnum.MoveNext() ? routeEnum.Current : null);

            if (htmlContent.Length < 1) // the first is the active one
            {
                htmlContent.Insert(0, $"<li class=\"is-active\"><a href=\"{path}\" aria-current=\"page\">{node.Title}</a></li>");
            }
            else
            {
                htmlContent.Insert(0, $"<li><a href=\"{path}\">{node.Title}</a></li>");
            }
            node = node.Parent;
        }

        htmlContent.Insert(0, "<ul>");
        htmlContent.Append("</ul>");

        nav.InnerHtml.AppendHtml(htmlContent.ToString());

        return nav;
    }
}
