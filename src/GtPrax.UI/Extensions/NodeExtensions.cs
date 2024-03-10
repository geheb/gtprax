namespace GtPrax.UI.Extensions;

using GtPrax.UI.Models;
using GtPrax.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

public static class NodeExtensions
{
    public static void UseNodeGenerator(this IApplicationBuilder app, Assembly assembly)
    {
        var nodeGenerator = app.ApplicationServices.GetRequiredService<NodeGeneratorService>();
        nodeGenerator.Add(assembly);
    }

    public static Node GetNode(this PageModel model)
    {
        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<NodeGeneratorService>();

        return breadcrumbGenerator.GetNode(model.GetType());
    }

    public static Node GetNode<T>(this PageModel model) where T : PageModel
    {
        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<NodeGeneratorService>();
        return breadcrumbGenerator.GetNode(typeof(T));
    }

    public static bool HasNodeAccess(this PageModel model, params Type[] pageModels)
    {
        if (!model.User.Identity?.IsAuthenticated ?? false)
        {
            return false;
        }

        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<NodeGeneratorService>();

        var nodes = pageModels.Select(t => breadcrumbGenerator.GetNode(t)).ToArray();
        foreach (var node in nodes)
        {
            var hasRole =
                node.AllowedRoles == null ||
                node.AllowedRoles.Length == 0 ||
                node.AllowedRoles.Any(r => model.User.IsInRole(r));

            if (hasRole)
            {
                return true;
            }
        }

        return nodes.Length < 1;
    }

    public static void BuildHeaderMenu(this PageModel model, object? routeValues = null)
    {
        if (model.User.Identity == null ||
            !model.User.Identity.IsAuthenticated)
        {
            return;
        }

        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<NodeGeneratorService>();
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
}
