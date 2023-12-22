using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class RollbackPromotionHandler(IAccountFirstDepositCheckService accountFirstDepositCheckService)
	: IMessageHandler<CheckPromotionRequest>
{
	public ValueTask HandleAsync(CheckPromotionRequest data, CancellationToken cancellationToken = default)
		=> accountFirstDepositCheckService.RollbackTagAsync(data.PlayerCode, cancellationToken);
}
