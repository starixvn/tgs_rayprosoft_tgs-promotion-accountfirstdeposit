using Tgs.Payment.Clients;
using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.ApplicationServices;
using Tgs.Promotion.Clients;

namespace AccountFirstDeposit.Core.UnitTests;

public class DistributeDepositBonusCommandServiceTests
{
    [Fact]
    public async Task DistributeDepositBonusCommandService_發放帳號首儲獎金()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var promotionId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 1000M,
            PlayerCode = "test1",
            PromotionId = promotionId,
            BrandId = brandId
        });
        var distributePlanId = Guid.NewGuid();

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(Arg.Is(promotionId))
            .Returns(new PromotionMainInfo
            {
                Id = promotionId,
                PromotionType = "AccountFirstDeposit",
                EventDuring = (DateTime.Now.AddDays(-1D), DateTime.Now.AddDays(1D))
            });

        var depositRewardSetting = new DepositRewardSetting(
            10M,
            1000M,
            10M,
            [1],
            []);
        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(Arg.Is(promotionId))
            .Returns(new PromotionRule(depositRewardSetting));

        _ = fakePromotionServiceClient.GetPromotionDistributePlanAsync(Arg.Is(promotionId), Arg.Is(0L))
            .Returns(new PromotionDistributePlanInfo(
                distributePlanId,
                brandId,
                promotionId,
                0L));

        _ = fakePromotionServiceClient.GetPromotionUsedAmountAsync(Arg.Is(promotionId))
            .Returns(100);

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.Received(1)
            .DistributePromotionBonusAsync(
                Arg.Is(distributePlanId),
                Arg.Is(command.DepositForm.PlayerCode),
                Arg.Is(100M),
                Arg.Is(10000M),
                Arg.Is((string?)null),
                Arg.Is(command.DepositForm.Id),
                Arg.Is(depositRewardSetting.BucketTypes));
        _ = fakePromotionServiceClient.Received(1)
            .UpdatePromotionUsedAmountAsync(
                Arg.Is(promotionId),
                Arg.Is(200M),
                Arg.Is((Guid?)null));
    }

    [Fact]
    public async Task DistributeDepositBonusCommandService_發放帳號首儲獎金不會超過最大發放金額()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var promotionId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 100000M,
            PlayerCode = "test1",
            PromotionId = promotionId,
            BrandId = brandId
        });
        var distributePlanId = Guid.NewGuid();

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(Arg.Is(promotionId))
            .Returns(new PromotionMainInfo
            {
                Id = promotionId,
                PromotionType = "AccountFirstDeposit",
                EventDuring = (DateTime.Now.AddDays(-1D), DateTime.Now.AddDays(1D))
            });

        var depositRewardSetting = new DepositRewardSetting(
            10M,
            1000M,
            10M,
            [1],
            []);
        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(Arg.Is(promotionId))
            .Returns(new PromotionRule(depositRewardSetting));

        _ = fakePromotionServiceClient.GetPromotionDistributePlanAsync(Arg.Is(promotionId), Arg.Is(0L))
            .Returns(new PromotionDistributePlanInfo(
                distributePlanId,
                brandId,
                promotionId,
                0L));

        _ = fakePromotionServiceClient.GetPromotionUsedAmountAsync(Arg.Is(promotionId))
            .Returns(100);

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.Received(1)
            .DistributePromotionBonusAsync(
                Arg.Is(distributePlanId),
                Arg.Is(command.DepositForm.PlayerCode),
                Arg.Is(1000M),
                Arg.Is(910000M),
                Arg.Is((string?)null),
                Arg.Is(command.DepositForm.Id),
                Arg.Is(depositRewardSetting.BucketTypes));
        _ = fakePromotionServiceClient.Received(1)
            .UpdatePromotionUsedAmountAsync(
                Arg.Is(promotionId),
                Arg.Is(1100M),
                Arg.Is((Guid?)null));
    }

    [Fact]
    public async Task DistributeDepositBonusCommandService_如果活動發放總金額已經達到限制不會發放獎金()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var promotionId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 1000M,
            PlayerCode = "test1",
            PromotionId = promotionId,
            BrandId = brandId
        });
        var distributePlanId = Guid.NewGuid();

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(Arg.Is(promotionId))
            .Returns(new PromotionMainInfo
            {
                Id = promotionId,
                PromotionType = "AccountFirstDeposit",
                EventDuring = (DateTime.Now.AddDays(-1D), DateTime.Now.AddDays(1D)),
                PromotionLimitAmount = 10000M
            });

        var depositRewardSetting = new DepositRewardSetting(
            10M,
            1000M,
            10M,
            [1],
            []);
        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(Arg.Is(promotionId))
            .Returns(new PromotionRule(depositRewardSetting));

        _ = fakePromotionServiceClient.GetPromotionUsedAmountAsync(Arg.Is(promotionId))
            .Returns(10000M);

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.DidNotReceive()
            .DistributePromotionBonusAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<string[]?>());
        _ = fakePromotionServiceClient.DidNotReceive()
            .UpdatePromotionUsedAmountAsync(
                Arg.Any<Guid>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid?>());
    }

    [Fact]
    public async Task DistributeDepositBonusCommandService_沒有參加活動的存款不會有動作()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 1000M,
            PlayerCode = "test1",
            BrandId = brandId
        });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.DidNotReceive()
            .DistributePromotionBonusAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<string[]?>());
        _ = fakePromotionServiceClient.DidNotReceive()
            .UpdatePromotionUsedAmountAsync(
                Arg.Any<Guid>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid?>());
    }

    [Fact]
    public async Task DistributeDepositBonusCommandService_參加活動的活動類型需要是AccountFirstDeposit才會有動作()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var promotionId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 1000M,
            PlayerCode = "test1",
            PromotionId = promotionId,
            BrandId = brandId
        });
        var distributePlanId = Guid.NewGuid();

        _ = fakePromotionServiceClient.GetPromotionMainInfoAsync(Arg.Is(promotionId))
            .Returns(new PromotionMainInfo
            {
                Id = promotionId,
                PromotionType = "FirstDeposit",
                EventDuring = (DateTime.Now.AddDays(-1D), DateTime.Now.AddDays(1D))
            });

        var depositRewardSetting = new DepositRewardSetting(
            10M,
            1000M,
            10M,
            [1],
            []);
        _ = fakePromotionServiceClient.GetPromotionRuleAsync<PromotionRule>(Arg.Is(promotionId))
            .Returns(new PromotionRule(depositRewardSetting));

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.DidNotReceive()
            .DistributePromotionBonusAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<string[]?>());
        _ = fakePromotionServiceClient.DidNotReceive()
            .UpdatePromotionUsedAmountAsync(
                Arg.Any<Guid>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid?>());
    }

    [Fact]
    public async Task DistributeDepositBonusCommandService_如果找不到Promotion的資料就不會有動作()
    {
        // Arrange
        var fakePromotionServiceClient = Substitute.For<IPromotionServiceClient>();

        var sut = new DistributeDepositBonusCommandService(fakePromotionServiceClient);

        var promotionId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var command = new DistributeDepositBonusCommand(new DepositFormInfo
        {
            Id = Guid.NewGuid(),
            Amount = 1000M,
            PlayerCode = "test1",
            PromotionId = promotionId,
            BrandId = brandId
        });

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakePromotionServiceClient.DidNotReceive()
            .DistributePromotionBonusAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<string?>(),
                Arg.Any<Guid?>(),
                Arg.Any<string[]?>());
        _ = fakePromotionServiceClient.DidNotReceive()
            .UpdatePromotionUsedAmountAsync(
                Arg.Any<Guid>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid?>());
    }
}
