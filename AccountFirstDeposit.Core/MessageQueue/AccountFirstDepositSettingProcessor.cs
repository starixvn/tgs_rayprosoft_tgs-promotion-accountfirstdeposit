using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.Promotion.Clients.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

class AccountFirstDepositSettingProcessor(ILogger<AccountFirstDepositSettingProcessor> logger)
	: IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
	public ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = CampaignDetailRequest.Parser.ParseFrom(data.Span);

		var settings = JsonSerializer.Deserialize<DepositRewardSetting>(
			request.Settings,
			PromotionUtility.SerializerOptions)
			?? throw new NullReferenceException("Deserialize DepositRewardSetting is null");

		logger.LogDebug("Account deposit setting : {settings}", settings);

		var response = new CampaignDetailResponse
		{
			CampaignDetail = JsonSerializer.Serialize(
				settings,
				PromotionUtility.SerializerOptions),
			BonusAmountLimit = settings.BonusAmountLimit,
			Percentage = settings.Percentage,
			TurnoverMultiple = settings.TurnoverMultiple
		};

		return ValueTask.FromResult((ReadOnlyMemory<byte>)response.ToByteArray());
	}
}
