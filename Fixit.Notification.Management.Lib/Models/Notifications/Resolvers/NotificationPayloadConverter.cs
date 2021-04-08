using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers
{
	public static class NotificationPayloadConverter
	{
		public static string Serialize(NotificationDto notificationDto, NotificationPlatform notificationPlatform)
		{
			return NotificationPayloadResolver.Resolve(notificationDto.Payload, notificationDto.Message, notificationDto.Silent, notificationDto.Action, notificationPlatform);
		}
	}
}