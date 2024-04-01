namespace GtPrax.Infrastructure.Stores;

using GtPrax.Domain.Entities;
using MongoDB.Bson;

internal static class WaitingListMapping
{
    public static WaitingListModel MapToModel(this WaitingList entity, ObjectId id) =>
        new()
        {
            Id = id,
            Name = entity.Identity.Name
        };

    public static WaitingList MapToDomain(this WaitingListModel model) =>
        new(new WaitingListIdentity(model.Id.ToString(), model.Name));

    public static WaitingList[] MapToDomain(this IEnumerable<WaitingListModel> models) =>
        models.Select(m => m.MapToDomain()).ToArray();

    public static WaitingListIdentity MapToIdentityDomain(this WaitingListModel model) =>
        new(model.Id.ToString(), model.Name);

    public static WaitingListIdentity[] MapToIdentityDomain(this IEnumerable<WaitingListModel> models) =>
        models.Select(m => m.MapToIdentityDomain()).ToArray();
}
