using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Extensions
{
	[DataContract]
	public static class DefaultAppleSilentNotificationCreateExtension
	{
		public static AppleSilentNotification CreateDefaultNotification(this AppleSilentNotification appleSilentNotification, string message, string action, object fixitPayload)
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

			appleSilentNotification.FixitData = fixitPayload;

			return appleSilentNotification;
		}
	}
}
