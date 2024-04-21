namespace GtPrax.Infrastructure.Repositories;

using GtPrax.Domain.Models;
using MongoDB.Bson;

internal static class WaitingListMapping
{
    public static WaitingListModel MapToModel(this WaitingListItem entity) =>
        new()
        {
            Id = ObjectId.Parse(entity.Id),
            Name = entity.Name,
            CreatedDate = entity.Audit.CreatedDate,
            CreatedById = ObjectId.Parse(entity.Audit.CreatedBy)
        };

    public static WaitingListItem MapToDomain(this WaitingListModel model) =>
        new(model.Id.ToString(),
            model.Name,
            model.CreatedById.ToString(),
            model.CreatedDate);

    public static WaitingListItem[] MapToDomain(this IEnumerable<WaitingListModel> models) =>
        models.Select(m => m.MapToDomain()).ToArray();
}
