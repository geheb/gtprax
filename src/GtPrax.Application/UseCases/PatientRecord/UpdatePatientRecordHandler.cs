namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class UpdatePatientRecordHandler : ICommandHandler<UpdatePatientRecordCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public UpdatePatientRecordHandler(
        TimeProvider timeProvider,
        IPatientRecordRepo patientRecordRepo)
    {
        _timeProvider = timeProvider;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<Result> Handle(UpdatePatientRecordCommand command, CancellationToken cancellationToken)
    {
        var patientRecord = await _patientRecordRepo.Find(command.Id, cancellationToken);
        if (patientRecord is null)
        {
            return Result.Fail("Der/die Patient(in) wurde nicht gefunden.");
        }

        var patientRecordUpdate = patientRecord.Update(
            command.ModifiedById,
            _timeProvider.GetUtcNow(),
            command.PatientRecord.PhoneNumber,
            new(command.PatientRecord.ReferralReason, command.PatientRecord.ReferralDoctor),
            command.PatientRecord.TherapyDays.MapToDomain(),
            command.PatientRecord.Tags.MapToDomain(),
            command.PatientRecord.Remark);

        await _patientRecordRepo.Upsert(patientRecordUpdate, cancellationToken);

        return Result.Ok();
    }
}
