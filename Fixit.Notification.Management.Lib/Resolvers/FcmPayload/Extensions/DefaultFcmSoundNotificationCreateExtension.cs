using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Extensions;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Extensions
{
  [DataContract]
  public static class DefaultFcmSoundNotificationCreateExtension
  {
    public static FcmSoundNotification CreateDefaultNotification(this FcmSoundNotification fcmSoundNotification, Guid notificationId, string message, string action, NotificationPayloadDto notificationPayloadDto)
    {
      fcmSoundNotification.Action = action;

      fcmSoundNotification.Notification = new FcmNotification
      {
        Title = action,
        Body = message
      };

      fcmSoundNotification.Data = new FcmData
      {
        Id = notificationId.ToString(),
        Action = ((int) notificationPayloadDto.Action).ToString(),
        SystemPayload = !(notificationPayloadDto.SystemPayload is null) ? JObject.FromObject(notificationPayloadDto.SystemPayload).ToCamelCase().ToString() : null
      };

      return fcmSoundNotification;
    }
  }
}
