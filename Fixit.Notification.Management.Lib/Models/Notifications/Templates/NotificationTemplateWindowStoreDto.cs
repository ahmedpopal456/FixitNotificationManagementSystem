using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Templates
{
  [DataContract]
  public class NotificationTemplateWindowStoreDto : NotificationTemplateBaseDto
  {
    [DataMember]
    public IList<string> Header { get; set; }
  }
}
