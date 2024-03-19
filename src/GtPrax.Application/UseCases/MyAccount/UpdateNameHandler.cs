namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class UpdateNameHandler : IRequestHandler<UpdateNameCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateNameHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result> Handle(UpdateNameCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.SetName(request.Id, request.Name, cancellationToken);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
