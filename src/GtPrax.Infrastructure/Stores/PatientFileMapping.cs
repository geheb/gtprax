namespace GtPrax.Infrastructure.Stores;

using GtPrax.Domain.Entities;
using MongoDB.Bson;

internal static class PatientFileMapping
{
    public static PatientFileModel MapToModel(this PatientFile entity) => new()
    {
        Id = string.IsNullOrEmpty(entity.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(entity.Id),
        WaitingListId = ObjectId.Parse(entity.WaitingListId),
        CreatedDate = entity.Audit.CreatedDate,
        CreatedBy = entity.Audit.CreatedBy,
        LastModifiedDate = entity.Audit.LastModifiedDate,
        LastModifiedBy = entity.Audit.LastModifiedBy,
        Name = entity.Person.Identity.Name,
        BirthDate = new DateTime(entity.Person.Identity.BirthDate, TimeOnly.MinValue, DateTimeKind.Unspecified),
        PhoneNumber = entity.Person.PhoneNumber,
    };

    public static PatientFile MapToDomain(this PatientFileModel model)
    {
        var audit = new AuditMetadata(model.CreatedDate, model.CreatedBy, model.LastModifiedDate, model.LastModifiedBy);
        var person = new Person(new PersonIdentity(model.Name, DateOnly.FromDateTime(model.BirthDate)), model.PhoneNumber);
        return new(model.Id.ToString(), audit, person);
    }

    public static PatientFile[] MapToDomain(this IEnumerable<PatientFileModel> models) =>
        models.Select(m => m.MapToDomain()).ToArray();

    public static PersonIdentity MapToIdentityDomain(this PatientFileModel model) =>
        new(model.Name, DateOnly.FromDateTime(model.BirthDate));

    public static PersonIdentity[] MapToIdentityDomain(this IEnumerable<PatientFileModel> models) =>
        models.Select(m => m.MapToIdentityDomain()).ToArray();
}
