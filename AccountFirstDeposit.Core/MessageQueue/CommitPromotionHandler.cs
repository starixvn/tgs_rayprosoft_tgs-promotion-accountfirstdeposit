using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class CommitPromotionHandler : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
		=> ValueTask.CompletedTask;
}
