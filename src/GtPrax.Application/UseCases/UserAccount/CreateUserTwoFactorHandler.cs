namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using Mediator;

internal sealed class CreateUserTwoFactorHandler : ICommandHandler<CreateUserTwoFactorCommand, Result<UserTwoFactorDto>>
{
    private readonly IUserService _userService;

    public CreateUserTwoFactorHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask<Result<UserTwoFactorDto>> Handle(CreateUserTwoFactorCommand command, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateTwoFactor(command.Id, command.AppName);
        if (result.IsFailed)
        {
            return result.ToResult();
        }
        return Result.Ok(result.Value.MapToDto());
    }
}
