namespace GtPrax.Infrastructure.Stores;

using GtPrax.Domain.Entities;
using MongoDB.Bson;

internal static class WaitingListMapping
{
    public static WaitingListModel MapToModel(this WaitingList entity) =>
        new()
        {
            Id = string.IsNullOrEmpty(entity.Identity.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(entity.Identity.Id),
            Name = entity.Identity.Name,
            CreatedDate = entity.Audit.CreatedDate,
            CreatedBy = entity.Audit.CreatedBy
        };

    public static WaitingList MapToDomain(this WaitingListModel model) =>
        new(model.MapToIdentityDomain(), new(model.CreatedDate, model.CreatedBy));

    public static WaitingList[] MapToDomain(this IEnumerable<WaitingListModel> models) =>
        models.Select(m => m.MapToDomain()).ToArray();

    public static WaitingListIdentity MapToIdentityDomain(this WaitingListModel model) =>
        new(model.Id.ToString(), model.Name);

    public static WaitingListIdentity[] MapToIdentityDomain(this IEnumerable<WaitingListModel> models) =>
        models.Select(m => m.MapToIdentityDomain()).ToArray();
}
