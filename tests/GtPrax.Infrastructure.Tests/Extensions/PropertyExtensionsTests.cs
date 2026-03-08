namespace GtPrax.Infrastructure.Tests.Extensions;

using GtPrax.Infrastructure.Extensions;
using Xunit;

public sealed class PropertyExtensionsTests
{
    private sealed class TestObject
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public void SetValue_DifferentValue_ShouldUpdateAndReturnTrue()
    {
        var obj = new TestObject { Name = "old" };
        var result = obj.SetValue(o => o.Name, "new");
        Assert.True(result);
        Assert.Equal("new", obj.Name);
    }

    [Fact]
    public void SetValue_SameValue_ShouldReturnFalse()
    {
        var obj = new TestObject { Name = "same" };
        var result = obj.SetValue(o => o.Name, "same");
        Assert.False(result);
    }

    [Fact]
    public void SetValue_SameReference_ShouldReturnFalse()
    {
        var value = "test";
        var obj = new TestObject { Name = value };
        var result = obj.SetValue(o => o.Name, value);
        Assert.False(result);
    }

    [Fact]
    public void SetValue_NullToValue_ShouldUpdateAndReturnTrue()
    {
        var obj = new TestObject { Name = null };
        var result = obj.SetValue(o => o.Name, "new");
        Assert.True(result);
        Assert.Equal("new", obj.Name);
    }

    [Fact]
    public void SetValue_ValueToNull_ShouldUpdateAndReturnTrue()
    {
        var obj = new TestObject { Name = "old" };
        var result = obj.SetValue(o => o.Name, null);
        Assert.True(result);
        Assert.Null(obj.Name);
    }

    [Fact]
    public void SetValue_BothNull_ShouldReturnFalse()
    {
        var obj = new TestObject { Name = null };
        var result = obj.SetValue(o => o.Name, null);
        Assert.False(result);
    }

    [Fact]
    public void SetValue_IntDifferentValue_ShouldUpdateAndReturnTrue()
    {
        var obj = new TestObject { Value = 1 };
        var result = obj.SetValue(o => o.Value, 2);
        Assert.True(result);
        Assert.Equal(2, obj.Value);
    }

    [Fact]
    public void SetValue_IntSameValue_ShouldReturnFalse()
    {
        var obj = new TestObject { Value = 5 };
        var result = obj.SetValue(o => o.Value, 5);
        Assert.False(result);
    }
}
