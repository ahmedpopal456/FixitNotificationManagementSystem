using Fixit.Core.DataContracts.Notifications.Enums;
using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  public class NotificationStatusUpdateRequestDto : IDtoValidator
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public NotificationStatus Status { get; set; }

    public bool Validate()
    {
      bool isValid = (Id != Guid.Empty);
      return isValid;
    }
  }
}
