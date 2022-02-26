using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses
{
  [DataContract]
  public class NotificationsDeleteResponse
  {
    [DataMember]
    public Guid NotificationId {get; set;}
  }
}
