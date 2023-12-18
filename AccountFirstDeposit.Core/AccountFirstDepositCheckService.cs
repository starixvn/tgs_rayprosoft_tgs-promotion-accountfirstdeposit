using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;

namespace Tgs.Promotion.AccountFirstDeposit;

internal class AccountFirstDepositCheckService(
	IPaymentServiceClient paymentServiceClient,
	IFirstLevelRepository firstLevelRepository,
	ILogger<AccountFirstDepositCheckService> logger)
	: IAccountFirstDepositCheckService
{
	public async ValueTask<bool> CheckAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var depositCount = await firstLevelRepository.GetPlayerFirstDepositCountAsync(
			playerCode,
			cancellationToken).ConfigureAwait(false);

		if (depositCount is not null)
			return depositCount.Value <= 0L;

		var overview = await paymentServiceClient.GetPlayerPaymentOverviewAsync(
			playerCode,
			cancellationToken).ConfigureAwait(false);

		logger.LogInformation("PaymentOverview => {overview}", overview);

		if (overview.FirstDepositOn is null)
			await firstLevelRepository.DecrementFirstDepositAsync(
				playerCode,
				cancellationToken).ConfigureAwait(false);

		return overview.FirstDepositOn is null;
	}

	public ValueTask RollbackTagAsync(string playerCode, CancellationToken cancellationToken = default)
		=> firstLevelRepository.DecrementFirstDepositAsync(playerCode, cancellationToken);

	public ValueTask TagDepositAsync(string playerCode, CancellationToken cancellationToken = default)
		=> firstLevelRepository.IncrementFirstDepositAsync(playerCode, cancellationToken);
}
