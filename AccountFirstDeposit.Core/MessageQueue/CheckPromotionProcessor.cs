using Google.Protobuf;
using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

class CheckPromotionProcessor(IAccountFirstDepositCheckService accountFirstDepositCheckService)
	: IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
	public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = CheckPromotionRequest.Parser.ParseFrom(data.Span);

		var result = await accountFirstDepositCheckService.CheckAsync(
			request.PlayerCode,
			cancellationToken).ConfigureAwait(false);

		var response = new CheckPromotionResponse
		{
			Result = result
		};

		return response.ToByteArray();
	}
}
