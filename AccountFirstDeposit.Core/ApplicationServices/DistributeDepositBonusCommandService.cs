using Tgs.Promotion.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Promotion.AccountFirstDeposit.ApplicationServices;

internal class DistributeDepositBonusCommandService(IPromotionServiceClient promotionServiceClient) : ICommandService<DistributeDepositBonusCommand>
{
	public async ValueTask ExecuteAsync(DistributeDepositBonusCommand command, CancellationToken cancellationToken = default)
	{
		if (command.DepositForm.PromotionId is null)
			return;

		var promotionId = command.DepositForm.PromotionId.Value;

		var rule = await promotionServiceClient.GetPromotionRuleAsync<PromotionRule>(
			promotionId,
			cancellationToken).ConfigureAwait(false);

		if (rule is null)
			return;

		var (bonusAmount, bucketAmount) = CalculateAmount(
			rule.CampaignDetail,
			command.DepositForm.ActualAmount ?? command.DepositForm.Amount);

		var distPlan = await promotionServiceClient.GetPromotionDistributePlanAsync(
			promotionId,
			0L,
			cancellationToken).ConfigureAwait(false);

		await promotionServiceClient.DistributePromotionBonusAsync(
			distPlan.Id,
			command.DepositForm.PlayerCode,
			bonusAmount,
			bucketAmount,
			null,
			command.DepositForm.Id,
			rule.CampaignDetail.BucketTypes,
			cancellationToken).ConfigureAwait(false);
	}

	private static (decimal bonusAmount, decimal bucketAmount) CalculateAmount(DepositRewardSetting setting,
		decimal depositAmount)
	{
		var depositBonusAmount = depositAmount;

		var bonusAmount = setting.Percentage * depositAmount / 100M;

		bonusAmount = bonusAmount > setting.BonusAmountLimit
			? setting.BonusAmountLimit
			: bonusAmount;

		depositBonusAmount += bonusAmount;

		var bucketAmount = depositBonusAmount * setting.TurnoverMultiple - depositAmount;

		return (bonusAmount, bucketAmount);
	}
}
