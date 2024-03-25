namespace GtPrax.Application.UseCases.UsersManagement;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using Mediator;
using Microsoft.AspNetCore.Identity;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IdentityErrorDescriber _identityErrorDescriber;
    private readonly IEmailValidatorService _emailValidatorService;
    private readonly IIdentityService _identityService;

    public UpdateUserHandler(
        IdentityErrorDescriber identityErrorDescriber,
        IEmailValidatorService emailValidatorService,
        IIdentityService identityService)
    {
        _identityErrorDescriber = identityErrorDescriber;
        _emailValidatorService = emailValidatorService;
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUser(request.Id);
        if (user is null)
        {
            return Result.Fail(Messages.UserNotFound);
        }

        IdentityResult result;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            result = await _identityService.SetName(request.Id, request.Name, cancellationToken);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (!await _emailValidatorService.Validate(request.Email, cancellationToken))
            {
                return Result.Fail(_identityErrorDescriber.InvalidEmail(request.Email).Description);
            }

            result = await _identityService.SetEmail(request.Id, request.Email);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            result = await _identityService.SetPassword(request.Id, request.Password);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        result = await _identityService.SetRoles(request.Id, request.Roles);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
