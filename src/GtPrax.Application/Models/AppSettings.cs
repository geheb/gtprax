namespace GtPrax.Application.Models;

using System.Reflection;

public sealed class AppSettings
{
    public string Version { get; }
    public string? Slogan { get; set; }
    public string HeaderTitle { get; set; } = "GT Prax";
    public string? Signature { get; set; } = "GT Prax";

    public AppSettings()
    {
        Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";
    }
}
