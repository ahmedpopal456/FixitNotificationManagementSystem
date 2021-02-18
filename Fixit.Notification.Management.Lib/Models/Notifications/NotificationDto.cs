using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models.Notifications.Enums;
using Fixit.Notification.Management.Lib.Models.Notifications.Payloads;

namespace Fixit.Notification.Management.Lib.Models.Notifications
{
  [DataContract]
  public class NotificationDto
  {
    [DataMember]
    public PayloadBaseDto Payload { get; set; }

    [DataMember]
    public NotificationTypes Action { get; set; }

    [DataMember]
    public IList<NotificationTagDto> Tags { get; set; }

    [DataMember]
    public IEnumerable<UserSummaryDto> Recipients { get; set; }

    [DataMember]
    public bool Silent { get; set; }

    [DataMember]
    public long CreatedTimestampUtc { get; set; }

    [DataMember]
    public int Retries { get; set; }
  }
}
