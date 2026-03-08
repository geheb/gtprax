namespace GtPrax.Infrastructure.Tests.Email;

using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Security;
using NSubstitute;
using Xunit;

public sealed class EmailValidatorTests
{
    private readonly IIpReputationChecker _reputationChecker = Substitute.For<IIpReputationChecker>();
    private readonly EmailValidator _sut;

    public EmailValidatorTests()
    {
        _sut = new EmailValidator(_reputationChecker);
    }

    [Theory]
    [InlineData("user@gmail.com")]
    [InlineData("user@gmx.de")]
    [InlineData("user@web.de")]
    [InlineData("user@outlook.com")]
    public async Task Validate_KnownProvider_ShouldReturnTrue(string email)
    {
        var result = await _sut.Validate(email, CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public async Task Validate_UnknownDomain_NotListed_ShouldReturnTrue()
    {
        _reputationChecker.IsListedMx("custom-domain.com", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _sut.Validate("user@custom-domain.com", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_UnknownDomain_Listed_ShouldReturnFalse()
    {
        _reputationChecker.IsListedMx("spam-domain.com", Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _sut.Validate("user@spam-domain.com", CancellationToken.None);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_EmptyOrNull_ShouldReturnFalse(string? email)
    {
        var result = await _sut.Validate(email!, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task Validate_MalformedEmail_ShouldReturnFalse()
    {
        var result = await _sut.Validate("no-at-sign", CancellationToken.None);
        Assert.False(result);
    }
}
