using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Installations
{
  [DataContract]
  public class DeviceInstallationDto
  {
    [DataMember]
    public string InstallationId { get; set; }

    [DataMember]
    public NotificationPlatform Platform { get; set; }

    [DataMember]
    public string PushChannelToken { get; set; }

    [DataMember]
    public bool PushChannelTokenExpired { get; set; }

    [DataMember]
    public IList<NotificationTagDto> Tags { get; set; }

    [DataMember]
    public IDictionary<string, NotificationTemplateBaseDto> Templates { get; set; }

    [DataMember]
    public Guid UserId { get; set; }

    [DataMember]
    public long InstalledTimestampUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampUtc { get; set; }
  }
}
