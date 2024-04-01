namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class ConfirmResetPasswordHandler : ICommandHandler<ConfirmResetPasswordCommand, Result>
{
    private readonly IUserService _userService;

    public ConfirmResetPasswordHandler(
        IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(ConfirmResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.ResetPassword(command.Id, command.Token, command.NewPassword);
        return result;
    }
}
