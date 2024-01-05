using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.Promotion.AccountFirstDeposit;
using Tgs.Promotion.AccountFirstDeposit.Repositories;
using Tgs.Promotion.AccountFirstDeposit.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAccountFirstDepositRepositories(
		this IServiceCollection services,
		Action<DbContextOptionsBuilder> dbConfigure,
		Func<IServiceProvider, IDatabase> resolveRedisDatabase)
		=> services
			.AddSingleton<IFirstLevelRepository>(sp => ActivatorUtilities.CreateInstance<RedisFirstLevelRepository>(
				sp,
				resolveRedisDatabase(sp)))
			.AddDbContext<TgsContext>(dbConfigure)
			.AddScoped<ITgsRepository, TgsRepository>();
}
