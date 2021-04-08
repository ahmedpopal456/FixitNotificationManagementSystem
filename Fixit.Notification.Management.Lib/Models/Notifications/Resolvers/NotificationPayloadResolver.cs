using Fixit.Notification.Management.Lib.Models.Notifications.Enums;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Extensions;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Extensions;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers
{
	public static class NotificationPayloadResolver
	{
		public static string Resolve(object notificationPayload, bool isSilent, NotificationTypes notificationType, NotificationPlatform notificationPlatform)
		{
			switch (notificationPlatform, isSilent)
			{
				case (NotificationPlatform.Apns, true):
					return JsonConvert.SerializeObject(new AppleSilentNotification().CreateDefaultNotification(notificationPayload, notificationType.ToString()));
				case (NotificationPlatform.Apns, false):
					return JsonConvert.SerializeObject(new AppleSoundNotification().CreateDefaultNotification(notificationPayload, notificationType.ToString()));
				case (NotificationPlatform.Fcm, true):
					return JsonConvert.SerializeObject(new FcmSilentNotification().CreateDefaultNotification(notificationPayload, notificationType.ToString()));
				case (NotificationPlatform.Fcm, false):
					return JsonConvert.SerializeObject(new FcmSoundNotification().CreateDefaultNotification(notificationPayload, notificationType.ToString()));
				default:
					return string.Empty;
			}
		}
	}
}