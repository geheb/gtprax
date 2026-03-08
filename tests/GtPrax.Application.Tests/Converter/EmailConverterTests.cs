namespace GtPrax.Application.Tests.Converter;

using GtPrax.Application.Converter;
using Xunit;

public sealed class EmailConverterTests
{
    private readonly EmailConverter _sut = new();

    [Fact]
    public void Anonymize_ShouldMaskUsernameAndConvertIdnDomain()
    {
        var result = _sut.Anonymize("test@example.com");
        Assert.Equal("t***@example.com", result);
    }

    [Fact]
    public void Anonymize_SingleCharUsername_ShouldMask()
    {
        var result = _sut.Anonymize("a@example.com");
        Assert.Equal("a***@example.com", result);
    }

    [Fact]
    public void Normalize_ShouldPreserveUsernameAndConvertIdnDomain()
    {
        var result = _sut.Normalize("test@example.com");
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void Normalize_IdnDomain_ShouldConvertToUnicode()
    {
        var result = _sut.Normalize("user@xn--mxahbxey0c.com");
        Assert.Equal("user@εχαμπλε.com", result);
    }

    [Fact]
    public void Anonymize_IdnDomain_ShouldConvertToUnicode()
    {
        var result = _sut.Anonymize("user@xn--mxahbxey0c.com");
        Assert.Equal("u***@εχαμπλε.com", result);
    }
}
