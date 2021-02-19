using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Fixit.Notification.Management.Lib.Seeders;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Installations
{
  [DataContract]
  public class DeviceInstallationDto : IFakeSeederAdapter<DeviceInstallationDto>
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

    #region IFakeSeederAdapter
    public IList<DeviceInstallationDto> SeedFakeDtos()
		{
      return new List<DeviceInstallationDto>
      {
        new DeviceInstallationDto
        {
          InstallationId = "445e50d1-b2e7-4c25-a628-c610aed7a357",
          Platform = NotificationPlatform.Fcm,
          Tags = new List<NotificationTagDto>
          {
            new NotificationTagDto { Key = "userId", Value = "445e50d1-b2e7-4c25-a628-c610aed7a357" }
					}
        }
      };
		}
		#endregion
	}
}
