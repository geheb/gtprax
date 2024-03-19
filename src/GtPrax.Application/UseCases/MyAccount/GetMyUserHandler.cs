namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Identity;
using Mediator;

internal sealed class GetMyUserHandler : IQueryHandler<GetMyUserQuery, MyUserDto?>
{
    private readonly IIdentityService _identityService;

    public GetMyUserHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<MyUserDto?> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUser(request.Id);
        if (user is null)
        {
            return null;
        }
        return new(user.Name, user.Email);
    }
}
