namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class UpdateMyUserHandler : IRequestHandler<UpdateMyUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateMyUserHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(UpdateMyUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.SetName(request.Id, request.Name, cancellationToken);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
