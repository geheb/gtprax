namespace GtPrax.UI.Services;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

public sealed class NodeGenerator
{
    private const string RazorPagesRootDirectory = "Pages";
    private static bool IsRazorPage(Type type) => type != null && typeof(PageModel).IsAssignableFrom(type);
    private readonly Dictionary<Type, Node> _nodes = [];

    public Node GetNode(string? page)
    {
        var node = _nodes.Values.First(n => n.Page.Equals(page, StringComparison.OrdinalIgnoreCase));
        return node;
    }

    public Node GetNode(Type pageType)
    {
        if (!IsRazorPage(pageType))
        {
            throw new InvalidOperationException($"'{pageType.FullName}' isn't a Razor page");
        }

        var node = _nodes[pageType];
        return node;
    }

    public void Add(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(IsRazorPage))
        {
            var node = CreateBreadcrumbNode(type);
            if (node != null)
            {
                _nodes.Add(node.Key, node);
            }
        }

        GenerateHierarchy(_nodes);
    }

    private static void GenerateHierarchy(Dictionary<Type, Node> entries)
    {
        if (entries.Count < 1)
        {
            return;
        }

        var defaultEntry = entries.Values.Single(e => e.IsDefault);
        var defaultNodeKey = defaultEntry.Key;

        foreach (var entry in entries.Values)
        {
            if (entry.IsDefault)
            {
                continue;
            }

            var fromKey = entry.FromKey ?? defaultNodeKey;
            if (!entries.TryGetValue(fromKey, out var node))
            {
                throw new InvalidOperationException($"No node exists that has a '{fromKey.FullName}' as a key. Make sure that Razor page has a {nameof(NodeAttribute)}");
            }

            entry.Parent = node;
        }
    }

    private static string ExtractRazorPage(Type pageType)
    {
        ArgumentNullException.ThrowIfNull(pageType);

        var fullName = pageType.FullName;
        var pagesIndex = fullName != null ? fullName.IndexOf($".{RazorPagesRootDirectory}.", StringComparison.OrdinalIgnoreCase) : -1;
        if (pagesIndex == -1)
        {
            throw new InvalidOperationException($"The full name {fullName} doesn't contain '{RazorPagesRootDirectory}'");
        }

        var startIndex = pagesIndex + RazorPagesRootDirectory.Length + 1;
        var endIndex = fullName!.EndsWith("Model", StringComparison.OrdinalIgnoreCase) ? fullName.Length - 5 : fullName.Length;
        return fullName[startIndex..endIndex].Replace('.', '/');
    }

    private static Node? CreateBreadcrumbNode(Type type)
    {
        var attr = type.GetCustomAttribute<NodeAttribute>();
        if (attr == null)
        {
            return null;
        }

        if (attr.FromPage != null && !IsRazorPage(attr.FromPage))
        {
            throw new InvalidOperationException($"FromPage '{attr.FromPage.FullName}' isn't a Razor Page");
        }

        var auth = type.GetCustomAttribute<AuthorizeAttribute>();

        //if no title is given, then fallback to type name (razor page name).
        if (string.IsNullOrWhiteSpace(attr.Title))
        {
            attr.Title = type.Name.Replace("Page", string.Empty);
        }

        var page = ExtractRazorPage(type);

        return new Node(type, page, attr.IsDefault, attr.Title)
        {
            FromKey = attr.FromPage,
            AllowedRoles = auth?.Roles?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToArray(),
            AllowedPolicy = auth?.Policy
        };
    }
}
