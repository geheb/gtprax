namespace GtPrax.Domain.ValueObjects;

using System.Collections.Generic;

public sealed class TagType : ValueObject
{
    public static readonly TagType Priority = new(1, nameof(Priority));
    public static readonly TagType Jumper = new(2, nameof(Jumper));
    public static readonly TagType Neurofeedback = new(3, nameof(Neurofeedback));
    public static readonly TagType School = new(4, nameof(School));

    public int Key { get; private set; }
    public string Value { get; private set; }

    public static TagType From(int key) =>
        key switch
        {
            1 => Priority,
            2 => Jumper,
            3 => Neurofeedback,
            4 => School,
            _ => throw new NotImplementedException()
        };

    private TagType(int key, string value)
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
