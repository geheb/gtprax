namespace GtPrax.Application.UseCases.UserAccounts;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class ChangeMyPasswordHandler : ICommandHandler<ChangeMyPasswordCommand, Result>
{
    private readonly IUserService _userService;

    public ChangeMyPasswordHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(ChangeMyPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.ChangePassword(command.Id, command.CurrentPassword, command.NewPassword);
        return result;
    }
}
