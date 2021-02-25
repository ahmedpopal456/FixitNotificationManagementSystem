using System;
using System.Collections.Generic;
using System.Text;
using Fixit.Notification.Management.Lib.Models.Notifications;

namespace Fixit.Notification.Management.Lib.Decorators
{
	public class ApplePayloadDecorator : NotificationBaseDecorator
	{
		public ApplePayloadDecorator(NotificationDto notificationDto) : base(notificationDto)
		{
		}

		public new string GetBase64StringConversion()
		{
			return "{\"aps\":{\"alert\":\"" + base.GetByteArrayConversion() + "\"}}";
		}
	}
}
