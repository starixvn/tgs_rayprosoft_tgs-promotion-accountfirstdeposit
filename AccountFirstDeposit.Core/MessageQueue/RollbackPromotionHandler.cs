using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class RollbackPromotionHandler(IAccountFirstDepositCheckService accountFirstDepositCheckService) : IMessageHandler
{
	public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = CheckPromotionRequest.Parser.ParseFrom(data.Span);

		return accountFirstDepositCheckService.RollbackTagAsync(request.PlayerCode, cancellationToken);
	}
}
