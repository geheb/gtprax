namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class ChangeMyPasswordHandler : IRequestHandler<ChangeMyPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ChangeMyPasswordHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ChangePassword(request.Id, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
