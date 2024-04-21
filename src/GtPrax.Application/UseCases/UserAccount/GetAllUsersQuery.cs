namespace GtPrax.Application.UseCases.UserAccount;

using Mediator;

public sealed record GetAllUsersQuery() : IQuery<UserDto[]>;
