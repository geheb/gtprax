namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IIdentityService _identityService;
    private readonly IEmailQueueService _emailQueueService;

    public ResetPasswordHandler(
        IdentityErrorDescriber errorDescriber,
        IIdentityService identityService,
        IEmailQueueService emailQueueService)
    {
        _errorDescriber = errorDescriber;
        _identityService = identityService;
        _emailQueueService = emailQueueService;
    }

    public async ValueTask<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUserByEmail(request.Email);
        if (user is null)
        {
            return Result.Fail("Der Benutzer wurde nicht gefunden.");
        }

        var token = await _identityService.GenerateResetPasswordToken(user.Id);
        if (token is null)
        {
            return Result.Fail(_errorDescriber.DefaultError().Description);
        }

        var callbackUrl = new UriBuilder(request.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueResetPassword(user.Email, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
