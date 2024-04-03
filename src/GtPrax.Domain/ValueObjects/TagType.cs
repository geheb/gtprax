namespace GtPrax.Domain.ValueObjects;

using System.Collections.Generic;

internal sealed class TagType : ValueObject
{
    public static readonly TagType Priority = new(0, nameof(Priority));
    public static readonly TagType Jumper = new(1, nameof(Jumper));
    public static readonly TagType Neurofeedback = new(2, nameof(Neurofeedback));

    public int Key { get; private set; }
    public string Value { get; private set; }

    public static TagType From(int key) =>
        key switch
        {
            0 => Priority,
            1 => Jumper,
            2 => Neurofeedback,
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
}
