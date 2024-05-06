namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class SignInTwoFactorHandler : ICommandHandler<SignInTwoFactorCommand, Result>
{
    private readonly IUserService _userService;

    public SignInTwoFactorHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(SignInTwoFactorCommand command, CancellationToken cancellationToken) =>
        await _userService.SignInTwoFactor(command.Code, command.RememberClient, cancellationToken);
}
