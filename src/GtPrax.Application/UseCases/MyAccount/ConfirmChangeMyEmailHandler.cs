namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class ConfirmChangeMyEmailHandler : IRequestHandler<ConfirmChangeMyEmailCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ConfirmChangeMyEmailHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(ConfirmChangeMyEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ChangeEmail(request.Id, request.Token, request.NewEmail);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }
        return Result.Ok();
    }
}
