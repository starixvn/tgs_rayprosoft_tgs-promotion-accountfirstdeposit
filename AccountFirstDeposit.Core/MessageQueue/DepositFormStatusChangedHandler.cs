using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Tgs.Promotion.AccountFirstDeposit.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.Promotion.AccountFirstDeposit.MessageQueue;

internal class DepositFormStatusChangedHandler(
	IPaymentServiceClient paymentServiceClient,
	ICommandService<DistributeDepositBonusCommand> distributeService) : IMessageHandler
{
	public async ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
	{
		var request = FormEventEx.Parser.ParseFrom(data.Span);
		var previousStatus = Enum.Parse<PaymentFlowState>(request.Status);

		var depositForm = await paymentServiceClient.GetDepositFormAsync(
			request.FormId,
			cancellationToken).ConfigureAwait(false);

		if (depositForm is null || depositForm.PromotionId is null)
			return;

		if (previousStatus != depositForm.Status
			&& depositForm.Status == PaymentFlowState.Approved)
		{
			var command = new DistributeDepositBonusCommand(depositForm);

			await distributeService.ExecuteAsync(
				command,
				cancellationToken).ConfigureAwait(false);
		}
	}
}
