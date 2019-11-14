using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookEntity : AuditableEntity
    {
        public WebHookEntity()
        {
            Events = new NullCollection<WebHookEventEntity>();
        }

        [StringLength(1024)]
        public string Name { get; set; }
        [StringLength(2083)]
        public string Url { get; set; }
        [StringLength(128)]
        public string ContentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllEvents { get; set; }
        public long RaisedEventCount { get; set; }
        public virtual ObservableCollection<WebHookEventEntity> Events { get; set; }
    }
}