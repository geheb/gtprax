namespace GtPrax.Application.UseCases.MyAccount;

using Mediator;

public sealed record GetMyUserQuery(string Id) : IQuery<MyUserDto?>;
