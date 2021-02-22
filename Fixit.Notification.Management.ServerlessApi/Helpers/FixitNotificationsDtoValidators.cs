using Newtonsoft.Json;
using System.Net.Http;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;

namespace Fixit.Notification.Management.ServerlessApi.Helpers
{
  public static class FixitNotificationsDtoValidators
  {
    public static bool IsValidDeviceInstallationUpsertRequest(HttpContent httpContent, out DeviceInstallationUpsertRequestDto deviceInstallationUpsertRequestDto)
    {
      bool isValid = false;
      deviceInstallationUpsertRequestDto = null;

      try
      {
        var deviceInstallationUpsertRequestDeserialized = JsonConvert.DeserializeObject<DeviceInstallationUpsertRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (deviceInstallationUpsertRequestDeserialized != null)
        {
          if (deviceInstallationUpsertRequestDeserialized.Validate())
          {
            deviceInstallationUpsertRequestDto = deviceInstallationUpsertRequestDeserialized;
            isValid = true;
          }
        }
      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    public static bool IsValidEnqueueNotificationRequest(HttpContent httpContent, out EnqueueNotificationRequestDto enqueueNotificationRequestDto)
    {
      bool isValid = false;
      enqueueNotificationRequestDto = null;

      try
      {
        var denqueueNotificationRequestDtoDeserialized = JsonConvert.DeserializeObject<EnqueueNotificationRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (denqueueNotificationRequestDtoDeserialized != null)
        {
          if (denqueueNotificationRequestDtoDeserialized.Validate())
          {
            enqueueNotificationRequestDto = denqueueNotificationRequestDtoDeserialized;
            isValid = true;
          }
        }
      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

    public static bool IsValidDeviceInstallationGetRequest(HttpContent httpContent, out DeviceInstallationGetRequestDto deviceInstallationGetRequest)
    {
      bool isValid = false;
      deviceInstallationGetRequest = null;

      try
      {
        var deviceInstallationGetRequestDeserialized = JsonConvert.DeserializeObject<DeviceInstallationGetRequestDto>(httpContent.ReadAsStringAsync().Result);
        if (deviceInstallationGetRequestDeserialized != null)
        {
          deviceInstallationGetRequest = deviceInstallationGetRequestDeserialized;
          isValid = true;
        }
      }
      catch
      {
        // Fall through 
      }
      return isValid;
    }

  }
}
