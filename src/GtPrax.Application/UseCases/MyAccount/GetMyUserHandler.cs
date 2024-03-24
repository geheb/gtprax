namespace GtPrax.Application.UseCases.MyAccount;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Identity;
using GtPrax.Application.Mappings;
using GtPrax.Application.UseCases.UsersManagement;
using Mediator;

internal sealed class GetMyUserHandler : IQueryHandler<GetMyUserQuery, UserDto?>
{
    private readonly TimeProvider _timeProvider;
    private readonly IIdentityService _identityService;

    public GetMyUserHandler(
        TimeProvider timeProvider,
        IIdentityService identityService)
    {
        _timeProvider = timeProvider;
        _identityService = identityService;
    }

    public async ValueTask<UserDto?> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindUser(request.UserId);
        if (user is null)
        {
            return null;
        }
        return user.MapToDto(new GermanDateTimeConverter(), _timeProvider);
    }
}
