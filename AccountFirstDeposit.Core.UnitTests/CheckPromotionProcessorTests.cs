using Google.Protobuf;
using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.MessageQueue;
using Tgs.Promotion.Clients.Protos;

namespace AccountFirstDeposit.Core.UnitTests;

public class CheckPromotionProcessorTests
{
    [Fact]
    public async Task CheckPromotionProcessor_處理檢查玩家是否儲值過事件()
    {
        // Arrange
        var fakeAccountFirstDepositCheckService = Substitute.For<IAccountFirstDepositCheckService>();

        var sut = new CheckPromotionProcessor(fakeAccountFirstDepositCheckService);

        var request = new CheckPromotionRequest
        {
            PlayerCode = "test1",
            PromotionId = Guid.NewGuid()
        };

        _ = fakeAccountFirstDepositCheckService.CheckAsync(Arg.Is(request.PlayerCode))
            .Returns(true);

        // Act
        var response = await sut.HandleAsync(request.ToByteArray());
        var actual = CheckPromotionResponse.Parser.ParseFrom(response.Span);

        // Assert
        Assert.True(actual.Result);
    }
}
