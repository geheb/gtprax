namespace GtPrax.Application.Models;

using System.Reflection;

public sealed class AppOptions
{
    public string Slogan { get; set; } = "Die Web-App hilft bei der Praxis-Organisation.";
    public string HeaderTitle { get; set; } = "GT Prax";
    public string Signature { get; set; } = "GT Prax";
    public string Version { get; set; }

    public AppOptions()
    {
        Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1";
    }
}
