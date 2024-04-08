namespace GtPrax.Application.UseCases.PatientFiles;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.UseCases.WaitingLists;
using GtPrax.Domain.Entities;
using GtPrax.Domain.ValueObjects;
using Mediator;

public sealed class CreatePatientFileHandler : ICommandHandler<CreatePatientFileCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IWaitingListStore _waitingListStore;
    private readonly IPatientFileStore _patientFileStore;

    public CreatePatientFileHandler(
        TimeProvider timeProvider,
        IWaitingListStore waitingListStore,
        IPatientFileStore patientFileStore)
    {
        _timeProvider = timeProvider;
        _waitingListStore = waitingListStore;
        _patientFileStore = patientFileStore;
    }

    public async ValueTask<Result> Handle(CreatePatientFileCommand command, CancellationToken cancellationToken)
    {
        var personIdentities = await _patientFileStore.GetIdentities(cancellationToken);
        var dto = command.PatientFile;

        var personResult = Person.Create(dto.Name, dto.BirthDate, dto.PhoneNumber, _timeProvider.GetUtcNow(), personIdentities);
        if (personResult.IsFailed)
        {
            return personResult.ToResult();
        }

        var waitingListIdentities = await _waitingListStore.GetIdentities(cancellationToken);
        var patientFileResult = PatientFile.Create(command.WaitingListId, _timeProvider.GetUtcNow(), dto.CreatedBy, personResult.Value, waitingListIdentities);
        if (patientFileResult.IsFailed)
        {
            return patientFileResult.ToResult();
        }

        var patientFile = patientFileResult.Value;

        patientFile.SetReferral(dto.ReferralReason, dto.ReferralDoctor);

        foreach (var day in dto.TherapyDays)
        {
            patientFile.SetTherapyDay(day.Day, day.IsMorning, day.IsAfternoon, day.IsHomeVisit, day.AvailableFrom);
        }

        foreach (var tag in dto.Tags)
        {
            patientFile.SetTag(TagType.From((int)tag));
        }

        patientFile.SetRemark(dto.Remark);

        await _patientFileStore.Create(patientFile, cancellationToken);

        return Result.Ok();
    }
}
