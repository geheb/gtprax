namespace GtPrax.Application.UseCases.UsersManagement;

using Mediator;

public sealed record GetAllUsersQuery() : IQuery<UserDto[]>;
