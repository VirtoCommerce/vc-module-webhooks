using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.BackgroundJobs;

/// <summary>
/// Engine-agnostic background job that sends webhook notifications for an event. Replaces the former
/// Hangfire <c>IBackgroundJobClient.Schedule(() =&gt; NotifyAsync(...))</c> call (now fire-and-forget).
/// </summary>
public class NotifyWebhookJob(IWebHookManager webHookManager) : IBackgroundJobHandler<WebhookRequest>
{
    public Task Execute(WebhookRequest payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        => webHookManager.NotifyAsync(payload, cancellationToken);
}
