using NATS.Client.Core;

namespace Tgs.Promotion.AccountFirstDeposit.NatsSerializers;

public class AccountFirstDepositSerizerRegistration : INatsSerializerRegistry
{
	public static readonly AccountFirstDepositSerizerRegistration Default = new();

	public INatsSerialize<T> GetSerializer<T>() => AccountFirstDepositSerializer<T>.Default;

	public INatsDeserialize<T> GetDeserializer<T>() => AccountFirstDepositSerializer<T>.Default;
}
