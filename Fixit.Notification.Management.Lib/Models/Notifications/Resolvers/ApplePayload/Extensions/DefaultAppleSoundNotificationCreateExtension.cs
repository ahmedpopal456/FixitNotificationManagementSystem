using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Extensions
{
	[DataContract]
	public static class DefaultAppleSoundNotificationCreateExtension
	{
		public static AppleSoundNotification CreateDefaultNotification(this AppleSoundNotification appleSoundNotification, string message, string action, object fixitPayload)
		{
			appleSoundNotification.Action = action;

			appleSoundNotification.ApplePushSettings = new AppleSoundPushSettings()
			{
				Alert = message
			};

			appleSoundNotification.FixitData = fixitPayload;

			return appleSoundNotification;
		}
	}
}
