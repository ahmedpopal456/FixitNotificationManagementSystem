using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Fixit.Notification.Management.Lib.Seeders;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class DeviceInstallationUpsertRequestDto : IDtoValidator, IFakeSeederAdapter<DeviceInstallationUpsertRequestDto>
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

		#region IDtoValidator
		public bool Validate()
    {
      bool isValid = ((UserId != Guid.Empty) || (Tags != null && Tags.Any()))
                     && !(string.IsNullOrWhiteSpace(PushChannelToken))
                     && !(string.IsNullOrWhiteSpace(InstallationId))
                     && (Enum.IsDefined(typeof(NotificationPlatform), Platform));

      return isValid;
    }
    #endregion

    #region IFakeSeederAdapter
    public IList<DeviceInstallationUpsertRequestDto> SeedFakeDtos()
    {
      return new List<DeviceInstallationUpsertRequestDto>
      {
        new DeviceInstallationUpsertRequestDto
        {

        }
      };
    }
		#endregion
	}
}
