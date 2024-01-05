using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.MessageQueue;
using Tgs.Promotion.Clients.Protos;

namespace AccountFirstDeposit.Core.UnitTests;
public class RollbackPromotionHandlerTests
{
    [Fact]
    public async Task RollbackPromotionHandler_處理回滾標記事件()
    {
        // Arrange
        var fakeAccountFirstDepositCheckService = Substitute.For<IAccountFirstDepositCheckService>();

        var sut = new RollbackPromotionHandler(fakeAccountFirstDepositCheckService);

        var request = new CheckPromotionRequest
        {
            PlayerCode = "test1",
            PromotionId = Guid.NewGuid()
        };

        // Act
        await sut.HandleAsync("test", request, null);

        // Assert
        _ = fakeAccountFirstDepositCheckService.Received(1)
            .RollbackTagAsync(Arg.Is(request.PlayerCode));
    }
}
