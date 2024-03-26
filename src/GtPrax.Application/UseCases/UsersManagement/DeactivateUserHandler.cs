namespace GtPrax.Application.UseCases.UsersManagement;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public DeactivateUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.Deactivate(request.Id);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
