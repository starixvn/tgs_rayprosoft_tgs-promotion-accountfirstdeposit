using Google.Protobuf;
using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.MessageQueue;
using Tgs.Promotion.Clients.Protos;

namespace AccountFirstDeposit.Core.UnitTests;

public class TagPromotionHandlerTests
{
    [Fact]
    public async Task TagPromotionHandler_處理標記玩家儲值事件()
    {
        // Arrange
        var fakeAccountFirstDepositCheckService = Substitute.For<IAccountFirstDepositCheckService>();

        var sut = new TagPromotionHandler(fakeAccountFirstDepositCheckService);

        var request = new CheckPromotionRequest
        {
            PlayerCode = "test1",
            PromotionId = Guid.NewGuid()
        };

        // Act
        await sut.HandleAsync(request.ToByteArray());

        // Assert
        _ = fakeAccountFirstDepositCheckService.Received(1)
            .TagDepositAsync(Arg.Is(request.PlayerCode));
    }
}
