namespace GtPrax.Infrastructure.Database.Entities;

using GtPrax.Application.Models;

internal sealed class Waitlist
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public ICollection<WaitlistPatient>? WaitlistPatients { get; set; }

    public WaitlistDto ToDto(int patientCount) =>
        new()
        {
            Id = Id,
            Name = Name,
            PatientCount = patientCount
        };
}
