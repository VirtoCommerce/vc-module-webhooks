using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
    /// <summary>
    /// Abstraction for logginng entries to the WebHook event feed.
    /// </summary>
    public interface IWebHookLogger
    {
        /// <summary>
        /// Writes <paramref name="feedEntry"/> to the feed.
        /// </summary>
        /// <param name="feedEntry">Entry to write.</param>
        /// <returns>Created/updated feed entry.</returns>
        Task<WebHookFeedEntry> LogAsync(WebHookFeedEntry feedEntry);
    }
}
