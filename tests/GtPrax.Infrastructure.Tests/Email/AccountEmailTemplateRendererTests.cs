namespace GtPrax.Infrastructure.Tests.Email;

using GtPrax.Infrastructure.Email;
using Xunit;

public sealed class AccountEmailTemplateRendererTests
{
    private readonly AccountEmailTemplateRenderer _sut = new();

    [Theory]
    [InlineData(AccountEmailTemplate.ConfirmRegistration)]
    [InlineData(AccountEmailTemplate.ConfirmPasswordForgotten)]
    [InlineData(AccountEmailTemplate.ConfirmChangeEmail)]
    public async Task Render_AllTemplates_ShouldReturnNonEmptyHtml(AccountEmailTemplate template)
    {
        var model = new { link = "https://example.com/confirm" };
        var result = await _sut.Render(template, model);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("https://example.com/confirm", result);
    }

    [Fact]
    public async Task Render_InvalidTemplate_ShouldThrow()
    {
        var invalidTemplate = (AccountEmailTemplate)99;
        await Assert.ThrowsAsync<NotImplementedException>(() =>
            _sut.Render(invalidTemplate, new { }));
    }
}
