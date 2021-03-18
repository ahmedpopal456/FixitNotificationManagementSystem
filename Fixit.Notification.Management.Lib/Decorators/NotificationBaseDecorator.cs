using System;
using System.Text;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Decorators
{
	public class NotificationBaseDecorator
	{
		protected readonly NotificationDto notificationDto;

		public NotificationBaseDecorator(NotificationDto pNotificationDto)
		{
			notificationDto = pNotificationDto;
		}

		public string GetBase64StringConversion()
		{
			string notificationJson = JsonConvert.SerializeObject(notificationDto);
			string base64EncodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(notificationJson));

			return base64EncodedMessage;
		}
	}
}
