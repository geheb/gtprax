namespace GtPrax.Application.UseCases.UserAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using GtPrax.Domain.ValueObjects;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class UpdateUserHandler : ICommandHandler<UpdateUserCommand, Result>
{
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailValidatorService _emailValidatorService;
    private readonly IUserService _userService;

    public UpdateUserHandler(
        IdentityErrorDescriber errorDescriber,
        IEmailValidatorService emailValidatorService,
        IUserService userService)
    {
        _errorDescriber = errorDescriber;
        _emailValidatorService = emailValidatorService;
        _userService = userService;
    }

    public async ValueTask<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userService.Find(command.Id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        Result result;

        if (!string.IsNullOrWhiteSpace(command.Name) && command.Name != user.Name)
        {
            result = await _userService.SetName(command.Id, command.Name, cancellationToken);
            if (result.IsFailed)
            {
                return result;
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Email) && command.Email != user.Email)
        {
            if (!await _emailValidatorService.Validate(command.Email, cancellationToken))
            {
                return Result.Fail(_errorDescriber.InvalidEmail(command.Email).Description);
            }

            result = await _userService.SetEmail(command.Id, command.Email);
            if (result.IsFailed)
            {
                return result;
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            result = await _userService.SetPassword(command.Id, command.Password);
            if (result.IsFailed)
            {
                return result;
            }
        }

        var roles = UserRoleType.From(command.Roles.Cast<int>());
        if (!user.Roles.SequenceEqual(roles))
        {
            result = await _userService.SetRoles(command.Id, roles);
            if (result.IsFailed)
            {
                return result;
            }
        }

        return Result.Ok();
    }
}
