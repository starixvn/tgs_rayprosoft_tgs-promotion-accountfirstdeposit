using Microsoft.Extensions.Logging.Abstractions;
using Tgs.Payment.Clients;
using Tgs.Promotion.AccountFirstDeposit;
using Xunit;

namespace AccountFirstDeposit.Core.UnitTests;

public class AccountFirstDepositCheckServiceTests
{
    [Fact]
    public async Task AccountFirstDepositCheckService_檢查玩家是否已經首儲過()
    {
        // Arrange
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeFirstLevelRepository = Substitute.For<IFirstLevelRepository>();

        var sut = new AccountFirstDepositCheckService(
            fakePaymentServiceClient,
            fakeFirstLevelRepository,
            NullLogger<AccountFirstDepositCheckService>.Instance);

        var playerCode = "test1";

        _ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(Arg.Is(playerCode))
            .Returns(new PaymentOverview(
                null,
                null,
                null,
                null,
                null,
                null,
                DateOnly.FromDateTime(DateTime.Now),
                0,
                0));

        // Act
        var actual = await sut.CheckAsync(playerCode);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task AccountFirstDepositCheckService_檢查玩家是否已經首儲過_如果快取沒有資料_會以PaymentOverview的FirstDepositOn是否有值做判斷()
    {
        // Arrange
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeFirstLevelRepository = Substitute.For<IFirstLevelRepository>();

        var sut = new AccountFirstDepositCheckService(
            fakePaymentServiceClient,
            fakeFirstLevelRepository,
            NullLogger<AccountFirstDepositCheckService>.Instance);

        var playerCode = "test1";

        _ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(Arg.Is(playerCode))
            .Returns(new PaymentOverview(
                null,
                null,
                DateTime.Now,
                null,
                null,
                null,
                DateOnly.FromDateTime(DateTime.Now),
                0,
                0));

        // Act
        var actual = await sut.CheckAsync(playerCode);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task AccountFirstDepositCheckService_檢查玩家是否已經首儲過_當快取取得次數會以其值做判斷()
    {
        // Arrange
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeFirstLevelRepository = Substitute.For<IFirstLevelRepository>();

        var sut = new AccountFirstDepositCheckService(
            fakePaymentServiceClient,
            fakeFirstLevelRepository,
            NullLogger<AccountFirstDepositCheckService>.Instance);

        var playerCode = "test1";

        _ = fakeFirstLevelRepository.GetPlayerFirstDepositCountAsync(Arg.Is(playerCode))
            .Returns(1L);

        // Act
        var actual = await sut.CheckAsync(playerCode);

        // Assert
        Assert.False(actual);
        _ = fakePaymentServiceClient.DidNotReceive()
            .GetPlayerPaymentOverviewAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AccountFirstDepositCheckService_標記玩家儲值()
    {
        // Arrange
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeFirstLevelRepository = Substitute.For<IFirstLevelRepository>();

        var sut = new AccountFirstDepositCheckService(
            fakePaymentServiceClient,
            fakeFirstLevelRepository,
            NullLogger<AccountFirstDepositCheckService>.Instance);

        var playerCode = "test1";

        // Act
        await sut.TagDepositAsync(playerCode);

        // Assert
        _ = fakeFirstLevelRepository.Received(1)
            .IncrementFirstDepositAsync(Arg.Is(playerCode));
    }

    [Fact]
    public async Task AccountFirstDepositCheckService_回滾標記玩家儲值()
    {
        // Arrange
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeFirstLevelRepository = Substitute.For<IFirstLevelRepository>();

        var sut = new AccountFirstDepositCheckService(
            fakePaymentServiceClient,
            fakeFirstLevelRepository,
            NullLogger<AccountFirstDepositCheckService>.Instance);

        var playerCode = "test1";

        // Act
        await sut.RollbackTagAsync(playerCode);

        // Assert
        _ = fakeFirstLevelRepository.Received(1)
            .DecrementFirstDepositAsync(Arg.Is(playerCode));
    }
}
