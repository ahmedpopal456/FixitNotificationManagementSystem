using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Extensions;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Extensions
{
  [DataContract]
  public static class DefaultFcmSilentNotificationCreateExtension
  {
    public static FcmSilentNotification CreateDefaultNotification(this FcmSilentNotification fcmSilentNotification, Guid notificationId, string message, string action, NotificationPayloadDto notificationPayloadDto)
    {
      fcmSilentNotification.Data = new FcmSilentData()
      {
        Id = notificationId.ToString(),
        Action = ((int)notificationPayloadDto.Action).ToString(),
        Message = message,
        SystemPayload = JObject.FromObject(notificationPayloadDto.SystemPayload).ToCamelCase().ToString()
      };

      return fcmSilentNotification;
    }
  }
}
