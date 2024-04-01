namespace GtPrax.Application.UseCases.UserAccounts;

using Mediator;

public sealed record GetAllUsersQuery() : IQuery<UserDto[]>;
