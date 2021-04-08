using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers
{
	public static class NotificationPayloadConverter
	{
		public static string Serialize(NotificationQueueRequestDto notificationQueueRequestDto, NotificationPlatform notificationPlatform)
		{
			var payload = JsonConvert.SerializeObject(notificationQueueRequestDto.Payload);

			return NotificationPayloadResolver.Resolve(notificationQueueRequestDto.Payload, notificationQueueRequestDto.Silent, notificationQueueRequestDto.Action, notificationPlatform);
		}
	}
}