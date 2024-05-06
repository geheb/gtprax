namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class ResetUserTwoFactorHandler : ICommandHandler<ResetUserTwoFactorCommand, Result>
{
    private readonly IUserService _userService;

    public ResetUserTwoFactorHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(ResetUserTwoFactorCommand command, CancellationToken cancellationToken) =>
        await _userService.ResetTwoFactor(command.Id);
}
