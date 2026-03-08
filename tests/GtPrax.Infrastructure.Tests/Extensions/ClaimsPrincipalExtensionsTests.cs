namespace GtPrax.Infrastructure.Tests.Extensions;

using System.Security.Claims;
using GtPrax.Infrastructure.Extensions;
using Xunit;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetId_ValidGuidClaim_ShouldReturnGuid()
    {
        var id = Guid.NewGuid();
        var principal = CreatePrincipal(ClaimTypes.NameIdentifier, id.ToString());

        var result = principal.GetId();

        Assert.Equal(id, result);
    }

    [Fact]
    public void GetId_MissingClaim_ShouldReturnGuidEmpty()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        var result = principal.GetId();

        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void GetId_InvalidString_ShouldReturnGuidEmpty()
    {
        var principal = CreatePrincipal(ClaimTypes.NameIdentifier, "not-a-guid");

        var result = principal.GetId();

        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void GetEmail_ShouldReturnEmailValue()
    {
        var principal = CreatePrincipal(ClaimTypes.Email, "user@example.com");

        var result = principal.GetEmail();

        Assert.Equal("user@example.com", result);
    }

    private static ClaimsPrincipal CreatePrincipal(string type, string value)
    {
        var identity = new ClaimsIdentity([new Claim(type, value)]);
        return new ClaimsPrincipal(identity);
    }
}
