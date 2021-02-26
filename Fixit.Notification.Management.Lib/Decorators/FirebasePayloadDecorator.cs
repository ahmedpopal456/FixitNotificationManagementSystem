﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fixit.Notification.Management.Lib.Models.Notifications;

namespace Fixit.Notification.Management.Lib.Decorators
{
	public class FirebasePayloadDecorator : NotificationBaseDecorator
	{
		private readonly string Title = "Fixit";

		public FirebasePayloadDecorator(NotificationDto notificationDto) : base(notificationDto)
		{
		}

		public new string GetBase64StringConversion()
		{
			var body = ToSentenceCase(notificationDto.Action.ToString());
			var notification = "\"notification\" : {\"title\":\"" + Title + "\", \"body\":\"" + body + "\"}";

			var message = base.GetBase64StringConversion();
			var data = "\"data\" : {\"message\":\"" + message + "\"}";

			return "{" + notification + "," + data + "}";
		}

		private string ToSentenceCase(string pascalCaseString)
		{
			return Regex.Replace(pascalCaseString, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
		}
	}
}
