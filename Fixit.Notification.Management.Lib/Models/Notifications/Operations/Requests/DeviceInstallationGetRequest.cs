using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class DeviceInstallationGetRequest
  {
    [DataMember]
    public NotificationPlatform? PlatformType { get; set; }

    [DataMember]
    public IList<NotificationTagDto> Tags { get; set; }

    [DataMember]
    public IEnumerable<Guid> UserIds { get; set; }
  }
}