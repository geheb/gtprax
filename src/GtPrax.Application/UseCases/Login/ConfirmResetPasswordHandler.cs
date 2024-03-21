namespace GtPrax.Application.UseCases.Login;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class ConfirmResetPasswordHandler : IRequestHandler<ConfirmResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ConfirmResetPasswordHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(ConfirmResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPassword(request.UserId, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
