namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class ChangeMyEmailHandler : ICommandHandler<ChangeMyEmailCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IUserService _userService;
    private readonly IEmailValidatorService _emailValidatorService;

    public ChangeMyEmailHandler(
        IdentityErrorDescriber errorDescriber,
        IEmailQueueService emailQueueService,
        IUserService userService,
        IEmailValidatorService emailValidatorService)
    {
        _errorDescriber = errorDescriber;
        _emailQueueService = emailQueueService;
        _userService = userService;
        _emailValidatorService = emailValidatorService;
    }

    public async ValueTask<Result> Handle(ChangeMyEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.Find(command.Id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        var result = await _userService.CheckPassword(command.Id, command.CurrentPassword);
        if (result.IsFailed)
        {
            return result;
        }

        var isValid = await _emailValidatorService.Validate(command.NewEmail, cancellationToken);
        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidEmail(command.NewEmail).Description);
        }

        var token = await _userService.GenerateChangeEmailToken(command.Id, command.NewEmail);
        if (token is null)
        {
            return Result.Fail(_errorDescriber.DefaultError().Description);
        }

        var callbackUrl = new UriBuilder(command.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}&email={command.NewEmail}"
        }.ToString();

        await _emailQueueService.EnqueueChangeEmail(command.NewEmail, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
