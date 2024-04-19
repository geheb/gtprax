namespace GtPrax.Application.UseCases.PatientFiles;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Repositories;
using Mediator;

public sealed class CreatePatientRecordHandler : ICommandHandler<CreatePatientRecordCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public CreatePatientRecordHandler(
        TimeProvider timeProvider,
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _timeProvider = timeProvider;
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<Result> Handle(CreatePatientRecordCommand command, CancellationToken cancellationToken)
    {
        var waitingList = await _waitingListRepo.Find(new(command.WaitingListId), cancellationToken);
        if (waitingList is null)
        {
            return Result.Fail(Messages.WaitingListNotFound);
        }

        var patientRecords = await _patientRecordRepo.GetAll(cancellationToken);

        var result = waitingList
            .AddPatientRecords(patientRecords)
            .AddPatientRecord(
                command.CreatedBy,
                _timeProvider.GetUtcNow(),
                new(command.PatientRecord.Name, command.PatientRecord.BirthDate, command.PatientRecord.PhoneNumber),
                new(command.PatientRecord.ReferralReason, command.PatientRecord.ReferralDoctor),
                command.PatientRecord.TherapyDays.MapToDomain(),
                command.PatientRecord.Tags.MapToDomain(),
                command.PatientRecord.Remark);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        await _patientRecordRepo.Upsert(result.Value, cancellationToken);

        return Result.Ok();
    }
}
