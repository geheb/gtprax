namespace GtPrax.Infrastructure.AspNetCore;

using GtPrax.Application.Models;
using GtPrax.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

public static class PageModelExtensions
{
    public static NodeDto GetNode(this PageModel model)
    {
        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<INodeGenerator>();
        return breadcrumbGenerator.GetNode(model.GetType());
    }

    public static NodeDto GetNode<T>(this PageModel model) where T : PageModel
    {
        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<INodeGenerator>();
        return breadcrumbGenerator.GetNode<T>();
    }

    public static bool HasNodeAccess(this PageModel model, params Type[] pageModels)
    {
        if (!model.User.Identity?.IsAuthenticated ?? false)
        {
            return false;
        }

        var breadcrumbGenerator = model.HttpContext.RequestServices.GetRequiredService<INodeGenerator>();

        var nodes = pageModels.Select(breadcrumbGenerator.GetNode).ToArray();
        foreach (var node in nodes)
        {
            var hasRole =
                node.AllowedRoles == null ||
                node.AllowedRoles.Length == 0 ||
                node.AllowedRoles.Any(model.User.IsInRole);

            if (hasRole)
            {
                return true;
            }
        }

        return nodes.Length < 1;
    }
}
