namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class SignInHandler : ICommandHandler<SignInCommand, Result<SignInAction>>
{
    private readonly IUserService _userService;

    public SignInHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result<SignInAction>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.SignIn(command.Email, command.Password, cancellationToken);
        return result;
    }
}
