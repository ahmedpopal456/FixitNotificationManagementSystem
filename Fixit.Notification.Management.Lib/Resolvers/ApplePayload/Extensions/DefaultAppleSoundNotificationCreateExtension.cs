using System;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Extensions
{
  [DataContract]
  public static class DefaultAppleSoundNotificationCreateExtension
  {
    public static AppleSoundNotification CreateDefaultNotification(this AppleSoundNotification appleSoundNotification, Guid notificationId, string message, string action, NotificationPayloadDto notificationPayloadDto)
    {
      appleSoundNotification.Action = action;
      appleSoundNotification.ApplePushSettings = new AppleSoundPushSettings()
      {
        Alert = message
      };

      appleSoundNotification.Data = new ApnsData()
      {
        Id = notificationId.ToString(),
        Action = ((int)notificationPayloadDto.Action).ToString(),
        SystemPayload = JsonConvert.SerializeObject(notificationPayloadDto.SystemPayload, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
      };

      return appleSoundNotification;
    }
  }
}
