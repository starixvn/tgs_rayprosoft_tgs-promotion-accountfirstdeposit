using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class TagPromotionHandler(IAccountFirstDepositCheckService accountFirstDepositCheckService)
	: IMessageHandler<CheckPromotionRequest>
{
	public ValueTask HandleAsync(CheckPromotionRequest data, CancellationToken cancellationToken = default)
		=> accountFirstDepositCheckService.TagDepositAsync(data.PlayerCode, cancellationToken);
}
