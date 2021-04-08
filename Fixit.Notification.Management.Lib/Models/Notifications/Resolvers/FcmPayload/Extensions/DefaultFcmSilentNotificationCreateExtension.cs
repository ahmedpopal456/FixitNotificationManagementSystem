using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Data;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Extensions
{
	[DataContract]
	public static class DefaultFcmSilentNotificationCreateExtension
	{
		public static FcmSilentNotification CreateDefaultNotification(this FcmSilentNotification fcmSilentNotification, object message, string action)
		{
			fcmSilentNotification.Data = new FcmSilentData()
			{
				Action = action,
				Message = message
			};

			return fcmSilentNotification;
		}
	}
}
