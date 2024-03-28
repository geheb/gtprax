namespace GtPrax.Domain.ValueObjects;

using System;
using System.Collections.Generic;

public sealed class UserRoleType : ValueObject
{
    private const string StaffName = "Staff";
    private const string AdminName = "Admin";
    private const string ManagerName = "Manager";

    public static readonly UserRoleType Staff = new(0, StaffName);
    public static readonly UserRoleType Admin = new(1, AdminName);
    public static readonly UserRoleType Manager = new(2, ManagerName);

    public static UserRoleType From(string value) =>
        value switch
        {
            StaffName => Staff,
            AdminName => Admin,
            ManagerName => Manager,
            _ => throw new NotImplementedException()
        };

    public static UserRoleType From(int key) =>
        key switch
        {
            0 => Staff,
            1 => Admin,
            2 => Manager,
            _ => throw new NotImplementedException()
        };

    public static UserRoleType[] From(IEnumerable<string> values) =>
        values.Select(From).ToArray();

    public static UserRoleType[] From(IEnumerable<int> values) =>
        values.Select(From).ToArray();

    public int Key { get; private set; }
    public string Value { get; private set; }

    private UserRoleType(int key, string value)
    {
        Key = key;
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
    }
}
