using System.Reflection;
using Tgs.Promotion.AccountFirstDeposit;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAccountFirstDeposit(this IServiceCollection services)
		=> services
			.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
			.AddTransient<IAccountFirstDepositCheckService, AccountFirstDepositCheckService>();
}
