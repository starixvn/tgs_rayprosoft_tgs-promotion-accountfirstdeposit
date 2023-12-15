using System.Text.Json;

namespace Tgs.Promotion.AccountFirstDeposit.Models;

public static class PromotionUtility
{
	public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = false
	};
}
