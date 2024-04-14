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

        var personResult = new PersonBuilder()
            .SetName(command.PatientFile.Name)
            .SetBirthDate(command.PatientFile.BirthDate)
            .SetPhoneNumber(command.PatientFile.PhoneNumber)
            .Build(personIdentities, _timeProvider.GetUtcNow());

        if (personResult.IsFailed)
        {
            return personResult.ToResult();
        }

        var patientFileBuilder = new PatientFileBuilder()
            .SetWaitingListId(command.WaitingListId)
            .SetCreated(command.CreatedBy, _timeProvider.GetUtcNow())
            .SetPerson(personResult.Value)
            .SetReferral(command.PatientFile.ReferralReason, command.PatientFile.ReferralDoctor)
            .SetRemark(command.PatientFile.Remark);

        Array.ForEach(command.PatientFile.TherapyDays, t => patientFileBuilder.SetTherapyDay(t.Day, t.IsMorning, t.IsAfternoon, t.IsHomeVisit, t.AvailableFrom));
        Array.ForEach(command.PatientFile.Tags, t => patientFileBuilder.SetTag(TagType.From((int)t)));

        var waitingListIdentities = await _waitingListStore.GetIdentities(cancellationToken);

        var patientFileResult = patientFileBuilder.Build(waitingListIdentities);
        if (patientFileResult.IsFailed)
        {
            return patientFileResult.ToResult();
        }

        await _patientFileStore.Upsert(patientFileResult.Value, cancellationToken);

        return Result.Ok();
    }
}
