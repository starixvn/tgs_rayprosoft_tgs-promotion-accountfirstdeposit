namespace Tgs.Promotion.AccountFirstDeposit;
public record DepositRewardSetting(
	decimal Percentage,
	decimal BonusAmountLimit,
	decimal TurnoverMultiple,
	int[] VipLevels,
	string[]? BucketTypes);
