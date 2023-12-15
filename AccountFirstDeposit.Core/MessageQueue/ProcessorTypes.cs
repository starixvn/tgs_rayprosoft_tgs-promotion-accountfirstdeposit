namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

public static class ProcessorTypes
{
	public static readonly Type AccountFirstDepositSettingProcessorType = typeof(AccountFirstDepositSettingProcessor);
	public static readonly Type CheckPromotionProcessorType = typeof(CheckPromotionProcessor);
	public static readonly Type TagPromotionHandlerType = typeof(TagPromotionHandler);
	public static readonly Type CommitPromotionHandlerType = typeof(CommitPromotionHandler);
	public static readonly Type RollbackPromotionHandlerType = typeof(RollbackPromotionHandler);
	public static readonly Type DepositFormStatusChangedHandlerType = typeof(DepositFormStatusChangedHandler);
}
