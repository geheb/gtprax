namespace GtPrax.Domain.ValueObjects;

public sealed class FilterType : ValueObject
{
    public static readonly FilterType ChildrenInTheMorning = new(1, nameof(ChildrenInTheMorning));
    public static readonly FilterType ChildrenInTheAfternoon = new(2, nameof(ChildrenInTheAfternoon));
    public static readonly FilterType AdultsInTheMorning = new(3, nameof(AdultsInTheMorning));
    public static readonly FilterType AdultsInTheAfternoon = new(4, nameof(AdultsInTheAfternoon));
    public static readonly FilterType TherapyDayWithHomeVisit = new(5, nameof(TherapyDayWithHomeVisit));
    public static readonly FilterType HasTagPriority = new(6, nameof(HasTagPriority));
    public static readonly FilterType HasTagJumper = new(7, nameof(HasTagJumper));
    public static readonly FilterType HasTagNeurofeedback = new(8, nameof(HasTagNeurofeedback));
    public static readonly FilterType HasTagSchool = new(9, nameof(HasTagSchool));

    public int Key { get; private set; }
    public string Value { get; private set; }

    public static FilterType From(int key) => key switch
    {
        1 => ChildrenInTheMorning,
        2 => ChildrenInTheAfternoon,
        3 => AdultsInTheMorning,
        4 => AdultsInTheAfternoon,
        5 => TherapyDayWithHomeVisit,
        6 => HasTagPriority,
        7 => HasTagJumper,
        8 => HasTagNeurofeedback,
        9 => HasTagSchool,
        _ => throw new NotImplementedException()
    };

    private FilterType(int key, string value)
    {
        Key = key;
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
    }

    public override string ToString() => Value;
}
