namespace GtPrax.UI.Services;

internal sealed class CspOptionsBuilder
{
    private readonly CspOptions _options = new();

    public CspDirectiveBuilder Defaults { get; } = new();
    public CspDirectiveBuilder Scripts { get; } = new();
    public CspDirectiveBuilder Styles { get; } = new();
    public CspDirectiveBuilder Images { get; } = new();
    public CspDirectiveBuilder Fonts { get; } = new();
    public CspDirectiveBuilder Media { get; } = new();
    public CspDirectiveBuilder Frame { get; } = new();
    public CspDirectiveBuilder Connect { get; } = new();
    public CspDirectiveBuilder Worker { get; } = new();


    internal CspOptions Build()
    {
        _options.Defaults = Defaults.Sources;
        _options.Scripts = Scripts.Sources;
        _options.Styles = Styles.Sources;
        _options.Images = Images.Sources;
        _options.Fonts = Fonts.Sources;
        _options.Media = Media.Sources;
        _options.Frame = Frame.Sources;
        _options.Connect = Connect.Sources;
        _options.Worker = Worker.Sources;
        return _options;
    }
}
