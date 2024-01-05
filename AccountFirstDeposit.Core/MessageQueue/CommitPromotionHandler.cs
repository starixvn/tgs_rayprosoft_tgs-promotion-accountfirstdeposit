using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class CommitPromotionHandler : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
		=> ValueTask.CompletedTask;
}
