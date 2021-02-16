using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models.Notifications.Enums;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class EnqueueNotificationRequestDto : IDtoValidator
  {
    [DataMember]
    public object Payload { get; set; }

    [DataMember]
    public NotificationTypes Action { get; set; }

    [DataMember]
    public IList<KeyValuePair<string, string>> Tags { get; set; }

    [DataMember]
    public IEnumerable<UserSummaryDto> Recipients { get; set; }

    [DataMember]
    public bool Silent { get; set; }

    [DataMember]
    public int Retries { get; set; }

    public bool Validate()
    {
      bool isValid = (Payload != null) ||
                     ((Tags != null && Tags.Any()) || (Recipients != null && Recipients.Any())) &&
                     (Enum.IsDefined(typeof(NotificationTypes), Action));

      return isValid;
    }
  }
}
