namespace GtPrax.Application.Models;

public sealed class WaitlistDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int PatientCount { get; set; }
}
