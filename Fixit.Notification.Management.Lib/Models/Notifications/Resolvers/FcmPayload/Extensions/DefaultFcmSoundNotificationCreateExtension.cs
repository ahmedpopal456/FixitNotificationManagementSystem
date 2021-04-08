using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Notifications;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Extensions
{
	[DataContract]
	public static class DefaultFcmSoundNotificationCreateExtension
	{
		public static FcmSoundNotification CreateDefaultNotification(this FcmSoundNotification fcmSoundNotification, object message, string action)
		{
			fcmSoundNotification.Action = action;

			fcmSoundNotification.Notification = new FcmNotification()
			{
				Title = action,
				Body = message
			};

			return fcmSoundNotification;
		}
	}
}
