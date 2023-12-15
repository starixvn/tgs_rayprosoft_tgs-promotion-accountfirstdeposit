using StackExchange.Redis;
using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAccountFirstDepositRepositories(this IServiceCollection services, Func<IServiceProvider, IDatabase> resolveRedisDatabase)
		=> services
			.AddTransient<IFirstLevelRepository>(sp => ActivatorUtilities.CreateInstance<RedisFirstLevelRepository>(
				sp,
				resolveRedisDatabase(sp)));
}
