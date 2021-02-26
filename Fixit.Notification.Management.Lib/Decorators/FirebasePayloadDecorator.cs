using Fixit.Notification.Management.Lib.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Decorators
{
	public class FirebasePayloadDecorator : NotificationBaseDecorator
	{
		public FirebasePayloadDecorator(NotificationDto notificationDto) : base(notificationDto)
		{
		}

		public new string GetBase64StringConversion()
		{
			var message = base.GetBase64StringConversion();
			return "{ \"data\" : {\"message\":\"" + message + "\"}}";
		}
	}
}
