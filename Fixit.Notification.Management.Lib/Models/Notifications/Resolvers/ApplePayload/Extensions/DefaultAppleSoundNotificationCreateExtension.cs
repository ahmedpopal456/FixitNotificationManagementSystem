using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Extensions
{
	[DataContract]
	public static class DefaultAppleSoundNotificationCreateExtension
	{
		public static AppleSoundNotification CreateDefaultNotification(this AppleSoundNotification appleSoundNotification, object message, string action)
		{
			appleSoundNotification.Action = action;

			appleSoundNotification.ApplePushSettings = new AppleSoundPushSettings()
			{
				Alert = message
			};

			return appleSoundNotification;
		}
	}
}
