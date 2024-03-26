namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class ChangeMyEmailHandler : IRequestHandler<ChangeMyEmailCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IIdentityService _identityService;
    private readonly IEmailValidatorService _emailValidatorService;

    public ChangeMyEmailHandler(
        IdentityErrorDescriber errorDescriber,
        IEmailQueueService emailQueueService,
        IIdentityService identityService,
        IEmailValidatorService emailValidatorService)
    {
        _errorDescriber = errorDescriber;
        _emailQueueService = emailQueueService;
        _identityService = identityService;
        _emailValidatorService = emailValidatorService;
    }

    public async ValueTask<Result> Handle(ChangeMyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUser(request.Id);
        if (user is null)
        {
            return Result.Fail(Messages.UserNotFound);
        }

        var result = await _identityService.CheckPassword(request.Id, request.CurrentPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        var isValid = await _emailValidatorService.Validate(request.NewEmail, cancellationToken);
        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidEmail(request.NewEmail).Description);
        }

        var token = await _identityService.GenerateChangeEmailToken(request.Id, request.NewEmail);
        if (token is null)
        {
            return Result.Fail(_errorDescriber.DefaultError().Description);
        }

        var callbackUrl = new UriBuilder(request.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}&email={request.NewEmail}"
        }.ToString();

        await _emailQueueService.EnqueueChangeEmail(request.NewEmail, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
