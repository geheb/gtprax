namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class ResetPasswordHandler : ICommandHandler<ResetPasswordCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IUserService _userService;
    private readonly IEmailQueueService _emailQueueService;

    public ResetPasswordHandler(
        IdentityErrorDescriber errorDescriber,
        IUserService userService,
        IEmailQueueService emailQueueService)
    {
        _errorDescriber = errorDescriber;
        _userService = userService;
        _emailQueueService = emailQueueService;
    }

    public async ValueTask<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByEmail(command.Email);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        var token = await _userService.GenerateResetPasswordToken(user.Id);
        if (token is null)
        {
            return Result.Fail(_errorDescriber.DefaultError().Description);
        }

        var callbackUrl = new UriBuilder(command.CallbackUrl)
        {
            Query = $"id={user.Id}&token={token}"
        }.ToString();

        await _emailQueueService.EnqueueResetPassword(user.Email, user.Name, callbackUrl, cancellationToken);

        return Result.Ok();
    }
}
