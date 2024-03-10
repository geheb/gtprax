namespace GtPrax.UI.Services;

internal sealed class CspOptions
{
    public ICollection<string> Defaults { get; set; } = new List<string>();
    public ICollection<string> Scripts { get; set; } = new List<string>();
    public ICollection<string> Styles { get; set; } = new List<string>();
    public ICollection<string> Images { get; set; } = new List<string>();
    public ICollection<string> Fonts { get; set; } = new List<string>();
    public ICollection<string> Media { get; set; } = new List<string>();
    public ICollection<string> Frame { get; set; } = new List<string>();
    public ICollection<string> Connect { get; set; } = new List<string>();
    public ICollection<string> Worker { get; set; } = new List<string>();
}
