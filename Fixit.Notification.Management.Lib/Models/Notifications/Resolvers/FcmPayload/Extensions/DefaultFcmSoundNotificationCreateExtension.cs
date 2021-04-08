using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Notifications;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Extensions
{
	[DataContract]
	public static class DefaultFcmSoundNotificationCreateExtension
	{
		public static FcmSoundNotification CreateDefaultNotification(this FcmSoundNotification fcmSoundNotification, string message, string action, object fixitData)
		{
			fcmSoundNotification.Action = action;

			fcmSoundNotification.Notification = new FcmNotification
			{
				Title = action,
				Body = message
			};

			fcmSoundNotification.Data = new FcmData
			{
				Action = action,
				FixitData = fixitData
			};

			return fcmSoundNotification;
		}
	}
}
