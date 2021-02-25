using Fixit.Notification.Management.Lib.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Decorators
{
	public class WindowsNativePayloadDecorator : NotificationBaseDecorator
	{
		public WindowsNativePayloadDecorator(NotificationDto notificationDto) : base(notificationDto)
		{
		}

		public new string GetBase64StringConversion()
		{
			return @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + base.GetByteArrayConversion() + "</text></binding></visual></toast>";
		}
	}
}
