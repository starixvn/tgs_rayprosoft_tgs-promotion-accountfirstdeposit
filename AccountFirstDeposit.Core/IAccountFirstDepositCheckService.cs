namespace Tgs.Promotion.AccountFirstDeposit;

internal interface IAccountFirstDepositCheckService
{
	ValueTask<bool> CheckAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask RollbackTagAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask TagDepositAsync(string playerCode, CancellationToken cancellationToken = default);
}
