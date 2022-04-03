using System;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Extensions;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        SystemPayload = !(notificationPayloadDto.SystemPayload is null) ? JObject.FromObject(notificationPayloadDto.SystemPayload).ToCamelCase().ToString() : null
      };

      return appleSoundNotification;
    }
  }
}
