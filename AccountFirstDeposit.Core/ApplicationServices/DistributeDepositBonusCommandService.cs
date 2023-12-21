using Tgs.Promotion.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Promotion.AccountFirstDeposit.ApplicationServices;

internal class DistributeDepositBonusCommandService(
	IPromotionServiceClient promotionServiceClient)
	: ICommandService<DistributeDepositBonusCommand>
{
	public async ValueTask ExecuteAsync(DistributeDepositBonusCommand command, CancellationToken cancellationToken = default)
	{
		if (command.DepositForm.PromotionId is null)
			return;

		var promotionId = command.DepositForm.PromotionId.Value;

		var promotionMainInfo = await promotionServiceClient.GetPromotionMainInfoAsync(
			command.DepositForm.PromotionId.Value,
			cancellationToken).ConfigureAwait(false);

		var rule = await promotionServiceClient.GetPromotionRuleAsync<PromotionRule>(
			promotionId,
			cancellationToken).ConfigureAwait(false);

		if (rule is null || promotionMainInfo is null || promotionMainInfo.PromotionType is not "AccountFirstDeposit")
			return;

		var (bonusAmount, bucketAmount) = CalculateAmount(
			rule.CampaignDetail,
			command.DepositForm.ActualAmount ?? command.DepositForm.Amount);

		if (promotionMainInfo.PromotionLimitAmount is not null)
		{
			var currentUsedPromotionAmount = await promotionServiceClient.GetPromotionUsedAmountAsync(
				promotionMainInfo.Id,
				cancellationToken).ConfigureAwait(false);

			if (currentUsedPromotionAmount >= promotionMainInfo.PromotionLimitAmount.Value)
				return;
		}

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

		var usedAmount = await promotionServiceClient.GetPromotionUsedAmountAsync(
			promotionMainInfo.Id,
			cancellationToken).ConfigureAwait(false)
			+ bonusAmount;

		await promotionServiceClient.UpdatePromotionUsedAmountAsync(
			promotionMainInfo.Id,
			usedAmount,
			null,
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
