namespace GtPrax.WebApp.Tests.Converter;

using GtPrax.Application.Models;
using GtPrax.WebApp.Converter;
using Xunit;

public sealed class RoleConverterTests
{
    private readonly RoleConverter _sut = new();

    [Theory]
    [InlineData(Roles.Admin, "is-danger")]
    [InlineData(Roles.Manager, "is-warning")]
    [InlineData(Roles.Staff, "is-info")]
    [InlineData("unknown", "")]
    public void RoleToClass_ShouldReturnCorrectCssClass(string role, string expected)
    {
        Assert.Equal(expected, _sut.RoleToClass(role));
    }

    [Theory]
    [InlineData(Roles.Admin, "Administrator")]
    [InlineData(Roles.Manager, "Manager")]
    [InlineData(Roles.Staff, "Mitarbeiter")]
    [InlineData("unknown", "")]
    public void RoleToName_ShouldReturnCorrectDisplayName(string role, string expected)
    {
        Assert.Equal(expected, _sut.RoleToName(role));
    }
}
