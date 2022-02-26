using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Extensions;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Extensions;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using System;

namespace Fixit.Notification.Management.Lib.Resolvers
{
  public static class NotificationPayloadResolver
  {
    public static string Resolve(NotificationPayloadDto notificationPayload, Guid id, string title, string message, bool isSilent, NotificationPlatform notificationPlatform)
    {
      return (notificationPlatform, isSilent) switch
      {
        (NotificationPlatform.Apns, true) => JsonConvert.SerializeObject(new AppleSilentNotification().CreateDefaultNotification(id, message, title, notificationPayload)),
        (NotificationPlatform.Apns, false) => JsonConvert.SerializeObject(new AppleSoundNotification().CreateDefaultNotification(id, message, title, notificationPayload)),
        (NotificationPlatform.Fcm, true) => JsonConvert.SerializeObject(new FcmSilentNotification().CreateDefaultNotification(id, message, title, notificationPayload)),
        (NotificationPlatform.Fcm, false) => JsonConvert.SerializeObject(new FcmSoundNotification().CreateDefaultNotification(id, message, title, notificationPayload)),
        _ => string.Empty
      };
    }
  }
}