using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Tgs.Promotion.AccountFirstDeposit.Repositories;

internal class RedisFirstLevelRepository(IDatabase redisDatabase) : IFirstLevelRepository
{
	private static readonly RedisKey _KeyPrefix = "Tgs:Promotion:AccountFirstDeposit:";

	public async ValueTask<long?> GetPlayerFirstDepositCountAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var dataKey = _KeyPrefix.Append(playerCode);

		var result = await redisDatabase.StringSetAndGetAsync(
			dataKey,
			1L,
			TimeSpan.FromDays(2D),
			when: When.NotExists).ConfigureAwait(false);

		return result.HasValue ? (long)result : null;
	}

	public async ValueTask IncrementFirstDepositAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var dataKey = _KeyPrefix.Append(playerCode);

		_ = await redisDatabase.StringIncrementAsync(
			dataKey).ConfigureAwait(false);
	}

	public async ValueTask DecrementFirstDepositAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var dataKey = _KeyPrefix.Append(playerCode);

		var result = await redisDatabase.StringDecrementAsync(
			  dataKey).ConfigureAwait(false);

		if (result < 0)
			await IncrementFirstDepositAsync(
				playerCode,
				cancellationToken).ConfigureAwait(false);
	}
}
