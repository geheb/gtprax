namespace GtPrax.Application.UseCases.UsersManagement;

using Mediator;

public sealed record GetUserQuery(string Id) : IQuery<UserDto?>;
