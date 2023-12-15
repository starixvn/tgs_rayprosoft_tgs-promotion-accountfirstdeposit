using StackExchange.Redis;
using Tgs.Promotion.AccountFirstDeposit.Repositories;

namespace AccountFirstDeposit.Repositories.UnitTests;

public class RedisFirstLevelRepositoryTests
{
    [Fact]
    public async Task RedisFirstLevelRepository_取得存款次數()
    {
        // Arrange
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new RedisFirstLevelRepository(fakeRedisDatabase);

        var playerCode = "test1";

        // Act
        await sut.GetPlayerFirstDepositCountAsync(playerCode);

        // Assert
        _ = fakeRedisDatabase.Received(1)
            .StringSetAndGetAsync(
                Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")),
                Arg.Is((RedisValue)1L),
                Arg.Is(TimeSpan.FromDays(2D)),
                when: Arg.Is(When.NotExists));
    }

    [Fact]
    public async Task RedisFirstLevelRepository_遞增存款次數()
    {
        // Arrange
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new RedisFirstLevelRepository(fakeRedisDatabase);

        var playerCode = "test1";

        // Act
        await sut.IncrementFirstDepositAsync(playerCode);

        // Assert
        _ = fakeRedisDatabase.Received(1)
            .StringIncrementAsync(Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")));
    }

    [Fact]
    public async Task RedisFirstLevelRepository_遞減存款次數()
    {
        // Arrange
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new RedisFirstLevelRepository(fakeRedisDatabase);

        var playerCode = "test1";

        // Act
        await sut.DecrementFirstDepositAsync(playerCode);

        // Assert
        _ = fakeRedisDatabase.Received(1)
            .StringDecrementAsync(Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")));
        _ = fakeRedisDatabase.Received(0)
            .StringIncrementAsync(Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")));
    }

    [Fact]
    public async Task RedisFirstLevelRepository_遞減存款次數_如果回傳負數會恢復遞減動作()
    {
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new RedisFirstLevelRepository(fakeRedisDatabase);

        var playerCode = "test1";

        _ = fakeRedisDatabase.StringDecrementAsync(Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")))
            .Returns(-1);

        // Act
        await sut.DecrementFirstDepositAsync(playerCode);

        // Assert
        _ = fakeRedisDatabase.Received(1)
            .StringIncrementAsync(Arg.Is(new RedisKey($"Tgs:Promotion:AccountFirstDeposit:{playerCode}")));
    }
}
