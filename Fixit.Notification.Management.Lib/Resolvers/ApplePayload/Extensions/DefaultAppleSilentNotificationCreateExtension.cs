using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Extensions
{
  [DataContract]
  public static class DefaultAppleSilentNotificationCreateExtension
  {
    public static AppleSilentNotification CreateDefaultNotification(this AppleSilentNotification appleSilentNotification, Guid notificationId, string message, string action, NotificationPayloadDto notificationPayloadDto)
    {
      appleSilentNotification.Action = action;
      appleSilentNotification.Message = message;

      appleSilentNotification.ApplePushSettings = new AppleSilentPushSettings()
      {
        ApnsPriority = 5,
        Badge = 0,
        Sound = "",
        ContentAvailable = 1
      };

      appleSilentNotification.Data = new ApnsData()
      {
        Id = notificationId.ToString(),
        Action = ((int)notificationPayloadDto.Action).ToString(),
        SystemPayload = JsonConvert.SerializeObject(notificationPayloadDto.SystemPayload, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
      };

      return appleSilentNotification;
    }
  }
}
