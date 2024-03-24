namespace GtPrax.Application.UseCases.UsersManagement;

using System;
using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Identity;
using GtPrax.Application.Mappings;
using Mediator;

internal sealed class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery, UserDto[]>
{
    private readonly TimeProvider _timeProvider;
    private readonly IIdentityService _identityService;

    public GetAllUsersHandler(
        TimeProvider timeProvider,
        IIdentityService identityService)
    {
        _timeProvider = timeProvider;
        _identityService = identityService;
    }

    public async ValueTask<UserDto[]> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _identityService.GetAllUsers(cancellationToken);
        if (users.Length < 1)
        {
            return [];
        }
        var dateTimeConverter = new GermanDateTimeConverter();
        return users.Select(u => u.MapToDto(dateTimeConverter, _timeProvider)).ToArray();
    }
}
