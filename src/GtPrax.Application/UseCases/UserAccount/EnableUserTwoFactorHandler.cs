namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class EnableUserTwoFactorHandler : ICommandHandler<EnableUserTwoFactorCommand, Result>
{
    private readonly IUserService _userService;

    public EnableUserTwoFactorHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(EnableUserTwoFactorCommand command, CancellationToken cancellationToken) =>
        await _userService.EnableTwoFactor(command.Id, command.Enable, command.Code);
}
