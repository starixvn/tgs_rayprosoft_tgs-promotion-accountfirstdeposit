using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

class CheckPromotionProcessor(IAccountFirstDepositCheckService accountFirstDepositCheckService)
	: IMessageProcessor<CheckPromotionRequest, CheckPromotionResponse>
{
	public async ValueTask<CheckPromotionResponse> HandleAsync(CheckPromotionRequest data, CancellationToken cancellationToken = default)
	{
		var result = await accountFirstDepositCheckService.CheckAsync(
			data.PlayerCode,
			cancellationToken).ConfigureAwait(false);

		return new CheckPromotionResponse
		{
			Result = result
		};
	}
}
