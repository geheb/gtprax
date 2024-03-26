namespace GtPrax.Application.UseCases.UsersManagement;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class SendConfirmEmailHandler : IRequestHandler<SendConfirmEmailCommand, Result>
{
    private readonly IEmailQueueService _emailQueueService;
    private readonly IIdentityService _identityService;

    public SendConfirmEmailHandler(
        IEmailQueueService emailQueueService,
        IIdentityService identityService)
    {
        _emailQueueService = emailQueueService;
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUser(request.Id);
        if (user is null)
        {
            return Result.Fail(Messages.UserNotFound);
        }

        if (user.IsEmailConfirmed)
        {
            return Result.Fail(Messages.EmailIsConfirmed);
        }

        var token = await _identityService.GenerateConfirmEmailToken(user.Id);
        var callbackUrl = new UriBuilder(request.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueConfirmEmail(user.Email, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
