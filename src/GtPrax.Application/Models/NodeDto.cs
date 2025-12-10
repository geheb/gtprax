namespace GtPrax.Application.Models;

using System.Diagnostics;

[DebuggerDisplay("{Page} - {Title}")]
public sealed class NodeDto
{
    public Type Key { get; set; }
    public Type? FromKey { get; set; }
    public string Page { get; set; }
    public bool IsDefault { get; set; }
    public NodeDto? Parent { get; set; }
    public string Title { get; set; }
    public string[]? AllowedRoles { get; set; }
    public string? AllowedPolicy { get; set; }

    public NodeDto(Type key, string page, bool isDefault, string title)
    {
        Key = key;
        Page = page;
        IsDefault = isDefault;
        Title = title;
    }
}
