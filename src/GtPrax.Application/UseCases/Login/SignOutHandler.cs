namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class SignOutHandler : ICommandHandler<SignOutCommand, Result>
{
    private readonly IUserService _userService;

    public SignOutHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        await _userService.SignOutCurrent();
        return Result.Ok();
    }
}
