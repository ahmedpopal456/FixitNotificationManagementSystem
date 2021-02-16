using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Templates
{
  [DataContract]
  public class NotificationTemplateBaseDto
  {
    [DataMember]
    public string Body { get; set; }

    [DataMember]
    public IList<KeyValuePair<string, string>> Tags { get; set; }
  }
}
