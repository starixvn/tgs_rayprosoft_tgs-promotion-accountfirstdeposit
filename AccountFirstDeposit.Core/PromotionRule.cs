using System.Text.Json.Serialization;

namespace Tgs.Promotion.AccountFirstDeposit;
public record PromotionRule(
	[property: JsonPropertyName("compaignDetail")] DepositRewardSetting CampaignDetail);
