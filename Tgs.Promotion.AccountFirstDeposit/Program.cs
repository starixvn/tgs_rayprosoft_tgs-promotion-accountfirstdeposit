using FluentdForward.OpenTelemetry.MessagePack;
using NATS.Client.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using Tgs.Promotion;
using Tgs.Promotion.AccountFirstDeposit.MessageQueue;
using Tgs.Promotion.AccountFirstDeposit.NatsSerializers;

const string AccountFirstDepositPromotionType = "AccountFirstDeposit";

var builder = WebApplication.CreateBuilder(args);
{
	_ = builder.Logging
		.AddOpenTelemetry(options =>
		{
			options.IncludeFormattedMessage = true;
			_ = options.AddFluentdForwardExporter(fluentOptions =>
			{
				var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
				fluentOptions.Host = nodeIp;
				fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
				fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
					"MachineName",
					() => Environment.MachineName));
			});
		});

	_ = builder.Services
		.AddKeyedSingleton<IConnectionMultiplexer>(
			"Redis",
			(sp, _) => ConnectionMultiplexer.Connect(
				builder.Configuration.GetConnectionString("Redis")!))
		.AddKeyedSingleton(
			"Redis",
			(sp, _) => sp.GetRequiredKeyedService<IConnectionMultiplexer>("Redis").GetDatabase())
		.AddAccountFirstDeposit()
		.AddAccountFirstDepositRepositories(sp => sp.GetRequiredKeyedService<IDatabase>("Redis"))
		.AddPromotionClientServices((opts, _) => opts.Url =
			builder.Configuration.GetValue<Uri>("AccountFirstDeposit:PromotionServiceBaseUrl")!)
		.AddPaymentServiceClient(httpBuilder => httpBuilder
			.ConfigureHttpClient(httpClient => httpClient.BaseAddress =
				builder.Configuration.GetValue<Uri>("AccountFirstDeposit:PaymentServiceBaseUrl")!));

	_ = builder.Services
		.AddKeyedSingleton(
			"Nats",
			(sp, _) => new NatsConnection(NatsOpts.Default with
			{
				Url = builder.Configuration.GetConnectionString("Nats")!,
				SerializerRegistry = AccountFirstDepositSerizerRegistration.Default
			}))
		.AddMessageQueue()
		.AddNatsMessageQueue(natsConfiguration => natsConfiguration
			.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
			.AddProcessor(
				ProcessorTypes.AccountFirstDepositSettingProcessorType,
				$"tgs.promotion.{AccountFirstDepositPromotionType.ToLowerInvariant()}.setting",
				"tgs.promotion.account-first-deposit.setting")
			.AddProcessor(
				ProcessorTypes.CheckPromotionProcessorType,
				string.Format(PromotionCheckConsts.CheckSubjectTemplate, AccountFirstDepositPromotionType.ToLowerInvariant()),
				"tgs.promotion.account-first-deposit.check")
			.AddReplyHandler(
				ProcessorTypes.TagPromotionHandlerType,
				string.Format(PromotionCheckConsts.TagSubjectTemplate, AccountFirstDepositPromotionType.ToLowerInvariant()),
				"tgs.promotion.account-first-deposit.tag")
			.AddReplyHandler(
				ProcessorTypes.CommitPromotionHandlerType,
				string.Format(PromotionCheckConsts.CommitSubjectTemplate, AccountFirstDepositPromotionType.ToLowerInvariant()),
				"tgs.promotion.account-first-deposit.commit")
			.AddReplyHandler(
				ProcessorTypes.RollbackPromotionHandlerType,
				string.Format(PromotionCheckConsts.RollbackSubjectTemplate, AccountFirstDepositPromotionType.ToLowerInvariant()),
				"tgs.promotion.account-first-deposit.rollback")
			.AddHandler(
				ProcessorTypes.DepositFormStatusChangedHandlerType,
				"tgs.payment.deposit.status_ex",
				"tgs.promotion.account-first-deposit.deposit"))
		.AddNatsGlobPatternExchange("*");

	var instrumentationExtenionFilters = new[] { "html", "ico", "png", "jpg" };
	_ = builder.Services
		.AddOpenTelemetry()
		.WithTracing(
			tracingBuilder => tracingBuilder
				.SetResourceBuilder(ResourceBuilder.CreateDefault()
					.AddEnvironmentVariableDetector())
				.AddSource("Tgs.*")
				.AddSource("TGS.*")
				.AddSource("Valhalla.*")
				.AddAspNetCoreInstrumentation(options => options.Filter =
					httpContext => !httpContext.Request.Path.HasValue
						|| !httpContext.Request.Path.StartsWithSegments("/swagger")
						&& !httpContext.Request.Path.StartsWithSegments("/healthz")
						&& !httpContext.Request.Path.StartsWithSegments("/metrics")
						&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
						&& !instrumentationExtenionFilters.Any(
							ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
				.AddProfileViewExporter()
				.AddOtlpExporter())
		.WithMetrics(meterBuilder => meterBuilder
			.SetResourceBuilder(ResourceBuilder.CreateDefault()
				.AddEnvironmentVariableDetector())
			.AddAspNetCoreInstrumentation()
			.AddRuntimeInstrumentation()
			.AddOtlpExporter())
		.Services
		.AddHealthChecks()
		.Services
		.AddProfileViewer();
}

var app = builder.Build();
{
	app.MapHealthChecks("/healthz");
	app.MapProfileViewer();
}

app.Run();
