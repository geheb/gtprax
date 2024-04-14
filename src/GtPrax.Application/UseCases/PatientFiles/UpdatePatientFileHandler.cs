namespace GtPrax.Application.UseCases.PatientFiles;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Entities;
using GtPrax.Domain.ValueObjects;
using Mediator;

internal sealed class UpdatePatientFileHandler : ICommandHandler<UpdatePatientFileCommand, Result>
{
    private readonly TimeProvider _timeProvider;
    private readonly IPatientFileStore _patientFileStore;

    public UpdatePatientFileHandler(
        TimeProvider timeProvider,
        IPatientFileStore patientFileStore)
    {
        _timeProvider = timeProvider;
        _patientFileStore = patientFileStore;
    }

    public async ValueTask<Result> Handle(UpdatePatientFileCommand command, CancellationToken cancellationToken)
    {
        var patientFile = await _patientFileStore.Find(command.Id, cancellationToken);
        if (patientFile is null)
        {
            return Result.Fail(Messages.PatientNotFound);
        }

        var personResult = new PersonBuilder(patientFile.Person)
            .SetPhoneNumber(command.PatientFile.PhoneNumber)
            .Build([], _timeProvider.GetUtcNow());

        if (personResult.IsFailed)
        {
            return personResult.ToResult();
        }

        var patientFileBuilder = new PatientFileBuilder(patientFile)
            .SetLastModified(command.ModifiedBy, _timeProvider.GetUtcNow())
            .SetPerson(personResult.Value)
            .SetReferral(command.PatientFile.ReferralReason, command.PatientFile.ReferralDoctor)
            .SetRemark(command.PatientFile.Remark);

        Array.ForEach(command.PatientFile.TherapyDays, t => patientFileBuilder.SetTherapyDay(t.Day, t.IsMorning, t.IsAfternoon, t.IsHomeVisit, t.AvailableFrom));
        Array.ForEach(command.PatientFile.Tags, t => patientFileBuilder.SetTag(TagType.From((int)t)));

        var patientResult = patientFileBuilder.Build([]);
        if (patientResult.IsFailed)
        {
            return patientResult.ToResult();
        }

        await _patientFileStore.Upsert(patientResult.Value, cancellationToken);
        return Result.Ok();
    }
}
