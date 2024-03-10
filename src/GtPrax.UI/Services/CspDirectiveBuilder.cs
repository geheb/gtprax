namespace GtPrax.UI.Services;

internal sealed class CspDirectiveBuilder
{
    public ICollection<string> Sources { get; } = new List<string>();

    public CspDirectiveBuilder AllowSelf() => Allow("'self'");
    public CspDirectiveBuilder AllowSelfData() => Allow("'self' data:");
    public CspDirectiveBuilder AllowNone() => Allow("none");
    public CspDirectiveBuilder AllowAny() => Allow("*");
    public CspDirectiveBuilder AllowUnsafeInline() => Allow("'unsafe-inline'");
    public CspDirectiveBuilder AllowBlob() => Allow("blob:");

    public CspDirectiveBuilder Allow(string source)
    {
        Sources.Add(source);
        return this;
    }
}
