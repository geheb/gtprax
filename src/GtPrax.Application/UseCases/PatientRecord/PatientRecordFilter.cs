namespace GtPrax.Application.UseCases.PatientRecord;

public enum PatientRecordFilter
{
    None = 0,
    ChildrenInTheMorning = 1,
    ChildrenInTheAfternoon = 2,
    AdultsInTheMorning = 3,
    AdultsInTheAfternoon = 4,
    TherapyDayWithHomeVisit = 5,
    HasTagPriority = 6,
    HasTagJumper = 7,
    HasTagNeurofeedback = 8
}
