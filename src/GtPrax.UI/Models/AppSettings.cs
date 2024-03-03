using System.Reflection;

namespace GtPrax.UI.Models;

public class AppSettings
{
    public string? Slogan { get; set; }
    public string? HeaderTitle { get; set; }
    public string? Signature { get; set; }
	public string Version { get; set; }

	public AppSettings()
	{
		Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1";
	}
}
