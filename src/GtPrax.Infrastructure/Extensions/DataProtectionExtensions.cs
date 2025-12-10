namespace GtPrax.Infrastructure.Extensions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

public static class DataProtectionExtensions
{
    /// <summary>
    /// Add certificate based DataProtection 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="appName"></param>
    /// <param name="configuration">Access to configuration values: 'PfxFile' and 'PfxPassword'</param>
    /// <returns></returns>
    public static IDataProtectionBuilder AddCertificate(this IDataProtectionBuilder builder, string appName, IConfiguration configuration)
    {
        var certFile = configuration.GetValue<string>("PfxFile");
        var certPass = configuration.GetValue<string>("PfxPassword");

        // openssl req -x509 -newkey rsa:4096 -keyout dataprotection.key -out dataprotection.crt -days 3650 -nodes -subj "/CN=app"
        // openssl pkcs12 -export -out dataprotection.pfx -inkey dataprotection.key -in dataprotection.crt -name "app"

        var protectionCert = X509CertificateLoader.LoadPkcs12CollectionFromFile(certFile!, certPass);

        var di = new DirectoryInfo(".dataprotection");
        if (!di.Exists)
        {
            di.Create();
        }

        return builder.SetApplicationName(appName)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
            .ProtectKeysWithCertificate(protectionCert[0])
            .PersistKeysToFileSystem(di);
    }
}
