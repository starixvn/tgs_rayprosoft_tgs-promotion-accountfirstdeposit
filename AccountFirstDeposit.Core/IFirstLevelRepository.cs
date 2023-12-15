namespace Tgs.Promotion.AccountFirstDeposit;

public interface IFirstLevelRepository
{
	ValueTask<long?> GetPlayerFirstDepositCountAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask IncrementFirstDepositAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask DecrementFirstDepositAsync(string playerCode, CancellationToken cancellationToken = default);
}
