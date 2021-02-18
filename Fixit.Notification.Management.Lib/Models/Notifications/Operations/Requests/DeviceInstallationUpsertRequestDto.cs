using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class DeviceInstallationUpsertRequestDto : IDtoValidator
  {
    [DataMember]
    public string InstallationId { get; set; }

    [DataMember]
    public NotificationPlatform Platform { get; set; }

    [DataMember]
    public string PushChannelToken { get; set; }

    [DataMember]
    public IList<NotificationTagDto> Tags { get; set; }

    [DataMember]
    public IDictionary<string, NotificationTemplateBaseDto> Templates { get; set; }

    [DataMember]
    public Guid UserId { get; set; }

    public bool Validate()
    {
      bool isValid = ((UserId != Guid.Empty) || (Tags != null && Tags.Any()))
                     && !(string.IsNullOrWhiteSpace(PushChannelToken))
                     && !(string.IsNullOrWhiteSpace(InstallationId))
                     && (Enum.IsDefined(typeof(NotificationPlatform), Platform));

      return isValid;
    }
  }
}
