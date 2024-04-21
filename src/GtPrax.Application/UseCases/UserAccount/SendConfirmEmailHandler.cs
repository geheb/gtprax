namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class SendConfirmEmailHandler : ICommandHandler<SendConfirmEmailCommand, Result>
{
    private readonly IEmailQueueService _emailQueueService;
    private readonly IUserService _userService;

    public SendConfirmEmailHandler(
        IEmailQueueService emailQueueService,
        IUserService userService)
    {
        _emailQueueService = emailQueueService;
        _userService = userService;
    }

    public async ValueTask<Result> Handle(SendConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.Find(command.Id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        if (user.IsEmailConfirmed)
        {
            return Result.Fail(Messages.EmailIsConfirmed);
        }

        var token = await _userService.GenerateConfirmEmailToken(user.Id);
        var callbackUrl = new UriBuilder(command.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueConfirmEmail(user.Email, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
