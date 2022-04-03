using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Core.DataContracts.Notifications.Operations;

namespace Fixit.Notification.Management.ServerlessApi.Utils.Validation
{
  public static class EmpowerNotificationsDtoValidators
  {
    public static bool IsValidDeviceInstallationUpsertRequest(HttpContent httpContent, out DeviceInstallationUpsertRequestDto deviceInstallationUpsertRequestDto)
    {
      bool isValid = false;
      deviceInstallationUpsertRequestDto = null;

      try
      {
        var deviceInstallationUpsertRequestDeserialized = JsonConvert.DeserializeObject<DeviceInstallationUpsertRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (deviceInstallationUpsertRequestDeserialized != null && deviceInstallationUpsertRequestDeserialized.Validate())
        {
          deviceInstallationUpsertRequestDto = deviceInstallationUpsertRequestDeserialized;
          isValid = true;
        }
      }
      catch
      {
        // Fall through.
      }
      return isValid;
    }

    public static bool IsValidEnqueueNotificationRequest(HttpContent httpContent, out EnqueueNotificationRequestDto enqueueNotificationRequestDto)
    {
      bool isValid = false;
      enqueueNotificationRequestDto = null;

      try
      {
        var enqueueNotificationRequestDtoDeserialized = JsonConvert.DeserializeObject<EnqueueNotificationRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (enqueueNotificationRequestDtoDeserialized != null && enqueueNotificationRequestDtoDeserialized.Validate())
        {
          enqueueNotificationRequestDto = enqueueNotificationRequestDtoDeserialized;
          isValid = true;
        }
      }
      catch
      {
        // Fall through.
      }
      return isValid;
    }

    public static bool IsValidNotificationsUpdateStatusRequest(HttpContent httpContent, out IEnumerable<NotificationStatusUpdateRequestDto> notificationStatusUpdateRequestDto)
    {
      bool isValid = false;
      notificationStatusUpdateRequestDto = null;

      try
      {
        var notificationStatusUpdateRequestDtoDeserialized = JsonConvert.DeserializeObject<IEnumerable<NotificationStatusUpdateRequestDto>>(httpContent.ReadAsStringAsync().Result);
        
        if (notificationStatusUpdateRequestDtoDeserialized != null 
          && notificationStatusUpdateRequestDtoDeserialized.Any() 
          && notificationStatusUpdateRequestDtoDeserialized.All(updateRequest => updateRequest.Validate()))
        {
          notificationStatusUpdateRequestDto = notificationStatusUpdateRequestDtoDeserialized;
          isValid = true;
        }
      }
      catch
      {
        // Fall through.
      }
      return isValid;
    }
  }
}
