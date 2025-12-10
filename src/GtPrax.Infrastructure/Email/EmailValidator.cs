namespace GtPrax.Infrastructure.Email;

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using DnsClient;
using GtPrax.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

internal sealed class EmailValidator : IEmailValidator
{
    private readonly string[] _knownProviderList =
    [
        "aol.com",
        "gmx.de",
        "gmx.net",
        "gmx.at",
        "gmx.ch",
        "gmail.com",
        "googlemail.com",
        "mail.com",
        "outlook.de",
        "outlook.com",
        "hotmail.com",
        "hotmail.de",
        "web.de",
        "yahoo.com",
        "yandex.com",
        "icloud.com",
        "me.com",
        "mac.com",
        "freenet.de",
        "t-online.de",
        "o2online.de",
        "posteo.de",
        "unitybox.de",
        "online.de"
    ];

    private readonly LookupClient _lookupClient;
    private readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;

    public EmailValidator(ILogger<EmailValidator> logger, IMemoryCache memoryCache)
    {
        var options = new LookupClientOptions
        {
            UseCache = true
        };
        _lookupClient = new LookupClient(options);
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<bool> Validate(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        email = new IdnMapping().GetAscii(email);

        var emailParts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);
        if (emailParts.Length < 2)
        {
            return false;
        }

        var domain = emailParts[1];

        if (_knownProviderList.Any(e => e.Equals(domain, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (_memoryCache.TryGetValue(domain, out bool isValid))
        {
            if (!isValid)
            {
                _logger.LogError("suspicous domain {Domain} found", domain);
            }

            return isValid;
        }

        _logger.LogInformation("query domain {Domain}", domain);

        var response = await _lookupClient.QueryAsync(domain, QueryType.MX, cancellationToken: cancellationToken);
        if (response.HasError)
        {
            _logger.LogWarning("query domain {Domain} failed: {ErrorMessage}", domain, response.ErrorMessage);
            return true;
        }

        if (response.Answers.Count < 1)
        {
            _logger.LogError("suspicous domain {Domain} found", domain);
            _memoryCache.Set(domain, false, DateTimeOffset.UtcNow.AddHours(1));
            return false;
        }

        var count = 0;

        foreach (var answer in response.Answers.MxRecords())
        {
            var host = answer.Exchange.Value;
            var entry = await _lookupClient.GetHostEntryAsync(host);
            if (entry == null || entry.AddressList.Length < 1)
            {
                continue;
            }

            foreach (var addr in entry.AddressList)
            {
                if (IPAddress.IsLoopback(addr))
                {
                    continue;
                }

                if (addr.AddressFamily is not AddressFamily.InterNetwork
                    and not AddressFamily.InterNetworkV6)
                {
                    continue;
                }

                count++;

                string ipAddressReversed;
                if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    var expand = string.Concat(addr.GetAddressBytes().Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
                    ipAddressReversed = string.Join('.', expand.ToCharArray().Reverse());
                }
                else
                {
                    ipAddressReversed = string.Join(".", addr.GetAddressBytes().Reverse());
                }

                var queryDns = ipAddressReversed + ".zen.spamhaus.org";
                entry = await _lookupClient.GetHostEntryAsync(queryDns);
                if (entry == null || entry.AddressList.Length < 1)
                {
                    continue;
                }

                if (entry.AddressList.Any(e => e.ToString().StartsWith("127.0.0", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogError("domain {Domain} is listed at spamhaus", domain);
                    _memoryCache.Set(domain, false, DateTimeOffset.UtcNow.AddHours(1));
                    return false;
                }
            }
        }

        if (count < 1)
        {
            _logger.LogError("suspicous domain {Domain} found", domain);
            _memoryCache.Set(domain, false, DateTimeOffset.UtcNow.AddHours(1));
            return false;
        }
        else
        {
            _memoryCache.Set(domain, true, DateTimeOffset.UtcNow.AddHours(1));
        }

        return true;
    }
}
