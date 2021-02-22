using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Installations
{
  [DataContract]
  public class DeviceInstallationDocument : DocumentBase, IFakeSeederAdapter<DeviceInstallationDocument>
  {
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
    public long InstalledTimestampUtc { get; set; }

    [DataMember]
    public long UpdatedTimestampUtc { get; set; }

    #region IFakeSeederAdapter
    public IList<DeviceInstallationDocument> SeedFakeDtos()
		{
      return new List<DeviceInstallationDocument>
      {
        new DeviceInstallationDocument
				{
          Platform = NotificationPlatform.Fcm
        }
      };
		}
		#endregion
	}
}
