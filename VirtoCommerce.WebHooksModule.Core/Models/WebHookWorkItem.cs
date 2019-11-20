namespace VirtoCommerce.WebHooksModule.Core.Models
{
    /// <summary>
    /// A work item represents the act of firing a single WebHook.
    /// </summary>
    public class WebHookWorkItem
    {
        /// <summary>
        /// Gets or sets the <see cref=Models.WebHook"/> to fire.
        /// </summary>
        public WebHook WebHook { get; set; }
        /// <summary>
        /// Gets or sets the causing eventId.
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// Gets or sets the attempt count of notification sending.
        /// </summary>
        public int AttemptCount { get; set; }
        /// <summary>
        /// Gets or sets log entry for this work item.
        /// </summary>
        public WebHookFeedEntry FeedEntry { get; set; }

    }
}
