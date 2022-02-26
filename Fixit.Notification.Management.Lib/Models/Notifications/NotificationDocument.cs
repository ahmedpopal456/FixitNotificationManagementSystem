using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Auditables;
using Fixit.Core.DataContracts.Notifications.Enums;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Core.DataContracts.Users;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications
{
  [DataContract]
  public class NotificationDocument : DocumentBase, ITimeTraceableEntity
  {
    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public NotificationPayloadDto Payload { get; set; }

    [DataMember]
    public UserBaseDto RecipientUser { get; set; }

    [DataMember]
    public NotificationStatus Status { get; set; }

    [DataMember]
    public bool Silent { get; set; }

    [DataMember]
    public long CreatedTimestampUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampUtc { get; set; }
  }
}
