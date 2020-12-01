using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
    public interface IWebHookFeedReader
    {
        Task<IDictionary<string, int>> GetSuccessCountsAsync(string[] webHookIds);
        Task<IDictionary<string, int>> GetErrorCountsAsync(string[] webHookIds);
    }
}
