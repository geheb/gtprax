namespace GtPrax.Infrastructure.Tests.Security;

using System.Net;
using GtPrax.Infrastructure.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Time.Testing;
using Xunit;

public sealed class BlacklistCacheTests : IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly BlacklistCache _sut;

    public BlacklistCacheTests()
    {
        _timeProvider.SetUtcNow(new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero));
        _sut = new BlacklistCache(_cache, _timeProvider);
    }

    public void Dispose() => _cache.Dispose();

    [Fact]
    public void Item_IsSuspicious_WhenCountHighAndAvgLow_ShouldReturnTrue()
    {
        var item = new BlacklistCache.Item("key", 7, DateTimeOffset.UtcNow, 0.5);
        Assert.True(item.IsSuspicious);
    }

    [Fact]
    public void Item_IsSuspicious_WhenCountLow_ShouldReturnFalse()
    {
        var item = new BlacklistCache.Item("key", 6, DateTimeOffset.UtcNow, 0.3);
        Assert.False(item.IsSuspicious);
    }

    [Fact]
    public void Item_IsSuspicious_WhenAvgHigh_ShouldReturnFalse()
    {
        var item = new BlacklistCache.Item("key", 10, DateTimeOffset.UtcNow, 0.6);
        Assert.False(item.IsSuspicious);
    }

    [Fact]
    public void Get_UnknownAddress_ShouldReturnNull()
    {
        var result = _sut.Get(IPAddress.Loopback, "agent");
        Assert.Null(result);
    }

    [Fact]
    public void Update_FirstVisit_Unlisted_ShouldCreateWithCount1()
    {
        var ip = IPAddress.Parse("192.168.1.1");
        _sut.Update(ip, "agent", isListed: false);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(1u, item.Value.Count);
        Assert.Equal(0.0, item.Value.AvgSeconds);
    }

    [Fact]
    public void Update_FirstVisit_Listed_ShouldCreateWithCount4()
    {
        var ip = IPAddress.Parse("192.168.1.2");
        _sut.Update(ip, "agent", isListed: true);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(4u, item.Value.Count);
    }

    [Fact]
    public void Update_RepeatVisit_ShouldIncrementCountAndCalculateAvg()
    {
        var ip = IPAddress.Parse("192.168.1.3");
        _sut.Update(ip, "agent", isListed: false);

        _timeProvider.Advance(TimeSpan.FromSeconds(1));
        _sut.Update(ip, "agent", isListed: false);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(2u, item.Value.Count);
        Assert.Equal(1.0, item.Value.AvgSeconds);
    }

    [Fact]
    public void Update_GapOver10Seconds_ShouldResetAvg()
    {
        var ip = IPAddress.Parse("192.168.1.4");
        _sut.Update(ip, "agent", isListed: false);

        _timeProvider.Advance(TimeSpan.FromSeconds(11));
        _sut.Update(ip, "agent", isListed: false);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(2u, item.Value.Count);
        Assert.Equal(0.0, item.Value.AvgSeconds);
    }

    [Fact]
    public void Update_RepeatVisit_Listed_ShouldIncrementBy4()
    {
        var ip = IPAddress.Parse("192.168.1.5");
        _sut.Update(ip, "agent", isListed: false);

        _timeProvider.Advance(TimeSpan.FromSeconds(1));
        _sut.Update(ip, "agent", isListed: true);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(5u, item.Value.Count);
    }

    [Fact]
    public void Get_AfterUpdate_ShouldReturnItem()
    {
        var ip = IPAddress.Parse("192.168.1.6");
        _sut.Update(ip, "agent", isListed: false);

        var item = _sut.Get(ip, "agent");
        Assert.NotNull(item);
        Assert.Equal(1u, item.Value.Count);
    }
}
