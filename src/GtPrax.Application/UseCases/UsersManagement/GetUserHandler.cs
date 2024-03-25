namespace GtPrax.Application.UseCases.UsersManagement;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Identity;
using GtPrax.Application.Mappings;
using Mediator;

internal sealed class GetUserHandler : IQueryHandler<GetUserQuery, UserDto?>
{
    private readonly TimeProvider _timeProvider;
    private readonly IIdentityService _identity;

    public GetUserHandler(
        TimeProvider timeProvider,
        IIdentityService identity)
    {
        _timeProvider = timeProvider;
        _identity = identity;
    }

    public async ValueTask<UserDto?> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _identity.FindUser(query.Id);
        if (user is null)
        {
            return null;
        }
        var dateConverter = new GermanDateTimeConverter();
        return user.MapToDto(dateConverter, _timeProvider);
    }
}
