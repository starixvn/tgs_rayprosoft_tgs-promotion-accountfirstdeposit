namespace Tgs.Promotion.AccountFirstDeposit;

public interface ITgsRepository
{
	ValueTask<Guid?> GetPromotionCalculateIdAsync(Guid promotionId, CancellationToken cancellationToken = default);

	ValueTask SetPromotionCalculateIdAsync(Guid promotionId, Guid calculateId, CancellationToken cancellationToken = default);
}
