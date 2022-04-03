using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses
{
  public class NotificationStatusUpdateResponseDto
  {
    [DataMember]
    public Guid NotificationId { get; set;} 
  }
}
