namespace Tgs.Promotion.AccountFirstDeposit;
public record DepositRewardSetting(
	decimal DepositMinimumLimit,
	decimal Percentage,
	decimal BonusAmountLimit,
	decimal TurnoverMultiple,
	IEnumerable<string>? BucketTypes);
